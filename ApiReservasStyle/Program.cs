using ApiReservasStyle.Middleware;
using Aplicacion_ReservasStyle.Services;
using Dominio_ReservasStyle.Entities;
using Infraestructura_ReservasStyle.Data;
using Infraestructura_ReservasStyle.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

#region ?? RENDER PORT
var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
#endregion

#region ?? CONTROLLERS
builder.Services.AddControllers();
#endregion

#region ?? SWAGGER + JWT
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Pega SOLO el token (sin 'Bearer')"
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
            Array.Empty<string>()
        }
    });
});
#endregion

#region ?? JWT AUTH FIX REAL
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

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var authHeader = context.Request.Headers["Authorization"].ToString();

            Console.WriteLine("?? RAW AUTH HEADER => " + authHeader);

            if (!string.IsNullOrEmpty(authHeader) &&
                authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                context.Token = authHeader["Bearer ".Length..].Trim();
            }

            return Task.CompletedTask;
        },

        OnAuthenticationFailed = context =>
        {
            Console.WriteLine("? JWT FAILED: " + context.Exception.Message);
            return Task.CompletedTask;
        },

        OnChallenge = context =>
        {
            Console.WriteLine("?? JWT CHALLENGE (401)");
            return Task.CompletedTask;
        },

        OnTokenValidated = context =>
        {
            Console.WriteLine("? TOKEN VALID");
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();
#endregion

#region ?? RATE LIMITING
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("loginPolicy", opt =>
    {
        opt.PermitLimit = 5;
        opt.Window = TimeSpan.FromSeconds(10);
    });
});
#endregion

#region ?? CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
#endregion

#region ?? SERVICES
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
#endregion

#region ??? DB
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
#endregion

var app = builder.Build();

#region ?? PIPELINE

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.ConfigObject.AdditionalItems["persistAuthorization"] = true;
});

#?? IMPORTANTÍSIMO: Forwarded Headers (Render / proxies)
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto
});

app.UseCors("AllowAll");

app.UseStaticFiles();
app.UseRateLimiter();

#?? JWT ORDER CORRECTO
app.UseAuthentication();
app.UseAuthorization();

#endregion

#region ?? DEBUG MIDDLEWARE
app.Use(async (context, next) =>
{
    Console.WriteLine("?? REQUEST: " + context.Request.Path);

    var auth = context.Request.Headers["Authorization"].ToString();

    Console.WriteLine(string.IsNullOrWhiteSpace(auth)
        ? "? NO AUTH HEADER"
        : "?? AUTH HEADER => " + auth);

    await next();
});
#endregion

app.MapControllers();

#region ?? MIGRATIONS + SEED
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
#endregion

app.Run();