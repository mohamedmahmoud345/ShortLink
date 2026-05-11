# ShortLink

A URL shortening platform built with **ASP.NET Core** and **Go**.  
The C# API handles authentication, link management, and analytics, while the Go redirector serves low‑latency public redirects with Redis caching.

---

## Features (in progress)

- JWT authentication + Identity
- Link CRUD (CQRS)
- Go redirector service
- Redis cache (redirect path)
- Analytics capture
- Docker + CI (planned)

---

## Architecture

```
C# Admin API (SQL Server)  →  Link management + analytics
Go Redirector (Redis)      →  Fast public redirects
```

---

## Tech Stack

- ASP.NET Core
- Go
- SQL Server
- Redis
- MediatR / CQRS
- Swagger


---

## Setup (coming soon)

Full setup instructions will be added later.