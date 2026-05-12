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

type Handler struct {
	DB *sql.DB
	Cache *cache.Cache
}

func NewHandler(db *sql.DB, cache *cache.Cache) *Handler {
	return &Handler{
		DB: db,
		Cache: cache,
	}
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
	originalUrl, err := h.Cache.Client.Get(ctx, cacheKey).Result()
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
	var dbURL string
	var expiresAt sql.NullTime

	dbStart := time.Now()

	query := "SELECT OriginalLink, ExpiresAt FROM ShortUrls WHERE ShortCode = @ShortCode AND IsActive = 1"
	err = h.DB.QueryRowContext(ctx, query, sql.Named("ShortCode", shortCode)).Scan(&dbURL, &expiresAt)
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

	if expiresAt.Valid {
		now := time.Now()
		if now.After(expiresAt.Time){
			log.Printf("redirect expired shortCode=%s expiresAt=%s", shortCode, expiresAt.Time)
			http.NotFound(w, r)
			return
		}

		ttl = time.Until(expiresAt.Time)
		if ttl > maxTTL {
			ttl = maxTTL
		}
	}else {
		ttl = maxTTL
	}
	
	go func()  {
		backgroundCtx, cancel := context.WithTimeout(context.Background(), 2*time.Second)
		defer cancel()
		_ = h.Cache.Client.Set(backgroundCtx, cacheKey, dbURL, ttl).Err()
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