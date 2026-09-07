# 🎬 Prime Movies — Microservices Project

A microservices-based backend for a movies catalog. Built to demonstrate a scalable, decoupled architecture where each service is independently deployable and communicates over REST.

> **Purpose:** Academic project demonstrating microservices architecture, REST API design, and containerized deployment.

---

## ✨ Features

- Microservices architecture with independently deployable services
- RESTful API for movie CRUD operations
- Centralized API Gateway for routing and cross-cutting concerns
- Database-per-service pattern
- Service-to-service communication
- Dockerized services with Docker Compose orchestration
- Health check and monitoring endpoints
- Clean separation of concerns and layering

---

## 🛠 Tech Stack

| Layer              | Technology                          |
| ------------------ | ----------------------------------- |
| Language           | Java / Kotlin                       |
| Framework          | Spring Boot                         |
| Microservices      | Spring Cloud (Gateway, OpenFeign)   |
| Database           | PostgreSQL / MySQL                  |
| Containerization   | Docker, Docker Compose              |
| API Style          | REST                                |
| Build Tool         | Maven / Gradle                      |

---

## 🏗 Architecture

```
Client
  │
  ▼
API Gateway (Spring Cloud Gateway)
  │
  ├──► Movies Service
  │        └── Movies DB
  │
  └──► [Additional Services]
           └── Service DB
```

Each microservice:
- Owns its own database
- Exposes its own REST endpoints
- Communicates with other services via HTTP/REST
- Is independently buildable and deployable

---

## 📡 API Endpoints

### Movies Service

| Method | Endpoint              | Description               |
| ------ | --------------------- | ------------------------- |
| GET    | `/api/movies`         | List all movies           |
| GET    | `/api/movies/{id}`    | Get movie by ID           |
| POST   | `/api/movies`         | Create a new movie        |
| PUT    | `/api/movies/{id}`    | Update a movie            |
| DELETE | `/api/movies/{id}`    | Delete a movie            |

### Health Check

| Method | Endpoint              | Description               |
| ------ | --------------------- | ------------------------- |
| GET    | `/actuator/health`    | Service health status     |

---

## 🚀 Getting Started

### Prerequisites

- Java 17+
- Docker & Docker Compose
- PostgreSQL (or MySQL)

### Run with Docker Compose

```bash
git clone https://github.com/TristanJoos/Prime-Movies-Project-MicorService-Movies.git
cd Prime-Movies-Project-MicorService-Movies

docker compose up --build
```

### Run Locally

```bash
# Start the database
docker compose up db -d

# Run the Movies Service
./mvnw spring-boot:run

# Run the API Gateway
./mvnw spring-boot:run -pl gateway
```

---

## 📁 Project Structure

```
Prime-Movies-Project-MicorService-Movies/
├── movies-service/
│   ├── src/
│   │   ├── main/
│   │   │   ├── java/
│   │   │   └── resources/
│   │   └── test/
│   └── pom.xml
├── gateway/
│   ├── src/
│   │   ├── main/
│   │   │   ├── java/
│   │   │   └── resources/
│   │   └── test/
│   └── pom.xml
├── docker-compose.yml
└── README.md
```

---

## 🔗 Links

- [Repository Tickets](#https://github.com/TristanJoos/Prime-Movies-Project-MicorService-Tickets)
- [ Demonstration of full Project](#https://youtu.be/Asxe2ByRsLA)

---

## 📝 Notes

- Each service runs on its own port
- The gateway routes requests to the correct service based on path
- Database migrations are handled per service
