package config

import "os"

type Config struct {
	ConStr       string
	Port         string
	RedisAddr    string
	RedisPass    string
}

func LoadConfig() *Config {
	return &Config{
		ConStr: os.Getenv("CON_STR"),
		Port: os.Getenv("PORT"),
		RedisAddr: os.Getenv("REDIS_ADDRESS"),
		RedisPass: os.Getenv("REDIS_PASSWORD"),
	}
}
