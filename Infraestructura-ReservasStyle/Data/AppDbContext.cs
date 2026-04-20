using System.Reflection;
using Dominio_ReservasStyle.Entities;
using Microsoft.EntityFrameworkCore;


namespace Infraestructura_ReservasStyle.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Sucursal> Sucursales { get; set; }
        public DbSet<Empleado> Empleados { get; set; }
        public DbSet<Servicio> Servicios { get; set; }
        public DbSet<ServicioSucursal> ServicioSucursal { get; set; }
        public DbSet<Cita> Citas { get; set; }
        public DbSet<Pago> Pagos { get; set; }
        public DbSet<Comprobante> Comprobantes { get; set; }
        public DbSet<Promocion> Promociones { get; set; }
        public DbSet<PromocionServicio> PromocionServicio { get; set; }
        public DbSet<Notificacion> Notificaciones { get; set; }
        public DbSet<HorarioLocal> HorarioLocal { get; set; }
        public DbSet<HorariosDisponibles> HorariosDisponibles { get; set; }
        public DbSet<Log> Logs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("Usuarios");

                entity.HasKey(x => x.IdUsuario);

                entity.Property(x => x.IdUsuario);
                entity.Property(x => x.Nombre).HasMaxLength(100);
                entity.Property(x => x.Apellido).HasMaxLength(100);
                entity.Property(x => x.Email).HasMaxLength(150);
                entity.Property(x => x.Telefono).HasMaxLength(20);
                entity.Property(x => x.ContrasenaHash);
                entity.Property(x => x.FotoPerfil);
                entity.Property(x => x.FechaRegistro);
                entity.Property(x => x.Estado);

                entity.Property(x => x.IdRol);
                entity.Property(x => x.IdSucursal);

                // FK Roles
                entity.HasOne(x => x.Rol)
                      .WithMany(r => r.Usuarios)
                      .HasForeignKey(x => x.IdRol)
                      .HasPrincipalKey(r => r.IdRol)
                      .OnDelete(DeleteBehavior.Restrict);

                // FK Sucursal
                entity.HasOne(x => x.Sucursal)
                    .WithMany(s => s.Usuarios)
                    .HasForeignKey(x => x.IdSucursal)
                    .HasPrincipalKey(s => s.IdSucursal)
                    .OnDelete(DeleteBehavior.Restrict);
            });


            modelBuilder.Entity<Rol>(entity =>
            {
                entity.ToTable("Roles");

                entity.HasKey(x => x.IdRol);

                entity.Property(x => x.NombreRol)
                      .HasMaxLength(50);
            });

            modelBuilder.Entity<Sucursal>(entity =>
            {
                entity.ToTable("Sucursal");

                entity.HasKey(x => x.IdSucursal);
            });

            modelBuilder.Entity<Empleado>(entity =>
            {
                entity.ToTable("Empleados");

                entity.HasKey(e => e.IdEmpleado);

                entity.Property(e => e.IdSucursal)
                      .HasColumnName("IdSucursal");

                entity.HasOne(e => e.Sucursal)
                      .WithMany(s => s.Empleados)
                      .HasForeignKey(e => e.IdSucursal)
                      .HasConstraintName("fk_empleado_sucursal");
            });


            modelBuilder.Entity<Servicio>(entity =>
            {
                entity.ToTable("Servicios");

                entity.HasKey(e => e.IdServicio);

                entity.Property(e => e.IdServicio)
                    .HasColumnName("IdServicio"); 

                entity.Property(e => e.Nombre)
                    .HasColumnName("Nombre")
                    .HasMaxLength(150)
                    .IsRequired();

                entity.Property(e => e.Descripcion)
                    .HasColumnName("Descripcion");

                entity.Property(e => e.DuracionMinutos)
                    .HasColumnName("DuracionMinutos");

                entity.Property(e => e.ImagenUrl)
                    .HasColumnName("Imagen");

                entity.Property(e => e.Estado)
                    .HasColumnName("Estado");

            });

            modelBuilder.Entity<ServicioSucursal>(entity =>
            {
                entity.ToTable("ServicioSucursal");

                entity.HasKey(x => x.IdServicioSucursal);

                entity.Property(x => x.IdServicio)
                      .HasColumnName("IdServicio");

                entity.Property(x => x.IdSucursal)
                      .HasColumnName("IdSucursal");

                entity.Property(x => x.Precio)
                      .HasColumnType("numeric(10,2)");

                entity.HasOne(x => x.Servicio)
                      .WithMany(s => s.ServicioSucursales)
                      .HasForeignKey(x => x.IdServicio)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Sucursal)
                      .WithMany(s => s.ServicioSucursales)
                      .HasForeignKey(x => x.IdSucursal)
                      .OnDelete(DeleteBehavior.Restrict);
            });


            modelBuilder.Entity<Cita>(entity =>
            {
                entity.ToTable("Citas");

                entity.HasKey(x => x.IdCita);
            });

            modelBuilder.Entity<Pago>(entity =>
            {
                entity.ToTable("Pagos");

                entity.HasKey(x => x.IdPago);
            });

            modelBuilder.Entity<Comprobante>(entity =>
            {
                entity.ToTable("Comprobantes");

                entity.HasKey(x => x.IdComprobante);
            });

            modelBuilder.Entity<Promocion>(entity =>
            {
                entity.ToTable("Promociones");

                entity.HasKey(x => x.IdPromocion);
            });

            modelBuilder.Entity<PromocionServicio>(entity =>
            {
                entity.ToTable("PromocionServicio");

                entity.HasKey(x => new { x.IdPromocion, x.IdServicioSucursal });

                entity.Property(x => x.IdPromocion)
                      .HasColumnName("id_promocion");

                entity.Property(x => x.IdServicioSucursal)
                      .HasColumnName("id_servicio_sucursal");

                entity.HasOne(x => x.Promocion)
                      .WithMany()
                      .HasForeignKey(x => x.IdPromocion)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(x => x.ServicioSucursal)
                      .WithMany()
                      .HasForeignKey(x => x.IdServicioSucursal)
                      .OnDelete(DeleteBehavior.Cascade);
            });


            modelBuilder.Entity<Notificacion>(entity =>
            {
                entity.ToTable("Notificaciones");

                entity.HasKey(x => x.IdNotificacion);
            });

            modelBuilder.Entity<HorarioLocal>(entity =>
            {
                entity.ToTable("HorarioLocal");

                entity.HasKey(x => x.IdHorarioLocal);

                entity.Property(x => x.IdSucursal);

                entity.HasOne(x => x.Sucursal)
                      .WithMany()
                      .HasForeignKey(x => x.IdSucursal)
                      .HasPrincipalKey(s => s.IdSucursal)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<HorariosDisponibles>(entity =>
            {
                entity.ToTable("HorariosDisponibles");

                entity.HasKey(x => x.IdHorario);
            });

            modelBuilder.Entity<Log>(entity =>
            {
                entity.ToTable("Logs");

                entity.HasKey(x => x.IdLog);

                entity.Property(x => x.IdLog)
                      .HasColumnName("IdLog");

                entity.Property(x => x.IdUsuario)
                      .HasColumnName("IdUsuario");

                entity.Property(x => x.Accion)
                      .HasColumnName("Accion")
                      .HasMaxLength(100)
                      .IsRequired();

                entity.Property(x => x.Descripcion)
                      .HasColumnName("Descripcion");

                entity.Property(x => x.TablaAfectada)
                      .HasColumnName("TablaAfectada")
                      .HasMaxLength(100);

                entity.Property(x => x.RegistroId)
                      .HasColumnName("RegistroId");

                entity.Property(x => x.Recha)
                      .HasColumnName("Recha")
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(x => x.Ip)
                      .HasColumnName("Ip")
                      .HasMaxLength(50);

                entity.HasOne(x => x.Usuario)
                      .WithMany()
                      .HasForeignKey(x => x.IdUsuario)
                      .OnDelete(DeleteBehavior.SetNull);
            });

        }
    }
}

