using Inventory.Api.Data;
using Microsoft.EntityFrameworkCore;
using Inventory.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Registrar servicios
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configurar la conexión a PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("No se encontró la cadena de conexión DefaultConnection.");
}

builder.Services.AddDbContext<InventoryDbContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddScoped<InventoryService>();

// Construir la aplicación DESPUÉS de registrar los servicios
var app = builder.Build();

// Configurar el pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
