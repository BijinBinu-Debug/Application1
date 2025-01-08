using LoginApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LoginApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AccountController> _logger;

        public AccountController(ApplicationDbContext context, ILogger<AccountController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            _logger.LogInformation($"Attempting to login with email: {email}");

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.Password == password);

            if (user != null)
            {
                // Authentication successful
                _logger.LogInformation($"Login successful for email: {email}");
                return RedirectToAction("Welcome", new { name = user.Name });
            }

            // Authentication failed
            _logger.LogWarning($"Invalid login attempt for email: {email}");
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _logger.LogInformation($"Registering user with email: {user.Email}");

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();

                    // Retrieve the saved user from the database
                    var savedUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);

                    if (savedUser != null)
                    {
                        _logger.LogInformation($"Registration successful for email: {user.Email}");
                    }
                    else
                    {
                        _logger.LogError($"Registration failed for email: {user.Email}");
                    }

                    // Pass the saved user details to the view
                    return View("RegisterSuccess", savedUser);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error registering user: {ex.Message}");
            }

            return View();
        }

        [HttpGet]
        public IActionResult Welcome(string name)
        {
            ViewBag.Name = name;
            return View();
        }
    }
}
