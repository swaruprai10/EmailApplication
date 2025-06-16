using EmailCampaignApp.Models;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using System.Diagnostics;
using System.Net;

namespace EmailCampaignApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _environment;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }

        public IActionResult Index()
        {
            return View(); //return welcome page in landing page
        }

        public IActionResult Home()
        {
            // Check if the user is authenticated
            if (User.Identity.IsAuthenticated)
            {
                ViewData["HideSidebar"] = false;
            }
            else
            {
                ViewData["HideSidebar"] = true;
            }

            return View();
        }

        public IActionResult template()
        {
            return View("template", "template");
        }

        public IActionResult template2()
        {
            return View("template2", "template");
        }

        public IActionResult template3()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult hh()
        {
            return View();
        }

        // POST: /Home/UploadImage
        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile image)
        {
            if (image == null || image.Length == 0)
                return BadRequest("No file was uploaded.");

            try
            {
                var supportedTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
                if (!supportedTypes.Contains(image.ContentType))
                {
                    return BadRequest("Invalid image format. Supported formats are JPG, PNG, GIF, and WebP.");
                }

                var fileName = Path.GetFileNameWithoutExtension(image.FileName) + ".webp";
                var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsPath))
                {
                    Directory.CreateDirectory(uploadsPath);
                }

                var filePath = Path.Combine(uploadsPath, fileName);

                using (var stream = new MemoryStream())
                {
                    await image.CopyToAsync(stream);
                    stream.Position = 0;

                    using (var imageSharp = Image.Load(stream))
                    {
                        imageSharp.Mutate(x => x.Resize(new ResizeOptions
                        {
                            Mode = ResizeMode.Max,
                            Size = new Size(800, 800)
                        }));

                        await imageSharp.SaveAsync(filePath, new WebpEncoder());
                    }
                }

                var fileUrl = $"{Request.Scheme}://{Request.Host}/uploads/{fileName}";
                return Json(new { fileName = fileUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image");
                return BadRequest($"Error uploading image: {ex.Message}");
            }
        }


        [HttpPost]
        public IActionResult DeleteImage([FromBody] DeleteImageRequest request)
        {
            if (string.IsNullOrEmpty(request.ImageUrl))
            {
                _logger.LogWarning("DeleteImage called with empty ImageUrl.");
                return BadRequest(new { success = false, message = "Invalid image URL." });
            }

            // Decode the URL to handle special characters
            var decodedUrl = request.ImageUrl;
            var fileName = WebUtility.UrlDecode(Path.GetFileName(new Uri(decodedUrl).AbsolutePath));

            // Log the decoded file name for debugging
            Console.WriteLine($"Decoded file name: {fileName}");

            // Construct the file path
            var filePath = Path.Combine(_environment.WebRootPath, "uploads", fileName);
            Console.WriteLine($"File path for deletion: {filePath}");

            // Try deleting the file
            if (System.IO.File.Exists(filePath))
            {
                try
                {
                    // Delete the file
                    System.IO.File.Delete(filePath);
                    _logger.LogInformation($"File {fileName} deleted successfully.");
                    return Ok(new { success = true });
                }
                catch (Exception ex)
                {
                    // Log the exception and return an error response
                    _logger.LogError(ex, $"Error deleting file {fileName}");
                    return StatusCode(500, new { success = false, message = $"Error deleting file: {ex.Message}" });
                }
            }
            // If the file does not exist, log a warning and return a not found response
            _logger.LogWarning($"File {fileName} not found for deletion.");
            return NotFound(new { success = false, message = "Image not found." });
        }



        public class DeleteImageRequest
        {
            public string ImageUrl { get; set; }
        }

        [HttpPost]
        public IActionResult SaveText(string content)
        {
            // Add your save logic here
            return Json(new { success = true });
        }

        public IActionResult RedirectBasedOnRole()
        {
            if (User.IsInRole("Admin") || User.IsInRole("Editor"))
            {
                return RedirectToAction("Index", "Contacts");
            }
            else if (User.IsInRole("User") || User.IsInRole("Author"))
            {
                return RedirectToAction("Index", "Contacts");
            }
            else
            {
                // Fallback for users with no roles or unrecognized roles
                return RedirectToAction("Index", "Home");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}