using Evaluacion_SanchezCori.Data;
using Evaluacion_SanchezCori.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Evaluacion_SanchezCori.Controllers
{
    [Authorize]
    public class ServiciosVeterinariosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ServiciosVeterinariosController(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var servicios =
                await _context.ServiciosVeterinarios
                    .OrderBy(s => s.Nombre)
                    .ToListAsync();

            return View(servicios);
        }

        [Authorize(Roles = "Administrador")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            ServicioVeterinario servicio)
        {
            ModelState.Remove(
                nameof(ServicioVeterinario.Citas)
            );

            if (!ModelState.IsValid)
            {
                return View(servicio);
            }

            servicio.Nombre =
                servicio.Nombre.Trim();

            servicio.Descripcion =
                servicio.Descripcion.Trim();

            _context.ServiciosVeterinarios.Add(
                servicio
            );

            await _context.SaveChangesAsync();

            TempData["Mensaje"] =
                "Servicio registrado correctamente.";

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Administrador")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var servicio =
                await _context.ServiciosVeterinarios
                    .FindAsync(id);

            if (servicio == null)
            {
                return NotFound();
            }

            return View(servicio);
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            ServicioVeterinario servicio)
        {
            if (id != servicio.Id)
            {
                return NotFound();
            }

            ModelState.Remove(
                nameof(ServicioVeterinario.Citas)
            );

            if (!ModelState.IsValid)
            {
                return View(servicio);
            }

            var original =
                await _context.ServiciosVeterinarios
                    .FindAsync(id);

            if (original == null)
            {
                return NotFound();
            }

            original.Nombre =
                servicio.Nombre.Trim();

            original.Descripcion =
                servicio.Descripcion.Trim();

            original.Precio =
                servicio.Precio;

            await _context.SaveChangesAsync();

            TempData["Mensaje"] =
                "Servicio actualizado correctamente.";

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Administrador")]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var servicio =
                await _context.ServiciosVeterinarios
                    .FindAsync(id);

            if (servicio == null)
            {
                return NotFound();
            }

            return View(servicio);
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(
            int id)
        {
            var servicio =
                await _context.ServiciosVeterinarios
                    .FindAsync(id);

            if (servicio == null)
            {
                return NotFound();
            }

            bool tieneCitas =
                await _context.Citas.AnyAsync(
                    c =>
                        c.ServicioVeterinarioId == id
                );

            if (tieneCitas)
            {
                TempData["Error"] =
                    "No se puede eliminar un servicio que tiene citas registradas.";

                return RedirectToAction(nameof(Index));
            }

            _context.ServiciosVeterinarios.Remove(
                servicio
            );

            await _context.SaveChangesAsync();

            TempData["Mensaje"] =
                "Servicio eliminado correctamente.";

            return RedirectToAction(nameof(Index));
        }
    }
}