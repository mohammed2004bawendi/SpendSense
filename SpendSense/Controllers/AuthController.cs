using Microsoft.AspNetCore.Mvc;
using SpendSense.Models;
using SpendSense.Services;

namespace SpendSense.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserService _userService;

        // Injects the UserService dependency via constructor
        public AuthController(UserService userService)
        {
            _userService = userService;
        }

        // Renders the registration form
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Processes registration: validates the model, checks for duplicate email, hashes password, and saves the user
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

        // Renders the login form
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Authenticates the user by email/password and stores UserId and Username in session
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

        // Clears the current session to log the user out and redirects to the login page
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Login");
        }
    }
}
