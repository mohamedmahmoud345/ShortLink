package config

import (
	"os"
	"strconv"
)

type Config struct {
	ConStr    string
	Port      string
	RedisAddr string
	RedisPass string
	RateLimit int
}

func LoadConfig() *Config {
	return &Config{
		ConStr:    os.Getenv("CON_STR"),
		Port:      os.Getenv("PORT"),
		RedisAddr: os.Getenv("REDIS_ADDRESS"),
		RedisPass: os.Getenv("REDIS_PASSWORD"),
		RateLimit: getEnvInt("REDIRECTOR_RATE_LIMIT", 20),
	}
}

func getEnvInt(key string, fallback int) int {
	if v, err := strconv.Atoi(os.Getenv(key)); err == nil && v > 0 {
		return v
	}
	return fallback
}
