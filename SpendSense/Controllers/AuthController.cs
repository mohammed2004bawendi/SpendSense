using Microsoft.AspNetCore.Mvc;
using SpendSense.Models;
using SpendSense.Services;

namespace SpendSense.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserService _userService;

        /// <summary>
        /// Constructor - injects UserService used for registration, login and user management.
        /// The service is resolved from the DI container at runtime.
        /// </summary>
        public AuthController(UserService userService)
        {
            _userService = userService;
        }

        // Register Page
        /// <summary>
        /// GET: Renders the registration page.
        /// </summary>
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Register Action
        /// <summary>
        /// POST: Handles user registration. Validates the model using ModelState,
        /// calls UserService.Register to create the user (service should handle hashing)
        /// and provides user feedback via TempData or ViewBag.
        /// </summary>
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
        /// <summary>
        /// GET: Renders the login page.
        /// </summary>
        [HttpGet]

        public IActionResult Login()
        {
            return View();
        }

        // Login Action
        /// <summary>
        /// POST: Authenticates a user using UserService.Login. On success stores UserId and Username
        /// in session and redirects to Home. On failure returns the login view with an error.
        /// </summary>
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
        /// <summary>
        /// Clears the session to log the user out and redirects to the login page.
        /// </summary>
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Login");
        }
    }
}