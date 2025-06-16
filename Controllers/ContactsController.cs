using ClosedXML.Excel;
using EmailCampaignApp.Data;
using EmailCampaignApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace EmailCampaignApp.Controllers
{
    [Authorize]
    public class ContactsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ContactsController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Contacts
        public async Task<IActionResult> Index(string searchQuery, int page = 1, int pageSize = 10)
        {
            var user = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(user);

            var contactsQuery = _context.Contacts.AsQueryable();

            // Apply search filter if there's a query
            if (!string.IsNullOrEmpty(searchQuery))
            {
                contactsQuery = contactsQuery.Where(c =>
                    c.Name.Contains(searchQuery) ||
                    c.Email.Contains(searchQuery) ||
                    c.PhoneNumber.Contains(searchQuery) ||
                    c.Address.Contains(searchQuery));
            }

            // Role-based data filtering
            if (roles.Contains("Admin") || roles.Contains("Editor"))
            {
                // Admins & Editors can view all contact details
            }
            else if (roles.Contains("User") || roles.Contains("Author"))
            {
                // Limit data visibility for "User" and "Author"
                contactsQuery = contactsQuery.Select(c => new Contact
                {
                    Name = c.Name,
                    Email = c.Email
                });
            }
            else
            {
                return Forbid(); // Deny access for unauthorized roles
            }

            // Pagination
            int totalContacts = await contactsQuery.CountAsync();
            var contacts = await contactsQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // ViewBag for Pagination and Search
            ViewBag.SearchQuery = searchQuery;
            ViewBag.Page = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalContacts / pageSize);

            return View(contacts);
        }

        // GET: Contacts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Contacts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Contact contact)
        {
            if (ModelState.IsValid)
            {

                foreach (var key in ModelState.Keys)
                {
                    var state = ModelState[key];
                    foreach (var error in state.Errors)
                    {
                        Console.WriteLine($"Field: {key}, Error: {error.ErrorMessage}");
                    }
                }

                return View(contact); // shows the same view with validation errors
                                      //try
                                      //{
                                      //    await _context.Contacts.InsertOneAsync(contact);
                                      //    return RedirectToAction(nameof(Index));
                                      //}
                                      //catch (MongoWriteException)
                                      //{
                                      //    // Handle duplicate key exception or other write errors
                                      //    ModelState.AddModelError("", "An error occurred while creating the contact. Please try again.");
                                      //    // Log the exception if necessary
                                      //}
                                      //catch (Exception)
                                      //{
                                      //    // Handle other exceptions
                                      //    ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
                                      //    // Log the exception if necessary
            }
            //return View(contact);
            await _context.Contacts.InsertOneAsync(contact);
            return RedirectToAction("Index");
        }

        // GET: Contacts/Edit/5
        [Authorize(Roles = "Admin,Editor")] // Only Admin and Editor can edit
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contact = await _context.Contacts.Find(c => c.Id == id).FirstOrDefaultAsync();

            if (contact == null)
            {
                return NotFound();
            }
            return View(contact);
        }

        // POST: Contacts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Editor")] // Only Admin and Editor can edit
        public async Task<IActionResult> Edit(string id, Contact contact)
        {
            if (id != contact.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _context.Contacts.ReplaceOneAsync(c => c.Id == id, contact);
                    if (result.MatchedCount == 0)
                    {
                        // Document with given ID was not found
                        return NotFound();
                    }
                    //if (result.ModifiedCount == 0)
                    //{
                    //    // If no documents were modified, it means the contact was not found
                    //    return NotFound();
                    //}
                    return RedirectToAction(nameof(Index));
                }
                catch (MongoWriteException)
                {
                    // Handle duplicate key exception or other write errors
                    ModelState.AddModelError("", "An database error occurred while updating the contact. Please try again.");
                    // Log the exception if necessary
                }
                catch (Exception)
                {
                    // Handle other exceptions
                    ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
                    // Log the exception if necessary
                }
            }
            return View(contact);
        }

        private async Task<bool> ContactExists(string id)
        {
            return await _context.Contacts.Find(c => c.Id == id).AnyAsync();
        }

        public IActionResult Upload()
        {
            return View();
        }


        //EpPlus
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> UploadExcel(IFormFile file)
        //{
        //    if (file == null || file.Length == 0)
        //    {
        //        ModelState.AddModelError("File", "Please upload a valid Excel file.");
        //        // reload the Index view with the error message
        //        return RedirectToAction(nameof(Index));
        //        //return View("Index");
        //    }

        //    // Check if the file is an Excel file
        //    var contacts = new List<Contact>();

        //    // Validate the file extension
        //    using (var stream = new MemoryStream())
        //    {
        //        // Copy the uploaded file to the memory stream
        //        await file.CopyToAsync(stream);
        //        // Reset the stream position to the beginning
        //        using (var package = new ExcelPackage(stream))
        //        {
        //            // Check if the package contains any worksheets
        //            ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();
        //            if (worksheet == null)
        //            {
        //                ModelState.AddModelError("File", "The uploaded file does not contain any worksheets.");
        //                return RedirectToAction(nameof(Index));
        //            }

        //            // Read the data from the first worksheet
        //            int rowCount = worksheet.Dimension.Rows;
        //            for (int row = 2; row <= rowCount; row++) // Assuming first row is header
        //            {
        //                var contact = new Contact
        //                {
        //                    Name = worksheet.Cells[row, 1].Text,
        //                    Email = worksheet.Cells[row, 2].Text,
        //                    PhoneNumber = worksheet.Cells[row, 3].Text,
        //                    Address = worksheet.Cells[row, 4].Text
        //                };
        //                contacts.Add(contact);
        //            }
        //        }
        //    }

        //    // Validate the contacts before inserting
        //    if (contacts.Any())
        //    {
        //        await _context.Contacts.InsertManyAsync(contacts);
        //    }

        //    return RedirectToAction(nameof(Index));
        //}


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadExcel(IFormFile file)
        {
            // Check if the file is null or empty
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("File", "Please upload a valid Excel file.");
                return RedirectToAction(nameof(Index));
            }

            // Check if the file is an Excel file
            var allowedExtensions = new[] { ".xlsx", ".xls" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(fileExtension))
            {
                ModelState.AddModelError("File", "Please upload a valid Excel file (.xlsx or .xls).");
                return RedirectToAction(nameof(Index));
            }

            // Initialize a list to hold contacts
            var contacts = new List<Contact>();

            // Validate the file and read the Excel data
            try
            {
                using (var stream = new MemoryStream())
                {
                    // Copy the uploaded file to the memory stream
                    await file.CopyToAsync(stream);
                    // Reset the stream position to the beginning
                    stream.Position = 0;

                    // Open the Excel file using ClosedXML
                    using (var workbook = new XLWorkbook(stream))
                    {
                        var worksheet = workbook.Worksheets.FirstOrDefault();
                        if (worksheet == null)
                        {
                            ModelState.AddModelError("File", "The uploaded file does not contain any worksheets.");
                            return RedirectToAction(nameof(Index));
                        }

                        // Check if worksheet has data
                        var rangeUsed = worksheet.RangeUsed();
                        if (rangeUsed == null || rangeUsed.RowCount() <= 1)
                        {
                            ModelState.AddModelError("File", "The uploaded file does not contain any data rows.");
                            return RedirectToAction(nameof(Index));
                        }

                        var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // Skip header row

                        // Iterate through each row and create Contact objects
                        foreach (var row in rows)
                        {
                            // Skip empty rows (check if first two columns are empty)
                            if (string.IsNullOrWhiteSpace(row.Cell(1).GetString()) &&
                                string.IsNullOrWhiteSpace(row.Cell(2).GetString()))
                                continue;

                            // Create a new Contact object from the row data
                            var contact = new Contact
                            {
                                Name = row.Cell(1).GetString()?.Trim(),
                                Email = row.Cell(2).GetString()?.Trim(),
                                PhoneNumber = row.Cell(3).GetString()?.Trim(),
                                Address = row.Cell(4).GetString()?.Trim()
                            };

                            // Basic validation - ensure we have at least name and email
                            if (!string.IsNullOrEmpty(contact.Name) && !string.IsNullOrEmpty(contact.Email))
                            {
                                contacts.Add(contact);
                            }
                        }
                    }
                }

                // Insert contacts if we have any valid ones
                if (contacts.Any())
                {
                    try
                    {
                        await _context.Contacts.InsertManyAsync(contacts);
                        TempData["SuccessMessage"] = $"Successfully imported {contacts.Count} contacts from Excel file.";
                    }
                    catch (MongoWriteException ex)
                    {
                        // Handle duplicate key or other MongoDB write errors
                        ModelState.AddModelError("", "A database error occurred while importing contacts. Some contacts may already exist.");
                        TempData["ErrorMessage"] = "Failed to import some contacts due to database constraints.";
                        // Log the exception if you have logging configured
                        // _logger.LogError(ex, "MongoDB write error during contact import");
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "An unexpected error occurred while saving contacts.");
                        TempData["ErrorMessage"] = "Failed to save contacts to database.";
                        // Log the exception if you have logging configured
                        // _logger.LogError(ex, "Unexpected error during contact import");
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "No valid contacts found in the uploaded file. Please ensure your Excel file has Name and Email columns with data.";
                }
            }
            catch (Exception ex)
            {
                // Handle file reading errors
                ModelState.AddModelError("File", "An error occurred while processing the Excel file. Please ensure it's a valid Excel format.");
                TempData["ErrorMessage"] = $"Failed to process Excel file: {ex.Message}";
                // Log the exception if you have logging configured
                // _logger.LogError(ex, "Error processing Excel file upload");
            }

            return RedirectToAction(nameof(Index));
        }



        // GET: Contacts/Delete/5
        [Authorize(Roles = "Admin,Editor")] // Only Admin and Editor can delete
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contact = await _context.Contacts.Find(c => c.Id == id).FirstOrDefaultAsync();
            if (contact == null)
            {
                return NotFound();
            }

            return View(contact); // Show confirmation page before deleting
        }

        // POST: Contacts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Editor")] // Only Admin and Editor can delete
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                var contact = await _context.Contacts.Find(c => c.Id == id).FirstOrDefaultAsync();
                if (contact == null)
                {
                    return NotFound();
                }

                var result = await _context.Contacts.DeleteOneAsync(c => c.Id == id);
                if (result.DeletedCount == 0)
                {
                    ModelState.AddModelError("", "Failed to delete contact. It may not exist.");
                    return View(contact);
                }

                return RedirectToAction(nameof(Index));
            }
            catch (MongoException)
            {
                ModelState.AddModelError("", "An database error occurred while deleting the contact. Please try again.");
                // Optionally log the exception
                var contact = await _context.Contacts.Find(c => c.Id == id).FirstOrDefaultAsync();
                return View(contact);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
                var contact = await _context.Contacts.Find(c => c.Id == id).FirstOrDefaultAsync();
                return View(contact);
            }
        }
    }
}