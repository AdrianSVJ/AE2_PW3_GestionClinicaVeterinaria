using Evaluacion_SanchezCori.Data;
using Evaluacion_SanchezCori.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Evaluacion_SanchezCori.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Dashboard()
        {
            int anio = DateTime.Today.Year;

            var datos = await _context.Citas
                .Where(c => c.FechaCita.Year == anio)
                .GroupBy(c => c.FechaCita.Month)
                .Select(g => new
                {
                    Mes = g.Key,
                    Cantidad = g.Count()
                })
                .ToDictionaryAsync(
                    x => x.Mes,
                    x => x.Cantidad
                );

            var cultura =
                new CultureInfo("es-ES");

            var model = new DashboardViewModel
            {
                TotalServicios =
                    await _context.ServiciosVeterinarios.CountAsync(),

                TotalMascotas =
                    await _context.Mascotas.CountAsync(),

                TotalUsuarios =
                    await _context.Users.CountAsync(),

                TotalCitas =
                    await _context.Citas.CountAsync()
            };

            for (int mes = 1; mes <= 12; mes++)
            {
                model.Meses.Add(
                    cultura.DateTimeFormat
                        .GetAbbreviatedMonthName(mes)
                );

                model.Cantidades.Add(
                    datos.ContainsKey(mes)
                        ? datos[mes]
                        : 0
                );
            }

            return View(model);
        }
    }
}