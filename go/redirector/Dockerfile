FROM golang:1.26-alpine AS build
WORKDIR /app

# Copy dependency files and download them
COPY go.mod go.sum ./
RUN go mod download

# Copy source code and build a statically linked binary
COPY . .
RUN CGO_ENABLED=0 GOOS=linux go build -o /shortlink-redirector ./cmd/redirector/

# STAGE 2: Create a microscopic runtime image using alpine or scratch
FROM alpine:3.21
WORKDIR /

RUN apk --no-cache add ca-certificates tzdata

COPY --from=build /shortlink-redirector /shortlink-redirector

USER nobody:nobody

EXPOSE 8080

ENTRYPOINT ["/shortlink-redirector"]
