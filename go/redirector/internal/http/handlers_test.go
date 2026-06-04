package http

import (
	"context"
	"database/sql"
	"errors"
	"net/http"
	"net/http/httptest"
	"testing"
	"time"

	"github.com/go-chi/chi/v5"
	"github.com/redis/go-redis/v9"
)

type mockCacheReader struct {
	data map[string]string
	err  error
}

func (m *mockCacheReader) Get(_ context.Context, key string) (string, error) {
	if m.err != nil {
		return "", m.err
	}
	val, ok := m.data[key]
	if !ok {
		return "", redis.Nil
	}
	return val, nil
}

type setCall struct {
	key        string
	value      interface{}
	expiration time.Duration
}

type mockCacheWriter struct {
	calls []setCall
}

func (m *mockCacheWriter) Set(_ context.Context, key string, value interface{}, expiration time.Duration) error {
	m.calls = append(m.calls, setCall{key, value, expiration})
	return nil
}

type mockLinkFinder struct {
	url       string
	expiresAt *time.Time
	err       error
}

func (m *mockLinkFinder) FindActiveLink(_ context.Context, shortCode string) (string, *time.Time, error) {
	return m.url, m.expiresAt, m.err
}

func withChiContext(req *http.Request, params map[string]string) *http.Request {
	rctx := chi.NewRouteContext()
	for k, v := range params {
		rctx.URLParams.Add(k, v)
	}
	return req.WithContext(context.WithValue(req.Context(), chi.RouteCtxKey, rctx))
}

func TestHandleHealth_Returns200(t *testing.T) {
	h := &Handler{}
	req := httptest.NewRequest("GET", "/healthz", nil)
	rec := httptest.NewRecorder()

	h.HandleHealth(rec, req)

	if rec.Code != http.StatusOK {
		t.Errorf("expected 200, got %d", rec.Code)
	}
	if rec.Body.String() != "OK" {
		t.Errorf("expected body 'OK', got '%s'", rec.Body.String())
	}
}

func TestHandleRedirect_CacheHit_Redirects(t *testing.T) {
	cache := &mockCacheReader{
		data: map[string]string{"link:abc123": "https://example.com"},
	}
	h := &Handler{
		cacheReader: cache,
		cacheWriter: &mockCacheWriter{},
		linkFinder:  &mockLinkFinder{},
	}
	req := httptest.NewRequest("GET", "/abc123", nil)
	req = withChiContext(req, map[string]string{"shortCode": "abc123"})
	rec := httptest.NewRecorder()

	h.HandleRedirect(rec, req)

	if rec.Code != http.StatusFound {
		t.Errorf("expected 302, got %d", rec.Code)
	}
	if loc := rec.Header().Get("Location"); loc != "https://example.com" {
		t.Errorf("expected Location 'https://example.com', got '%s'", loc)
	}
}

func TestHandleRedirect_CacheMissDbHit_Redirects(t *testing.T) {
	cr := &mockCacheReader{data: map[string]string{}}
	h := &Handler{
		cacheReader: cr,
		cacheWriter: &mockCacheWriter{},
		linkFinder:  &mockLinkFinder{url: "https://example.com"},
	}

	req := httptest.NewRequest("GET", "/abc123", nil)
	req = withChiContext(req, map[string]string{"shortCode": "abc123"})
	rec := httptest.NewRecorder()

	h.HandleRedirect(rec, req)

	if rec.Code != http.StatusFound {
		t.Errorf("expected 302, got %d", rec.Code)
	}
	if loc := rec.Header().Get("Location"); loc != "https://example.com" {
		t.Errorf("expected Location 'https://example.com', got '%s'", loc)
	}
}

func TestHandleRedirect_CacheMissDbMiss_Returns404(t *testing.T) {
	cr := &mockCacheReader{data: map[string]string{}}
	h := &Handler{
		cacheReader: cr,
		cacheWriter: &mockCacheWriter{},
		linkFinder:  &mockLinkFinder{err: sql.ErrNoRows},
	}

	req := httptest.NewRequest("GET", "/abc123", nil)
	req = withChiContext(req, map[string]string{"shortCode": "abc123"})
	rec := httptest.NewRecorder()

	h.HandleRedirect(rec, req)

	if rec.Code != http.StatusNotFound {
		t.Errorf("expected 404, got %d", rec.Code)
	}
}

func TestHandleRedirect_CacheError_FailsOpenToDb(t *testing.T) {
	cr := &mockCacheReader{
		data: map[string]string{},
		err:  errors.New("redis connection refused"),
	}
	h := &Handler{
		cacheReader: cr,
		cacheWriter: &mockCacheWriter{},
		linkFinder:  &mockLinkFinder{url: "https://example.com"},
	}

	req := httptest.NewRequest("GET", "/abc123", nil)
	req = withChiContext(req, map[string]string{"shortCode": "abc123"})
	rec := httptest.NewRecorder()

	h.HandleRedirect(rec, req)

	if rec.Code != http.StatusFound {
		t.Errorf("expected 302, got %d", rec.Code)
	}
	if loc := rec.Header().Get("Location"); loc != "https://example.com" {
		t.Errorf("expected Location 'https://example.com', got '%s'", loc)
	}
}

func TestHandleRedirect_ExpiredLink_Returns404(t *testing.T) {
	past := time.Now().Add(-1 * time.Hour)
	cr := &mockCacheReader{data: map[string]string{}}
	h := &Handler{
		cacheReader: cr,
		cacheWriter: &mockCacheWriter{},
		linkFinder:  &mockLinkFinder{url: "https://example.com", expiresAt: &past},
	}

	req := httptest.NewRequest("GET", "/abc123", nil)
	req = withChiContext(req, map[string]string{"shortCode": "abc123"})
	rec := httptest.NewRecorder()

	h.HandleRedirect(rec, req)

	if rec.Code != http.StatusNotFound {
		t.Errorf("expected 404, got %d", rec.Code)
	}
}

func TestHandleRedirect_DbError_Returns500(t *testing.T) {
	cr := &mockCacheReader{data: map[string]string{}}
	h := &Handler{
		cacheReader: cr,
		cacheWriter: &mockCacheWriter{},
		linkFinder:  &mockLinkFinder{err: errors.New("db connection lost")},
	}

	req := httptest.NewRequest("GET", "/abc123", nil)
	req = withChiContext(req, map[string]string{"shortCode": "abc123"})
	rec := httptest.NewRecorder()

	h.HandleRedirect(rec, req)

	if rec.Code != http.StatusInternalServerError {
		t.Errorf("expected 500, got %d", rec.Code)
	}
}
