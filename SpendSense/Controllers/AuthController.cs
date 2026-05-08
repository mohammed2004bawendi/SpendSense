using Microsoft.AspNetCore.Mvc;
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

        // Register Page
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Register Action
        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            bool result = await _userService.Register(user);

            if (!result)
            {
                ViewBag.Error = "Bu e-posta adresi zaten kullanılıyor.";
                return View(user);
            }

            TempData["Success"] = "Kayıt başarılı. Giriş yapabilirsiniz.";

            return RedirectToAction("Login");
        }

        // Login Page
        [HttpGet]
       
        public IActionResult Login()
        {
            return View();
        }

        // Login Action
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _userService.Login(email, password);

            if (user == null)
            {
                ViewBag.Error = "E-posta veya şifre hatalı.";
                return View();
            }

            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("Username", user.Username);

            return RedirectToAction("Index", "Home");
        }

        // Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Login");
        }
    }
}