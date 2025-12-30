# TayaAPI

**TayaAPI** is a RESTful backend API built with **.NET 10 and Entity Framework Core** to manage and track bank transactions efficiently. Tested with Postman and Swagger.

---

### Functional Requirements

1. **Paginated movements list API** with filters for:  
   - Date range  
   - Category (e.g., Business Services, Suppliers, Clients, HR Costs, …)  

2. **Summary API** returning additional information for the applied filters:  
   - Total number of movements  
   - Total income  
   - Total expenses  

### Features
- **CRUD operations** for bank movements  
- **Category management**  
- **Date filtering** and **pagination**  
- **Summary endpoint** for financial overview  
- Proper **HTTP status codes**: 200, 404, 500  
- Fully **Swagger-documented**  

### Technologies
- ASP.NET Core Web API  
- Entity Framework Core  
- SQL Server / PostgreSQL  
- Swagger/OpenAPI  

### Endpoints Overview
| Endpoint | Method | Description |
|----------|--------|-------------|
| `/api/Movements` | GET | Retrieve all movements (paginated, filterable by date and category) |
| `/api/Movements/summary` | GET | Get total movements, total income, total expenses |
| `/api/Movements` | POST | Create a new movement |
| `/api/Movements/{id}` | PUT | Update an existing movement |
| `/api/Movements/{id}` | DELETE | Delete a movement |
| `/api/Movements/category/{category}` | GET | Filter movements by category |

### Example Usage
```http
GET /api/Movements/summary?startDate=2025-12-26&endDate=2025-12-29
```
## Getting Started

### Prerequisites
- .NET SDK 8+
- SQL Server
- Entity Framework Core CLI

### Setup

1. Clone the repository:
```bash
git clone https://github.com/tnhdd/TayaAPI.git
```
2. Configure the database connection
You might need to update appsettings.json file
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=MovementDB;Trusted_Connection=True;TrustServerCertificate=True;"
}
```
3. Create the database
You must run
```bash
dotnet ef database update
```
This will:
- Create the database
- Create all tables
- Apply relationships and constraints
4.Run the API
```bash
dotnet run
```
Then open Swagger:
```http
http://localhost:5027/index.html
```


---

**TayaAPI** es una API RESTful desarrollada con **.NET y Entity Framework Core** para gestionar y hacer seguimiento de transacciones bancarias de manera eficiente. Probado con Postman y Swagger.



## Requisitos Funcionales

1. **API de lista de movimientos paginada** con filtros para:  
   - Intervalo de fechas  
   - Categoría (Servicios Empresariales, Proveedores, Clientes, Costes de RR. HH., …)  

2. **API de resumen** que devuelva información adicional según los filtros aplicados:  
   - Número total de movimientos  
   - Ingresos totales  
   - Gastos totales  

---

## Funcionalidades

- **Operaciones CRUD** para movimientos bancarios  
- **Gestión de categorías**  
- **Filtrado por fechas** y **paginación**  
- **Endpoint de resumen** para obtener una visión financiera rápida  
- Uso correcto de **códigos HTTP**: 200, 404, 500  
- Documentación completa con **Swagger**  

---

## Tecnologías

- ASP.NET Core Web API  
- Entity Framework Core  
- SQL Server / PostgreSQL  
- Swagger/OpenAPI  

---

## Endpoints de Ejemplo

| Endpoint | Método | Descripción |
|----------|--------|-------------|
| `/api/Movements` | GET | Obtener todos los movimientos (paginados, filtrables por fecha y categoría) |
| `/api/Movements/summary` | GET | Obtener total de movimientos, ingresos totales y gastos totales |
| `/api/Movements` | POST | Crear un nuevo movimiento |
| `/api/Movements/{id}` | PUT | Actualizar un movimiento |
| `/api/Movements/{id}` | DELETE | Eliminar un movimiento |
| `/api/Movements/category/{category}` | GET | Filtrar movimientos por categoría |

---

## Ejemplo de Uso

```http
GET /api/Movements/summary?startDate=2025-12-26&endDate=2025-12-29
```
## Primeros pasos

### Requisitos previos
- .NET SDK 8 o superior
- SQL Server
- Entity Framework Core CLI

### Configuración

1. Clonar el repositorio:
```bash
git clone https://github.com/tnhdd/TayaAPI.git
```
2. Configurar la conexión a la base de datos. Es posible que necesites actualizar el archivo appsettings.json:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=MovementDB;Trusted_Connection=True;TrustServerCertificate=True;"
}
```
3. Crear la base de datos. Debes ejecutar:
```bash
dotnet ef database update
```
Esto realizará lo siguiente:
- Creará la base de datos
- Creará todas las tablas
- Aplicará las relaciones y restricciones
4.Run the API
```bash
dotnet run
```
4.Ejecutar la API
```bash
dotnet run
```
Luego abre Swagger en el navegador:
```http
http://localhost:5027/index.html
```
