using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace EmailCampaignApp.Controllers
{
    [Authorize(Roles = "User")]
    public class UserController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
        public IActionResult Contacts()
        {
            // Redirect to the Contacts/Index action
            //return RedirectToAction("Index", "User");
            return RedirectToAction("Index", "Contacts");
        }

        public IActionResult SendEmail()
        {
            // Redirect to the Email/SendEmail action
            return RedirectToAction("SendEmail", "Email");
        }
    }
}