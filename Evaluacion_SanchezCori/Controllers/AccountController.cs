using Evaluacion_SanchezCori.Models;
using Evaluacion_SanchezCori.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Evaluacion_SanchezCori.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var usuario = await _userManager.FindByEmailAsync(model.Email);

            if (usuario == null)
            {
                ModelState.AddModelError(
                    "",
                    "Correo o contraseña incorrectos."
                );

                return View(model);
            }

            var resultado = await _signInManager.PasswordSignInAsync(
                usuario,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: false
            );

            if (!resultado.Succeeded)
            {
                ModelState.AddModelError(
                    "",
                    "Correo o contraseña incorrectos."
                );

                return View(model);
            }

            if (await _userManager.IsInRoleAsync(
                usuario,
                "Administrador"))
            {
                return RedirectToAction(
                    "Dashboard",
                    "Home"
                );
            }

            return RedirectToAction(
                "Index",
                "Home"
            );
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var usuarioExistente =
                await _userManager.FindByEmailAsync(model.Email);

            if (usuarioExistente != null)
            {
                ModelState.AddModelError(
                    "Email",
                    "Este correo electrónico ya está registrado."
                );

                return View(model);
            }

            var usuario = new ApplicationUser
            {
                NombreCompleto = model.NombreCompleto.Trim(),
                UserName = model.Email.Trim(),
                Email = model.Email.Trim()
            };

            var resultado =
                await _userManager.CreateAsync(
                    usuario,
                    model.Password
                );

            if (!resultado.Succeeded)
            {
                foreach (var error in resultado.Errors)
                {
                    ModelState.AddModelError(
                        "",
                        TraducirError(error.Code)
                    );
                }

                return View(model);
            }

            await _userManager.AddToRoleAsync(
                usuario,
                "Cliente"
            );

            await _signInManager.SignInAsync(
                usuario,
                isPersistent: false
            );

            TempData["Mensaje"] =
                "Cuenta creada correctamente. ¡Bienvenido!";

            return RedirectToAction(
                "Index",
                "Home"
            );
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction(
                "Index",
                "Home"
            );
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        private static string TraducirError(string codigo)
        {
            return codigo switch
            {
                "DuplicateEmail" =>
                    "Este correo electrónico ya está registrado.",

                "DuplicateUserName" =>
                    "Este correo electrónico ya está registrado.",

                "PasswordTooShort" =>
                    "La contraseña es demasiado corta.",

                "PasswordRequiresDigit" =>
                    "La contraseña debe contener al menos un número.",

                "PasswordRequiresUpper" =>
                    "La contraseña debe contener al menos una letra mayúscula.",

                "PasswordRequiresLower" =>
                    "La contraseña debe contener al menos una letra minúscula.",

                _ =>
                    "No se pudo crear la cuenta. Revise los datos ingresados."
            };
        }
    }
}