using AspNetCore.Identity.Mongo.Model;
using EmailCampaignApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver.Linq;

namespace EmailCampaignApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<MongoRole> _roleManager;

        public AdminController(UserManager<ApplicationUser> userManager, RoleManager<MongoRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Dashboard()
        {
            var users = await _userManager.Users.ToListAsync();
            var model = new AdminDashboardViewModel
            {
                TotalUsers = users.Count,
                Users = users
            };
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> ManageRoles(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            // Convert IList<string> to List<string>
            var roles = (await _userManager.GetRolesAsync(user)).ToList();
            var allRoles = _roleManager.Roles.Select(r => r.Name).ToList();

            var model = new ManageRolesViewModel
            {
                UserId = userId,
                UserName = user.UserName,
                UserRoles = roles, // Now this is List<string>
                AllRoles = allRoles
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ManageRoles(ManageRolesViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.RemoveFromRolesAsync(user, userRoles);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Failed to remove existing roles.");
                return View(model);
            }

            result = await _userManager.AddToRolesAsync(user, model.SelectedRoles);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Failed to add new roles.");
                return View(model);
            }

            return RedirectToAction("Dashboard");
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var model = new EditUserViewModel
            {
                UserId = user.Id.ToString(),
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return NotFound();
            }

            user.UserName = model.UserName;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return RedirectToAction("Dashboard", "Admin");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        public async Task<IActionResult> BlockUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            // Set lockout to true and lockoutEnd to MaxValue (blocks indefinitely)
            user.LockoutEnabled = true;
            user.LockoutEnd = DateTimeOffset.MaxValue;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "User has been blocked successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to block user.";
            }

            return RedirectToAction("Dashboard");
        }

        public async Task<IActionResult> UnblockUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            // Remove lockout by setting LockoutEnd to null (unblocks)
            user.LockoutEnd = null;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "User has been unblocked successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to unblock user.";
            }

            return RedirectToAction("Dashboard");
        }




        [HttpGet]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var model = new DeleteUserViewModel
            {
                UserId = user.Id.ToString(),
                UserName = user.UserName,
                Email = user.Email
            };

            return View(model); // Confirmation view for deletion
        }
        [HttpPost]
        [ActionName("DeleteUser")]
        public async Task<IActionResult> DeleteUserConfirmed(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            // Optionally, remove the user's roles first
            var userRoles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.RemoveFromRolesAsync(user, userRoles);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Failed to remove user roles.");
                return View("Error");
            }

            // Now delete the user
            var deleteResult = await _userManager.DeleteAsync(user);

            if (deleteResult.Succeeded)
            {
                return RedirectToAction("Dashboard", "Admin"); // Redirect back to dashboard
            }

            // Handle errors
            foreach (var error in deleteResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View("Error"); // Handle the error and display appropriate message
        }

    }
}