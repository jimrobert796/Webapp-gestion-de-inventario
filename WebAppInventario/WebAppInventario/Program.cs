using FacturacionElectronica.Models;
using FacturacionElectronica.Services;
using Microsoft.EntityFrameworkCore;
using WebAppInventario.Models;
using WebAppInventario.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ======= SERVICIOS =========

// Configuración SMTP
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
var smtpSettings = builder.Configuration.GetSection("SmtpSettings").Get<SmtpSettings>();
if (smtpSettings != null)
    builder.Services.AddSingleton(smtpSettings);

// Servicio de facturación
builder.Services.AddScoped<IInvoiceService, InvoiceService>();


// --- 2. AÑADIR ESTAS DOS LÍNEAS PARA TWILIO ---
builder.Services.Configure<TwilioConfig>(
    builder.Configuration.GetSection("TwilioConfig"));

builder.Services.AddSingleton<WhatsAppService>();
// --- FIN DE LO AÑADIDO ---




// Configurar el contexto de la base de datos para conexion SQL Server
builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
