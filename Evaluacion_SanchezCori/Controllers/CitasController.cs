using Evaluacion_SanchezCori.Data;
using Evaluacion_SanchezCori.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Evaluacion_SanchezCori.Controllers
{
    [Authorize]
    public class CitasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CitasController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var consulta = _context.Citas
                .Include(c => c.Mascota)
                .ThenInclude(m => m!.Usuario)
                .Include(c => c.ServicioVeterinario)
                .AsQueryable();

            if (!User.IsInRole("Administrador"))
            {
                string? usuarioId =
                    _userManager.GetUserId(User);

                consulta = consulta.Where(
                    c =>
                        c.Mascota != null &&
                        c.Mascota.UsuarioId == usuarioId
                );
            }

            var citas = await consulta
                .OrderByDescending(c => c.FechaCita)
                .ToListAsync();

            return View(citas);
        }

        [Authorize(Roles = "Cliente")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await CargarListas();

            return View(
                new Cita
                {
                    FechaCita = DateTime.Today
                }
            );
        }

        [Authorize(Roles = "Cliente")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            Cita cita)
        {
            string usuarioId =
                _userManager.GetUserId(User)!;

            bool mascotaPertenece =
                await _context.Mascotas.AnyAsync(
                    m =>
                        m.Id == cita.MascotaId &&
                        m.UsuarioId == usuarioId
                );

            if (!mascotaPertenece)
            {
                ModelState.AddModelError(
                    nameof(Cita.MascotaId),
                    "La mascota seleccionada no pertenece a tu cuenta."
                );
            }

            bool servicioExiste =
                await _context.ServiciosVeterinarios
                    .AnyAsync(
                        s =>
                            s.Id ==
                            cita.ServicioVeterinarioId
                    );

            if (!servicioExiste)
            {
                ModelState.AddModelError(
                    nameof(Cita.ServicioVeterinarioId),
                    "Seleccione un servicio válido."
                );
            }

            if (cita.FechaCita.Date < DateTime.Today)
            {
                ModelState.AddModelError(
                    nameof(Cita.FechaCita),
                    "La fecha no puede ser anterior a hoy."
                );
            }

            ModelState.Remove(nameof(Cita.Mascota));
            ModelState.Remove(nameof(Cita.ServicioVeterinario));

            if (!ModelState.IsValid)
            {
                await CargarListas();

                return View(cita);
            }

            cita.Estado =
                EstadoCita.Pendiente;

            _context.Citas.Add(cita);

            await _context.SaveChangesAsync();

            TempData["Mensaje"] =
                "Cita registrada correctamente.";

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado(
            int id,
            EstadoCita estado)
        {
            var cita =
                await _context.Citas.FindAsync(id);

            if (cita == null)
            {
                return NotFound();
            }

            cita.Estado = estado;

            await _context.SaveChangesAsync();

            TempData["Mensaje"] =
                "Estado actualizado correctamente.";

            return RedirectToAction(nameof(Index));
        }

        private async Task CargarListas()
        {
            string usuarioId =
                _userManager.GetUserId(User)!;

            var mascotas =
                await _context.Mascotas
                    .Where(
                        m => m.UsuarioId == usuarioId)
                    .OrderBy(m => m.Nombre)
                    .ToListAsync();

            var servicios =
                await _context.ServiciosVeterinarios
                    .OrderBy(s => s.Nombre)
                    .ToListAsync();

            ViewBag.Mascotas =
                new SelectList(
                    mascotas,
                    "Id",
                    "Nombre"
                );

            ViewBag.Servicios =
                new SelectList(
                    servicios,
                    "Id",
                    "Nombre"
                );
        }
    }
}