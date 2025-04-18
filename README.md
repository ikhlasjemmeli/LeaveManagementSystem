#  Leave Management System API

This project simulates a leave management system developed with **ASP.NET Core Web API**. It includes functionalities for filtering, pagination, business logic, reports, and deployment via Docker and Azure.

---

##  Features

- CRUD operations for leave requests (`LeaveRequest`)
- Dynamic filtering (employee, leave type, status, date range…)
- Pagination, sorting, keyword search
- Business rules:
  - No overlapping leave dates per employee
  - Max 20 annual leave days per year
  - Sick leave requires a non-empty reason
- Yearly summary report per employee
- Admin approval endpoint
- Dockerized and deployed to Azure

---

##  Tech Stack

- ASP.NET Core Web API (.NET 7)
- Entity Framework Core + SQLite
- AutoMapper
- Swagger for API documentation
- Docker & Docker Compose
- Azure App Service (deployment)

---


##  Key Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/leaverequests` | Retrieve all leave requests |
| GET | `/api/leaverequests/{id}` | Retrieve a specific leave request by ID |
| POST | `/api/leaverequests` | Create a new leave request |
| PUT | `/api/leaverequests/{id}` | Update an existing leave request |
| DELETE | `/api/leaverequests/{id}` | Delete a leave request |
| GET | `/api/leaverequests/filter?status=Pending&page=1&pageSize=5` | Filter leave requests with pagination and sorting |
| GET | `/api/leaverequests/report?year=2025` | Generate a leave summary report for a specific year |
| POST | `/api/leaverequests/{id}/approve` | Approve a pending leave request |


 Swagger available at `/swagger`


## Design Pattern Used: Repository Pattern
This project uses the Repository Pattern to encapsulate and abstract data access logic. By using this pattern, we maintain a clean separation between business logic and data persistence logic, making the application easier to maintain and test.

### Where it’s Used:
- An interface (e.g., ILeaveRequestRepository) defines a contract for data operations such as GetAllAsync, AddAsync, Delete, etc.

- A concrete implementation (e.g., LeaveRequestRepository) interacts with EF Core and the SQLite database.

- Controllers and services depend only on the interface, promoting loose coupling and testability.

### Why Repository Pattern:
- Centralizes data access logic

- Improves testability with mockable interfaces

- Makes the application architecture cleaner and more modular


##  Run Locally

```bash
git clone https://github.com/ikhlasjemmeli/LeaveManagementSystem.git
cd LeaveManagementSystem

# Run using dotnet
dotnet build
dotnet run
