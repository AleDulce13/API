using ApiReservasStyle.Middleware;
using Aplicacion_ReservasStyle.Services;
using Infraestructura_ReservasStyle.Data;
using Infraestructura_ReservasStyle.Repositories;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers()
     .ConfigureApiBehaviorOptions(options =>
     {
         options.SuppressModelStateInvalidFilter = false;
     });

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// RATE LIMITING
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("loginPolicy", opt =>
    {
        opt.PermitLimit = 5;
        opt.Window = TimeSpan.FromSeconds(10);
    });
});

// Services
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<SucursalService>();
builder.Services.AddScoped<HorarioLocalService>();
builder.Services.AddScoped<EmpleadoService>();
builder.Services.AddScoped<ServicioSucursalService>();
builder.Services.AddScoped<CitaService>();
builder.Services.AddScoped<PagoService>();
builder.Services.AddScoped<ComprobanteService>();
builder.Services.AddScoped<CitaService>();
builder.Services.AddScoped<PromocionService>();
builder.Services.AddScoped<NotificacionService>();
builder.Services.AddScoped<PromocionServicioService>();
builder.Services.AddScoped<LogService>(); 
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.UseRateLimiter();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<LogMiddleware>();

app.MapControllers();

app.Run();
