package config

import "os"

type Config struct {
	ConStr       string
	RedisAddress string
	Port         string
}

func LoadConfig() *Config {
	return &Config{
		ConStr: os.Getenv("CON_STR"),
		RedisAddress: os.Getenv("REDIS_ADDRESS"),
		Port: os.Getenv("PORT"),
	}
}
