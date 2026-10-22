# 🏛️ MyApp - Clean Architecture & Tactical Domain-Driven Design (DDD) Web API

[![.NET](https://img.shields.io/badge/.NET-8.0%20%7C%2010.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![Architecture](https://img.shields.io/badge/Architecture-Clean%20Architecture%20%2B%20DDD-blue.svg)](#-arquitectura-del-sistema)
[![Entity Framework Core](https://img.shields.io/badge/ORM-EF%20Core-512BD4.svg)](https://learn.microsoft.com/ef/core/)
[![FluentValidation](https://img.shields.io/badge/Validation-FluentValidation-00b4d8.svg)](https://docs.fluentvalidation.net/)
[![Swagger / OpenAPI](https://img.shields.io/badge/Docs-Swagger%20OpenAPI-85EA2D?logo=swagger&logoColor=black)](#-documentación-interactiva-swagger-ui)

Este proyecto es una **Web API empresarial** construida siguiendo los estándares de **Clean Architecture** y **Domain-Driven Design (DDD) táctico** en C# y .NET, organizada exactamente de acuerdo con la convención estándar adoptada en la solución `MyApp`.

Gestiona tres entidades de negocio centrales: **Customer** (Cliente), **Product** (Producto) y **Category** (Categoría).

---

## 📂 Estructura de la Solución (Estructura de Directorios)

```text
MyApp.sln
 └── src/
      ├── MyApp.Domain/
      │    ├── Abstraccions/
      │    │    ├── Entity.cs                 # Primitiva base para entidades con identidad (Common)
      │    │    ├── AggregateRoot.cs          # Primitiva base para raíces de agregado (Common)
      │    │    └── ValueObject.cs            # Primitiva base para objetos de valor (Common)
      │    ├── Entities/
      │    │    ├── Customer.cs               # Agregado Customer
      │    │    ├── Product.cs                # Agregado Product
      │    │    └── Category.cs               # Agregado Category
      │    ├── Enums/
      │    │    ├── CustomerStatus.cs         # Estados de cliente (Active, Inactive, Suspended)
      │    │    └── ProductStatus.cs          # Estados de producto (Active, Inactive, Discontinued)
      │    ├── Errors/
      │    │    ├── DomainException.cs        # Excepción base con ErrorCode
      │    │    ├── DomainSpecificExceptions.cs # CustomerNotFound, InvalidPrice, InvalidEmail, etc.
      │    │    └── Result.cs                 # Patrón Result y Error funcional
      │    ├── Events/
      │    │    ├── IDomainEvent.cs           # Contrato para eventos de dominio
      │    │    ├── CustomerCreatedEvent.cs   # Evento de cliente creado
      │    │    └── ProductCreatedEvent.cs    # Evento de producto creado
      │    └── ValueObjects/
      │         ├── Email.cs                  # Objeto de valor validado e inmutable
      │         ├── Money.cs                  # Objeto de valor (Monto + Divisa ISO)
      │         └── Sku.cs                    # Objeto de valor para código de inventario
      │
      ├── MyApp.Application/
      │    ├── DTOs/
      │    │    ├── CustomerDtos.cs           # Records Response, Create y Update
      │    │    ├── ProductDtos.cs            # Records Response, Create y Update
      │    │    └── CategoryDtos.cs           # Records Response, Create y Update
      │    ├── Interfaces/
      │    │    ├── IRepositories.cs          # Contratos: ICustomerRepository, IProductRepository, ICategoryRepository, IUnitOfWork
      │    │    └── ServicesInterfaces.cs     # Contratos: ICustomerService, IProductService, ICategoryService, IDateTimeService
      │    ├── Services/
      │    │    ├── CustomerService.cs        # Implementación de casos de uso de Customer
      │    │    ├── ProductService.cs         # Implementación de casos de uso de Product
      │    │    └── CategoryService.cs        # Implementación de casos de uso de Category
      │    ├── Validations/
      │    │    ├── ApplicationValidationException.cs # Excepción de validación de DTOs
      │    │    └── DtoValidators.cs          # Validadores FluentValidation de entrada
      │    └── DependencyInjection.cs         # Inyección de dependencias (AddApplication)
      │
      ├── MyApp.Infraestructure/
      │    ├── Persistence/
      │    │    ├── ApplicationDbContext.cs   # Contexto EF Core
      │    │    ├── DatabaseSeeder.cs         # Siembra inicial idempotente de datos
      │    │    ├── Configurations/           # Fluent API Entity Configurations
      │    │    │    ├── CustomerConfiguration.cs
      │    │    │    ├── ProductConfiguration.cs
      │    │    │    └── CategoryConfiguration.cs
      │    │    └── Repositories/
      │    │         └── RepositoriesImpl.cs  # Repositorios concretos y UnitOfWork
      │    ├── Services/
      │    │    └── DateTimeService.cs        # Proveedor de hora UTC de infraestructura
      │    └── DependencyInjection.cs         # Configuración EF Core y repositorios (AddInfrastructure)
      │
      └── MyApp.Api/
           ├── Controllers/
           │    ├── CustomersController.cs    # Endpoints REST para Customers
           │    ├── ProductsController.cs     # Endpoints REST para Products
           │    └── CategoriesController.cs   # Endpoints REST para Categories
           ├── Middlewares/
           │    └── GlobalExceptionHandlingMiddleware.cs # Interceptor RFC 7807 (ProblemDetails)
           ├── appsettings.json               # Configuración de base de datos SQLite / InMemory
           └── Program.cs                     # Startup, Swagger y Pipeline HTTP
```

---

## 🏛 Principios Clave y Responsabilidad de las Capas

### 1. `MyApp.Domain`
* **Primitivas en `Abstraccions`**: `Entity<TId>`, `AggregateRoot<TId>` y `ValueObject` son las primitivas fundamentales que gobiernan la identidad y la inmutabilidad de los modelos de negocio.
* **Invariantes Ricas en `Entities`**: Constructores privados y métodos mutadores de negocio (`UpdateDetails`, `UpdatePrice`, `ChangeEmail`, `Deactivate`). No existen setters públicos.
* **Inmutabilidad en `ValueObjects`**: `Email`, `Money` y `Sku` protegen sus reglas de formato y validación en su propia construcción.
* **Separación de `Enums`, `Events` y `Errors`**: Cada concepto tiene su carpeta dedicada para máxima cohesión y facilidad de navegación en el explorador de soluciones.

### 2. `MyApp.Application`
* **`DTOs`**: Separados por registros inmutables (`record`) para transporte de información.
* **`Validations` (FluentValidation)**: Reglas de validación aplicadas antes de que los datos alcancen el dominio.
* **`Services` & `Interfaces`**: Casos de uso orquestados limpiamente mediante inyección de dependencias.

### 3. `MyApp.Infraestructure`
* **`Persistence`**: Aísla todo el acceso a datos. Utiliza Fluent API en `Configurations/` para mapear los objetos de valor sin contaminar las entidades.
* **`Services`**: Servicios que interactúan con tecnología externa (como el reloj del sistema en `DateTimeService`).

### 4. `MyApp.Api`
* **`Controllers`**: Endpoints REST con códigos HTTP semánticos (200, 201, 204, 400, 404).
* **`Middlewares`**: `GlobalExceptionHandlingMiddleware` intercepta todas las excepciones y genera respuestas normalizadas **RFC 7807 (Problem Details)**.

---

## 🚀 Guía Rápida de Ejecución

### 1. Compilar la Solución
```powershell
dotnet build MyApp.sln
```

### 2. Ejecutar la Web API
```powershell
dotnet run --project src/MyApp.Api/MyApp.Api.csproj
```

### 3. Acceder a Swagger UI
Abre tu navegador en:
👉 **`http://localhost:5000/`**

---

## 📡 Ejemplos de Pruebas de Endpoints (cURL)

### Consultar Categorías (con datos de prueba sembrados)
```bash
curl -X GET "http://localhost:5000/api/categories"
```

### Crear Producto
```bash
curl -X POST "http://localhost:5000/api/products" \
  -H "Content-Type: application/json" \
  -d '{
    "sku": "AUDIO-HEAD-001",
    "name": "Auriculares Noise Cancelling 700",
    "description": "Auriculares inalámbricos de alta definición.",
    "price": 329.99,
    "currency": "USD",
    "stock": 40,
    "categoryId": "11111111-1111-1111-1111-111111111111"
  }'
```

### Prueba de Error de Dominio: Email Inválido (400 Bad Request)
```bash
curl -X POST "http://localhost:5000/api/customers" \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "Juan",
    "lastName": "Pérez",
    "email": "correo-invalido",
    "phoneNumber": "1234567"
  }'
```
Retorna:
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Error de validación",
  "status": 400,
  "detail": "Ocurrieron uno o más errores de validación en la solicitud.",
  "instance": "/api/customers"
}
```

---

Desarrollado con rigor arquitectónico por el **Riwi Architecture Team**.
