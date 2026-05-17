package main

import (
	"database/sql"
	"log"
	"net/http"
	"time"

	"github.com/didip/tollbooth/v7"
	"github.com/didip/tollbooth/v7/limiter"
	"github.com/go-chi/chi/v5"
	"github.com/go-chi/chi/v5/middleware"
	"github.com/joho/godotenv"
	"github.com/mohamedmahmoud345/shortlink/go/redirector/internal/cache"
	"github.com/mohamedmahmoud345/shortlink/go/redirector/internal/config"

	_ "github.com/denisenkom/go-mssqldb"
	router "github.com/mohamedmahmoud345/shortlink/go/redirector/internal/http"
)

func main() {

	err := godotenv.Load()
	if err != nil {
		log.Println("No .env file found, using system default envs")
	}

	cfg := config.LoadConfig()

	// conect to sql server 
	db, err := sql.Open("sqlserver", cfg.ConStr)
	if err != nil {
		log.Fatalf("Error preparing SQL connection: %v", err)
	}
	defer db.Close()

	if err := db.Ping(); err != nil {
		log.Fatalf("SQL Database is unreachable: %v", err)
	}
	log.Println("successfully connected to sql server")

	redisCache, err := cache.NewCache(cfg.RedisAddr, cfg.RedisPass)
	if err != nil {
		log.Fatalf("Redis Database is unreachable: %v", err)
	}
	log.Println("Successfully connected to Redis Container!")

	lmt := tollbooth.NewLimiter(20, &limiter.ExpirableOptions{DefaultExpirationTTL: time.Hour})
	lmt.SetMessage("Too many redirect requests. Please slow down.")
	lmt.SetStatusCode(http.StatusTooManyRequests)

	h := router.NewHandler(db, redisCache)

	r := chi.NewRouter()
	
	r.Use(middleware.Logger)
	r.Use(middleware.Recoverer)

	r.Get("/healthz", h.HandleHealth)
	r.With(func(next http.Handler) http.Handler {
		return tollbooth.LimitHandler(lmt, next)
	}).Get("/{shortCode}", h.HandleRedirect)

	// 5. Start HTTP Server
	log.Printf("Starting Go redirector on port %s", cfg.Port)
	if err := http.ListenAndServe(":"+cfg.Port, r); err != nil {
		log.Fatal(err)
	}
}