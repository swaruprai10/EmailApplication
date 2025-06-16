using EmailCampaignApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ILogger<AccountController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }

    //[Authorize(Roles = "Admin")]
    [HttpGet]
    public IActionResult Register()
    {

        return View();
    }
    //[Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Register(RegisterModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new ApplicationUser { UserName = model.Username, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Assign a default role to the user
                await _userManager.AddToRoleAsync(user, "User");

                //await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Home", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        return View(model);
    }

    [HttpGet]
    public IActionResult Login()
    {
        ViewData["HideSidebar"] = true;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginModel model)
    {
        if (ModelState.IsValid)
        {
            _logger.LogInformation($"Attempting to log in with email: {model.Email}");

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                _logger.LogWarning($"User not found for email: {model.Email}");
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                _logger.LogInformation($"User logged in successfully: {user.UserName}");


                // Check the user's role and redirect accordingly

                var roles = await _userManager.GetRolesAsync(user);

                _logger.LogInformation($"Roles for user {user.UserName}: {string.Join(", ", roles)}");
                if (roles.Contains("Admin"))
                {
                    return RedirectToAction("Home", "Home");
                }
                else
                {
                    return RedirectToAction("Dashboard", "Email");
                }
            }
            else
            {
                ViewData["HideSidebar"] = true;
                _logger.LogWarning($"Invalid login attempt for user: {user.UserName}");
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
        }
        _logger.LogInformation("Returning to login view with model state errors.");
        ViewData["HideSidebar"] = true;
        return View(model);
    }
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        _logger.LogInformation("Attempting to log out user.");

        await _signInManager.SignOutAsync();
        _logger.LogInformation("User logged out successfully.");

        _logger.LogInformation("Redirecting to Home/Index.");
        return RedirectToAction("Index", "Home");
    }
}