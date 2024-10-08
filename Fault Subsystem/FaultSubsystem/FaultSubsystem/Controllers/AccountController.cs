using FaultSubsystem.Data;
using FaultSubsystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using FaultSubsystem.Models.Account;

namespace FaultSubsystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _dBContext;

        public AccountController(ApplicationDbContext context)
        {
            _dBContext = context;
        }

        [HttpGet]
        public IActionResult CreateAccount()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAccount(CreateAccountViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Does email already exist
            var existingUser = await _dBContext.User.FirstOrDefaultAsync(u => u.Email == model.Email);

            if (existingUser != null)
            {
                ModelState.AddModelError("", "An account with this email already exists.");
                return View(model);
            }

            // Hash password
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

            // Increment ID, when deleting is enabled you go through all ids and count, if no id of that exist that is the new id
            var maxUsers = await _dBContext.User.MaxAsync(u => (int?)u.UserID) ?? 0;

            var newUserID = maxUsers + 1;

            // Create the new user
            var user = new User
            {
                UserID = newUserID,
                Email = model.Email,
                Password = passwordHash,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
                AccountActive = true
            };

            _dBContext.User.Add(user);
            await _dBContext.SaveChangesAsync();

            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _dBContext.User.FirstOrDefaultAsync(u => u.Email == model.Email);

                // Successful Log in
                if (user != null && BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
                {
                    // Store a claim for accessing logged in user information in other views and controllers
                    var claims = new List<Claim>
                    {
                        new Claim("UserID", user.UserID.ToString()),
                        new Claim(ClaimTypes.Name, user.FirstName),
                        new Claim(ClaimTypes.Surname, user.LastName),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.OtherPhone, user.PhoneNumber)
                    };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    // Check the users role and load the proper dashboard

                    return RedirectToAction("Dashboard", "Admin");
                }

                ModelState.AddModelError("", "Invalid email or password.");
            }

            return View(model);
        }
    }
}
