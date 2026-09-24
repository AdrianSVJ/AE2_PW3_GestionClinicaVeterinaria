using Evaluacion_SanchezCori.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Evaluacion_SanchezCori.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Mascota> Mascotas => Set<Mascota>();

        public DbSet<ServicioVeterinario> ServiciosVeterinarios
            => Set<ServicioVeterinario>();

        public DbSet<Cita> Citas => Set<Cita>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Mascota>()
                .HasOne(m => m.Usuario)
                .WithMany(u => u.Mascotas)
                .HasForeignKey(m => m.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Cita>()
                .HasOne(c => c.Mascota)
                .WithMany(m => m.Citas)
                .HasForeignKey(c => c.MascotaId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Cita>()
                .HasOne(c => c.ServicioVeterinario)
                .WithMany(s => s.Citas)
                .HasForeignKey(c => c.ServicioVeterinarioId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ServicioVeterinario>()
                .Property(s => s.Precio)
                .HasColumnType("decimal(10,2)");
        }
    }
}