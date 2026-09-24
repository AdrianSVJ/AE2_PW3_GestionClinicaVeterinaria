using Evaluacion_SanchezCori.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Evaluacion_SanchezCori.Data
{
    public static class DbInitializer
    {
        public static async Task InicializarAsync(
            IServiceProvider services)
        {
            var context =
                services.GetRequiredService<ApplicationDbContext>();

            var roleManager =
                services.GetRequiredService<RoleManager<IdentityRole>>();

            var userManager =
                services.GetRequiredService<UserManager<ApplicationUser>>();

            await context.Database.MigrateAsync();

            string[] roles =
            {
                "Administrador",
                "Cliente"
            };

            foreach (var rol in roles)
            {
                if (!await roleManager.RoleExistsAsync(rol))
                {
                    await roleManager.CreateAsync(
                        new IdentityRole(rol)
                    );
                }
            }

            string adminEmail =
                "admin@veterinaria.com";

            var admin =
                await userManager.FindByEmailAsync(
                    adminEmail
                );

            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    NombreCompleto = "Administrador",
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var resultado =
                    await userManager.CreateAsync(
                        admin,
                        "Admin123!"
                    );

                if (resultado.Succeeded)
                {
                    await userManager.AddToRoleAsync(
                        admin,
                        "Administrador"
                    );
                }
            }

            if (!await context
                .ServiciosVeterinarios
                .AnyAsync())
            {
                context.ServiciosVeterinarios.AddRange(
                    new ServicioVeterinario
                    {
                        Nombre = "Consulta general",
                        Descripcion =
                            "Evaluación general de la mascota.",
                        Precio = 80
                    },

                    new ServicioVeterinario
                    {
                        Nombre = "Vacunación",
                        Descripcion =
                            "Aplicación y control de vacunas.",
                        Precio = 120
                    },

                    new ServicioVeterinario
                    {
                        Nombre = "Desparasitación",
                        Descripcion =
                            "Tratamiento de desparasitación.",
                        Precio = 90
                    }
                );

                await context.SaveChangesAsync();
            }
        }
    }
}