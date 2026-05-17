using Microsoft.AspNetCore.Mvc;
using SpendSense.DTOs;
using SpendSense.Models;
using SpendSense.Services;

namespace SpendSense.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserService _userService;

        public AuthController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterDto());
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                Password = dto.Password,
                DateOfBirth = dto.DateOfBirth
            };

            bool result = await _userService.Register(user);

            if (!result)
            {
                ViewBag.Error = "Bu e-posta adresi zaten kullanılıyor.";
                return View(dto);
            }

            TempData["Success"] = "Kayıt başarılı. Giriş yapabilirsiniz.";
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginDto());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _userService.Login(dto.Email, dto.Password);

            if (user == null)
            {
                ViewBag.Error = "E-posta veya şifre hatalı.";
                return View(dto);
            }

            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("Username", user.Username);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
