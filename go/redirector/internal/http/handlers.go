package http

import (
	"net/http"

	"github.com/go-chi/chi/v5"
)

func HandleHealth(w http.ResponseWriter, r *http.Request) {
	w.WriteHeader(http.StatusOK)
	w.Write([]byte("Ok"))
}

func HandleRedirect(w http.ResponseWriter, r *http.Request){
	shortCode := chi.URLParam(r, "shortCode")

	if shortCode == "test" {
		http.Redirect(w, r, "https://google.com", http.StatusFound)
		return
	}

	http.NotFound(w, r)
}