package analytics

import (
	"bytes"
	"encoding/json"
	"log"
	"net/http"
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

	// 4. Perform the POST request
	resp, err := client.Post(csApiUrl, "application/json", bytes.NewBuffer(jsonData))
	if err != nil {
		log.Printf("Analytics: Failed to send data to C#: %v", err)
		return
	}
	defer resp.Body.Close()

	if resp.StatusCode != http.StatusOK {
		log.Printf("Analytics: C# API returned status: %d", resp.StatusCode)
	}
}
