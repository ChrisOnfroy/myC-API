using Microsoft.OpenApi;
using MyApp.Api.Middlewares;
using MyApp.Application;
using MyApp.Infraestructure;
using MyApp.Infraestructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// =========================================================================
// Inyección de dependencias por capas (Clean Architecture)
// =========================================================================

// Capa Application: Casos de uso y validadores
builder.Services.AddApplication();

// Capa Infraestructure: EF Core, Repositorios, UnitOfWork, Servicios externos
builder.Services.AddInfrastructure(builder.Configuration);

// Capa Api: Controladores
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger / OpenAPI con comentarios XML
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "MyApp Web API",
        Description = "Clean Architecture y DDD Táctico en .NET.\n\nEntidades: Customer, Product y Category.",
        Contact = new OpenApiContact
        {
            Name = "Riwi Development Team",
            Email = "dev@riwi.io"
        }
    });

    string[] xmlFiles =
    [
        $"{typeof(Program).Assembly.GetName().Name}.xml",
        $"{typeof(MyApp.Application.DependencyInjection).Assembly.GetName().Name}.xml",
        $"{typeof(MyApp.Domain.Abstraccions.BaseEntity).Assembly.GetName().Name}.xml"
    ];

    foreach (string xmlFile in xmlFiles)
    {
        string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
        }
    }
});

var app = builder.Build();

// =========================================================================
// Pipeline HTTP
// =========================================================================

// Middleware Global de Excepciones
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

// Swagger UI en la raíz
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyApp Web API v1");
    c.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Inicialización y siembra de base de datos
await DatabaseSeeder.SeedAsync(app.Services);

await app.RunAsync();
