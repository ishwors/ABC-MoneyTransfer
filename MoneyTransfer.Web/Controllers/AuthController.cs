using MoneyTransfer.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MoneyTransfer.Core.DTOs;
using MoneyTransfer.Web.Models.Auth;

namespace MoneyTransfer.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var registerDto = new RegisterDto
                {
                    Email = model.Email,
                    Password = model.Password,
                    FirstName = model.FirstName,
                    MiddleName = model.MiddleName,
                    LastName = model.LastName,
                    Address = model.Address,
                    Country = model.Country
                };

                await _authService.RegisterUserAsync(registerDto);

                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var loginDto = new LoginDto
            {
                Email = model.Email,
                Password = model.Password
            };

            var (success, token) = await _authService.LoginAsync(loginDto);
            if (!success)
            {
                ModelState.AddModelError("", "Invalid email or password");
                return View(model);
            }

            // Store token in an HTTP-only cookie
            Response.Cookies.Append("JWTToken", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // Set to true if using HTTPS
                SameSite = SameSiteMode.Strict
            });

            return RedirectToAction("Index", "Home");
        }


        [HttpPost]
        public IActionResult Logout()
        {
            // Clear the JWT token from the cookies
            Response.Cookies.Delete("JWTToken", new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // Set to true if using HTTPS
                SameSite = SameSiteMode.Strict
            });

            // Redirect to the login page
            return RedirectToAction("Index", "Home");
        }
    }
}
