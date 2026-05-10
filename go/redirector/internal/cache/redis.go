package cache

import (
	"context"
	"time"

	"github.com/redis/go-redis/v9"
)

type Cache struct {
	Client *redis.Client
}

func NewCache(addr, password string) (*Cache, error) {
	rdb := redis.NewClient(&redis.Options{
		Addr: addr,
		Password: password,
		PoolSize: 50,
		MinIdleConns: 10,
		DialTimeout: 5 * time.Second,
	})

	ctx, cancel := context.WithTimeout(context.Background(), 3 * time.Second)
	defer cancel()

	if err := rdb.Ping(ctx).Err(); err != nil {
		return nil, err
	}

	return &Cache{Client: rdb}, nil
}