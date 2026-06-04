package http

import (
	"context"
	"database/sql"
	"log"
	"net"
	"net/http"
	"strings"
	"time"

	"github.com/go-chi/chi/v5"
	"github.com/mohamedmahmoud345/shortlink/go/redirector/internal/analytics"
	"github.com/mohamedmahmoud345/shortlink/go/redirector/internal/cache"
	"github.com/redis/go-redis/v9"
)

type CacheReader interface {
	Get(ctx context.Context, key string) (string, error)
}
type CacheWriter interface {
	Set(ctx context.Context, key string, value interface{}, expiration time.Duration) error
}
type LinkFinder interface {
	FindActiveLink(ctx context.Context, shortCode string) (originalUrl string, expiresAt *time.Time, err error)
}


type Handler struct {
	linkFinder LinkFinder
	cacheReader CacheReader
	cacheWriter CacheWriter
}
type sqlLinkFinder struct {
	db *sql.DB
}

func NewHandler(db *sql.DB, cache *cache.Cache) *Handler {
    return &Handler{
        linkFinder:  &sqlLinkFinder{db: db},
        cacheReader: &redisCacheReader{client: cache.Client},
        cacheWriter: &redisCacheWriter{client: cache.Client},
    }
}


func (s *sqlLinkFinder) FindActiveLink(ctx context.Context, shortCode string) (string, *time.Time, error) {
    var dbURL string
    var expiresAt sql.NullTime
    query := "SELECT OriginalLink, ExpiresAt FROM ShortUrls WHERE ShortCode = @ShortCode AND IsActive = 1"
    err := s.db.QueryRowContext(ctx, query, sql.Named("ShortCode", shortCode)).Scan(&dbURL, &expiresAt)
    if err != nil {
        return "", nil, err
    }
    if expiresAt.Valid {
        return dbURL, &expiresAt.Time, nil
    }
    return dbURL, nil, nil
}

type redisCacheReader struct {
    client *redis.Client
}
func (r *redisCacheReader) Get(ctx context.Context, key string) (string, error) {
    return r.client.Get(ctx, key).Result()
}

type redisCacheWriter struct {
    client *redis.Client
}
func (r *redisCacheWriter) Set(ctx context.Context, key string, value interface{}, expiration time.Duration) error {
    return r.client.Set(ctx, key, value, expiration).Err()
}


func (h *Handler) HandleHealth(w http.ResponseWriter, r *http.Request) {
	w.WriteHeader(http.StatusOK)
	w.Write([]byte("OK"))
}

func (h *Handler) HandleRedirect(w http.ResponseWriter, r *http.Request){
	ctx := r.Context()
	shortCode := chi.URLParam(r, "shortCode")
	cacheKey := "link:" + shortCode

	start := time.Now()

	// check redis (cache hit?)
	originalUrl, err := h.cacheReader.Get(ctx, cacheKey)
	if err == nil {
 		elapsed := time.Since(start)
        log.Printf("redirect cache_hit shortCode=%s duration_ms=%d ip=%s", shortCode, elapsed.Milliseconds(), clientIP(r))

		payload := analytics.ClickPayload{
        ShortCode: shortCode,
        Referrer:  r.Referer(),
        IpAddress: clientIP(r),
        UserAgent: r.UserAgent(),
    	}
   		go analytics.RecordClick(payload) 
		http.Redirect(w, r, originalUrl, http.StatusFound)
		return 
	}else if err != redis.Nil {
		// Log real redis errors, but don't block the user (fail-open to database)
		log.Printf("redis error getting key=%s err=%v", cacheKey, err)
	}

	// cache miss 
	dbStart := time.Now()

	dbURL, expiresAt, err := h.linkFinder.FindActiveLink(ctx, shortCode)
	dbElapsed := time.Since(dbStart)

	if err != nil {
		if err == sql.ErrNoRows {
			log.Printf("redirect miss notfound shortCode=%s db_ms=%d", shortCode, dbElapsed.Milliseconds())
			http.NotFound(w, r)
		}else {
			log.Printf("redirect db error shortCode=%s err=%v", shortCode, err)
			http.Error(w, "Database error",  http.StatusInternalServerError)
		}
		return
	}

	var ttl time.Duration
	maxTTL := 24 * time.Hour

	if expiresAt != nil {
		now := time.Now()
		if now.After(*expiresAt){
			log.Printf("redirect expired shortCode=%s expiresAt=%s", shortCode, *expiresAt)
			http.NotFound(w, r)
			return
		}

		ttl = time.Until(*expiresAt)
		if ttl > maxTTL {
			ttl = maxTTL
		}
	}else {
		ttl = maxTTL
	}
	
	go func()  {
		backgroundCtx, cancel := context.WithTimeout(context.Background(), 2*time.Second)
		defer cancel()
		_ = h.cacheWriter.Set(backgroundCtx, cacheKey, dbURL, ttl)
	}()

	payload := analytics.ClickPayload{
		ShortCode: shortCode,
        Referrer:  r.Referer(),
        IpAddress: clientIP(r),
        UserAgent: r.UserAgent(),
    }
	log.Printf("Analytics: enqueue click for shortCode=%s ip=%s ref=%s", shortCode, payload.IpAddress, payload.Referrer)
	go analytics.RecordClick(payload)
	
	total := time.Since(start)
    log.Printf("redirect served shortCode=%s total_ms=%d db_ms=%d", shortCode, total.Milliseconds(), dbElapsed.Milliseconds())
	
	http.Redirect(w, r, dbURL, http.StatusFound)	
}

func clientIP(r *http.Request) string {
    // Prefer proxy headers if present
    if fwd := r.Header.Get("X-Forwarded-For"); fwd != "" {
        parts := strings.Split(fwd, ",")
        return strings.TrimSpace(parts[0])
    }
    if real := r.Header.Get("X-Real-IP"); real != "" {
        return real
    }

    host, _, err := net.SplitHostPort(r.RemoteAddr)
    if err == nil && host != "" {
        return host
    }
    return r.RemoteAddr
}