
# User Management API

A RESTful API built with ASP.NET Core and MySQL for managing users with CRUD operations. This application uses raw SQL queries with Dapper for optimal performance and control.

## Features

- User CRUD operations (Create, Read, Update, Delete)
- User types: Worker and Supervisor
- Password hashing for security
- MySQL database with Docker support
- Raw SQL queries using Dapper
- Swagger UI for API documentation
- Connection testing endpoint


## Prerequisites

- .NET 8.0 SDK or later
- Docker and Docker Compose
- Git (optional)


## Getting Started

### 1. Clone or Download the Project

```bash
git clone <repository-url>
cd MyServerApp
```


### 2. Install Dependencies

```bash
dotnet restore
```


### 3. Start MySQL Database

```bash
# Start MySQL container with Docker
sudo docker-compose up -d

# Verify container is running
docker ps

# Check MySQL logs (optional)
docker logs myapp_mysql
```


### 4. Build and Run the Application

```bash
# Build the application
dotnet build

# Run the application
dotnet run

# OR with hot reload for development (recommended)
dotnet watch run
```

The application will start on: `http://localhost:5267`

## API Endpoints

### Base URL: `http://localhost:5267/api/users`

### Swagger UI: `http://localhost:5267/swagger`

### Available Endpoints

| Method | Endpoint | Description |
| :-- | :-- | :-- |
| GET | `/api/users` | Get all users |
| GET | `/api/users/{id}` | Get user by ID |
| GET | `/api/users/type/{type}` | Get users by type (0=Worker, 1=Supervisor) |
| POST | `/api/users` | Create new user |
| PUT | `/api/users/{id}` | Update user |
| DELETE | `/api/users/{id}` | Delete user |
| GET | `/api/users/test-connection` | Test database connection |

## User Model

### Create User Request

```json
{
  "name": "John Doe",
  "email": "john@example.com",
  "phone": "+1234567890",
  "password": "password123",
  "type": 0
}
```


### User Response

```json
{
  "id": 1,
  "name": "John Doe",
  "email": "john@example.com",
  "phone": "+1234567890",
  "type": 0,
  "typeName": "Worker",
  "createdAt": "2025-09-16T10:30:00Z",
  "updatedAt": "2025-09-16T10:30:00Z"
}
```


### User Types

- `0` = Worker
- `1` = Supervisor


## Testing the API

### 1. Test Connection

```bash
curl -X GET "http://localhost:5267/api/users/test-connection"
```


### 2. Get All Users

```bash
curl -X GET "http://localhost:5267/api/users"
```


### 3. Create New User

```bash
curl -X POST "http://localhost:5267/api/users" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Jane Smith",
    "email": "jane@example.com",
    "phone": "+1987654321",
    "password": "securepass123",
    "type": 1
  }'
```


### 4. Get User by ID

```bash
curl -X GET "http://localhost:5267/api/users/1"
```


### 5. Update User

```bash
curl -X PUT "http://localhost:5267/api/users/1" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Jane Updated",
    "email": "jane.updated@example.com"
  }'
```


### 6. Delete User

```bash
curl -X DELETE "http://localhost:5267/api/users/1"
```


## Database Configuration

The application uses MySQL with Docker. Configuration is in `docker-compose.yml`:

- **Host**: localhost
- **Port**: 3307 (to avoid conflicts)
- **Database**: myapp_db
- **Username**: myapp_user
- **Password**: myapp_password


## Project Structure

```
MyServerApp/
├── Controllers/
│   └── UsersController.cs
├── Models/
│   ├── User.cs
│   └── DTOs/
├── Services/
│   ├── IUserService.cs
│   └── UserService.cs
├── Helpers/
│   └── PasswordHelper.cs
├── docker-compose.yml
├── init.sql
└── appsettings.json
```


## Troubleshooting

### Port Already in Use

```bash
# Kill process using the port
sudo kill -9 $(sudo lsof -t -i:5267)
dotnet run
```


### MySQL Container Issues

```bash
# Restart containers
docker-compose down
docker-compose up -d

# Check container logs
docker logs myapp_mysql
```


### Build Errors

```bash
# Clean and rebuild
dotnet clean
dotnet build
```


## Development Commands

```bash
# Run with hot reload
dotnet watch run

# Stop the application
Ctrl + C

# View running processes
ps aux | grep dotnet

# Check port usage
netstat -tlnp | grep :5267
```


## Security Features

- Password hashing using SHA256
- Input validation with Data Annotations
- SQL injection prevention with parameterized queries
- Unique email constraint


## Technologies Used

- ASP.NET Core 8
- MySQL 8.0
- Dapper (Micro ORM)
- MySqlConnector
- Docker \& Docker Compose
- Swagger/OpenAPI

***

**Note**: This is a development setup. For production deployment, ensure proper security configurations, environment variables, and SSL certificates are implemented.
