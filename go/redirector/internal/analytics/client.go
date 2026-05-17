package analytics

import (
	"bytes"
	"encoding/json"
	"log"
	"net/http"
	"os"
	"time"
)

type ClickPayload struct {
	ShortCode string `json:"shortCode"`
	Referrer  string `json:"referrer"`
	IpAddress string `json:"ipAddress"`
	UserAgent string `json:"userAgent"`
}

func RecordClick(payload ClickPayload) {
	// 1. Define the C# internal endpoint
	// In production, move this to an environment variable or config
	csApiUrl := "http://localhost:5218/api/clickevent"

	// 2. Serialize the payload to JSON
	jsonData, err := json.Marshal(payload)
	if err != nil {
		log.Printf("Analytics: Error marshaling payload: %v", err)
		return
	}

	// 3. Create a client with a timeout
	client := &http.Client{
		Timeout: 5 * time.Second,
	}

	token := os.Getenv("INTERNAL_SECURE_TOKEN")

	// 4. Perform the POST request
	req, err := http.NewRequest("POST", csApiUrl, bytes.NewBuffer(jsonData))
	if err != nil {
		log.Printf("Analytics: Failed to build request: %v", err)
		return
	}
	req.Header.Set("Content-Type", "application/json")
	req.Header.Set("INTERNAL_SECURE_TOKEN", token)

	resp, err := client.Do(req)
	if err != nil {
		log.Printf("Analytics: Failed to send data to C#: %v", err)
		return
	}
	defer resp.Body.Close()

	if resp.StatusCode != http.StatusOK {
		log.Printf("Analytics: C# API returned status: %d", resp.StatusCode)
	}
}

