package main

import (
	"log"
	"net/http"

	"github.com/joho/godotenv"
	"github.com/mohamedmahmoud345/shortlink/go/redirector/internal/config"

	router "github.com/mohamedmahmoud345/shortlink/go/redirector/internal/http"
)

func main() {
	err := godotenv.Load()
	if err != nil {
		log.Println("No .env file found, using system default envs")
	}

	cfg := config.LoadConfig()

	// 3. Initialize Chi Router[cite: 1]
	r := router.NewRouter()

	// 4. Start Server[cite: 1]
	log.Printf("Redirector starting on port %s", cfg.Port)
	if err := http.ListenAndServe(":"+cfg.Port, r); err != nil {
		log.Fatal(err)
	}
}