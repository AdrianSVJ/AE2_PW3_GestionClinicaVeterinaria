using Evaluacion_SanchezCori.Data;
using Evaluacion_SanchezCori.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Evaluacion_SanchezCori.Controllers
{
    [Authorize(Roles = "Cliente")]
    public class MascotasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public MascotasController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            string? usuarioId =
                _userManager.GetUserId(User);

            var mascotas = await _context.Mascotas
                .Where(m => m.UsuarioId == usuarioId)
                .OrderBy(m => m.Nombre)
                .ToListAsync();

            return View(mascotas);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            Mascota mascota)
        {
            ModelState.Remove(nameof(Mascota.UsuarioId));
            ModelState.Remove(nameof(Mascota.Usuario));
            ModelState.Remove(nameof(Mascota.Citas));

            if (!ModelState.IsValid)
            {
                return View(mascota);
            }

            mascota.UsuarioId =
                _userManager.GetUserId(User)!;

            _context.Mascotas.Add(mascota);

            await _context.SaveChangesAsync();

            TempData["Mensaje"] =
                "Mascota registrada correctamente.";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            string? usuarioId =
                _userManager.GetUserId(User);

            var mascota = await _context.Mascotas
                .FirstOrDefaultAsync(
                    m =>
                        m.Id == id &&
                        m.UsuarioId == usuarioId
                );

            if (mascota == null)
            {
                return NotFound();
            }

            return View(mascota);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            Mascota mascota)
        {
            if (id != mascota.Id)
            {
                return NotFound();
            }

            ModelState.Remove(nameof(Mascota.UsuarioId));
            ModelState.Remove(nameof(Mascota.Usuario));
            ModelState.Remove(nameof(Mascota.Citas));

            if (!ModelState.IsValid)
            {
                return View(mascota);
            }

            string usuarioId =
                _userManager.GetUserId(User)!;

            var original =
                await _context.Mascotas
                    .FirstOrDefaultAsync(
                        m =>
                            m.Id == id &&
                            m.UsuarioId == usuarioId
                    );

            if (original == null)
            {
                return NotFound();
            }

            original.Nombre =
                mascota.Nombre.Trim();

            original.Especie =
                mascota.Especie.Trim();

            original.Raza =
                mascota.Raza.Trim();

            await _context.SaveChangesAsync();

            TempData["Mensaje"] =
                "Mascota actualizada correctamente.";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            string? usuarioId =
                _userManager.GetUserId(User);

            var mascota = await _context.Mascotas
                .FirstOrDefaultAsync(
                    m =>
                        m.Id == id &&
                        m.UsuarioId == usuarioId
                );

            if (mascota == null)
            {
                return NotFound();
            }

            return View(mascota);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(
            int id)
        {
            string? usuarioId =
                _userManager.GetUserId(User);

            var mascota = await _context.Mascotas
                .FirstOrDefaultAsync(
                    m =>
                        m.Id == id &&
                        m.UsuarioId == usuarioId
                );

            if (mascota == null)
            {
                return NotFound();
            }

            _context.Mascotas.Remove(mascota);

            await _context.SaveChangesAsync();

            TempData["Mensaje"] =
                "Mascota eliminada correctamente.";

            return RedirectToAction(nameof(Index));
        }
    }
}