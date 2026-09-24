using Evaluacion_SanchezCori.Data;
using Evaluacion_SanchezCori.Models;
using Evaluacion_SanchezCori.Services;
using Evaluacion_SanchezCori.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Evaluacion_SanchezCori.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class ReportesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PdfReportService _pdf;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReportesController(
            ApplicationDbContext context,
            PdfReportService pdf,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _pdf = pdf;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var clientes =
                await _userManager
                    .GetUsersInRoleAsync("Cliente");

            ViewBag.Clientes =
                new SelectList(
                    clientes,
                    "Id",
                    "NombreCompleto"
                );

            return View();
        }

        public async Task<IActionResult> CitasGenerales()
        {
            var citas =
                await ConsultaCitas()
                    .OrderBy(c => c.FechaCita)
                    .ToListAsync();

            byte[] archivo =
                _pdf.GenerarCitas(
                    citas,
                    "Listado general de citas"
                );

            return File(
                archivo,
                "application/pdf",
                "ListadoGeneralCitas.pdf"
            );
        }

        public async Task<IActionResult> CitasPorUsuario(
            string usuarioId)
        {
            if (string.IsNullOrWhiteSpace(usuarioId))
            {
                TempData["Error"] =
                    "Seleccione un cliente.";

                return RedirectToAction(nameof(Index));
            }

            var usuario =
                await _userManager
                    .FindByIdAsync(usuarioId);

            if (usuario == null)
            {
                return NotFound();
            }

            var citas =
                await ConsultaCitas()
                    .Where(
                        c =>
                            c.Mascota != null &&
                            c.Mascota.UsuarioId == usuarioId
                    )
                    .OrderBy(c => c.FechaCita)
                    .ToListAsync();

            byte[] archivo =
                _pdf.GenerarCitas(
                    citas,
                    $"Citas de {usuario.NombreCompleto}"
                );

            return File(
                archivo,
                "application/pdf",
                "CitasPorUsuario.pdf"
            );
        }

        public async Task<IActionResult> ServiciosSolicitados()
        {
            var servicios =
                await _context.ServiciosVeterinarios
                    .Select(
                        s =>
                            new ServicioReporteItem
                            {
                                Nombre = s.Nombre,
                                Cantidad =
                                    s.Citas.Count()
                            }
                    )
                    .OrderByDescending(
                        s => s.Cantidad)
                    .ThenBy(s => s.Nombre)
                    .ToListAsync();

            byte[] archivo =
                _pdf.GenerarServicios(servicios);

            return File(
                archivo,
                "application/pdf",
                "ServiciosMasSolicitados.pdf"
            );
        }

        private IQueryable<Cita> ConsultaCitas()
        {
            return _context.Citas
                .Include(c => c.Mascota)
                .ThenInclude(m => m!.Usuario)
                .Include(c => c.ServicioVeterinario);
        }
    }
}