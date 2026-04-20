using ApiReservasStyle.Middleware;
using Aplicacion_ReservasStyle.Services;
using Dominio_ReservasStyle.Entities;
using Infraestructura_ReservasStyle.Data;
using Infraestructura_ReservasStyle.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// PORT RENDER

var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// CONTROLLERS

builder.Services.AddControllers();

// SWAGGER + JWT FIX

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Escribe: Bearer {token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// JWT AUTH

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
        ),
        ClockSkew = TimeSpan.Zero
    };

    // AQUÍ ES DONDE VA EL DEBUG
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine("JWT FAILED:");
            Console.WriteLine(context.Exception.Message);
            return Task.CompletedTask;
        },

        OnChallenge = context =>
        {
            Console.WriteLine("JWT CHALLENGE (401)");
            Console.WriteLine("Error: " + context.Error);
            Console.WriteLine("Description: " + context.ErrorDescription);
            return Task.CompletedTask;
        },

        OnTokenValidated = context =>
        {
            Console.WriteLine("TOKEN VALID");
            Console.WriteLine("User authenticated: " +
                context.Principal.Identity?.IsAuthenticated);
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

// RATE LIMITING

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("loginPolicy", opt =>
    {
        opt.PermitLimit = 5;
        opt.Window = TimeSpan.FromSeconds(10);
    });
});

// CORS

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// SERVICES

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<SucursalService>();
builder.Services.AddScoped<HorarioLocalService>();
builder.Services.AddScoped<EmpleadoService>();
builder.Services.AddScoped<ServicioSucursalService>();
builder.Services.AddScoped<CitaService>();
builder.Services.AddScoped<PagoService>();
builder.Services.AddScoped<ComprobanteService>();
builder.Services.AddScoped<PromocionService>();
builder.Services.AddScoped<NotificacionService>();
builder.Services.AddScoped<PromocionServicioService>();
builder.Services.AddScoped<LogService>();
builder.Services.AddScoped<ServicioService>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// DB

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();


// PIPELINE 

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.ConfigObject.AdditionalItems["persistAuthorization"] = true;
});

// AQUÍ VA TU DEBUG 
app.Use(async (context, next) =>
{
    var auth = context.Request.Headers["Authorization"].ToString();

    if (!string.IsNullOrEmpty(auth))
    {
        Console.WriteLine("?? AUTH HEADER:");
        Console.WriteLine(auth);
    }
    else
    {
        Console.WriteLine("? NO AUTH HEADER");
    }

    await next();
});


app.UseCors("AllowAll");

app.UseStaticFiles();

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// MIGRATIONS + SEED

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    if (!db.Roles.Any())
    {
        db.Roles.AddRange(
            new Rol { NombreRol = "Admin" },
            new Rol { NombreRol = "Cliente" },
            new Rol { NombreRol = "Empleado" }
        );
        db.SaveChanges();
    }
}

app.Run();
