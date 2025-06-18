using EmailApplication.Models;
using EmailCampaignApp.Data;
using EmailCampaignApp.Models;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using System.Net;
using System.Net.Mail;

namespace EmailCampaignApp.Controllers
{
    [Authorize]
    public class EmailController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailController> _logger;
        private readonly IBackgroundJobClient _backgroundJobClient;

        public EmailController(AppDbContext context, IConfiguration configuration, ILogger<EmailController> logger, IBackgroundJobClient backgroundJobClient)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _backgroundJobClient = backgroundJobClient ?? throw new ArgumentNullException(nameof(backgroundJobClient));
        }

        [HttpGet]
        public IActionResult SendEmail()
        {
            return View();
        }


        public async Task<IActionResult> Dashboard()
        {
            // Fetch total contacts and sent emails from the database
            var totalContacts = (int)await _context.Contacts.CountDocumentsAsync(_ => true);
            var totalSentEmails = (int)await _context.SentEmails.CountDocumentsAsync(_ => true);

            // Create a view model to pass data to the view
            var dashboardViewModel = new DashboardViewModel
            {
                TotalContacts = totalContacts,
                TotalSentEmails = totalSentEmails
            };

            return View(dashboardViewModel);
        }

        //[HttpPost]
        //public async Task<IActionResult> SendEmail(string subject, string body, List<IFormFile> attachments, string recipientType, string recipientEmail)
        //{
        //    if (string.IsNullOrEmpty(subject) || string.IsNullOrEmpty(body))
        //    {
        //        ModelState.AddModelError("", "Subject and Body are required.");
        //        return View();
        //    }

        //    List<Contact> contacts;

        //    // Fetch recipients
        //    // Check recipient type and fetch contacts accordingly
        //    if (recipientType == "specific" && !string.IsNullOrEmpty(recipientEmail))
        //    {
        //        // Fetch contacts by specific email
        //        contacts = await _context.Contacts
        //            .Find(c => c.Email == recipientEmail)
        //            .ToListAsync();
        //    }
        //    else
        //    {
        //        // Fetch all contacts if recipient type is 'all' or no specific email is provided
        //        contacts = await _context.Contacts
        //            .Find(_ => true) // Fetch all contacts
        //            .ToListAsync();
        //    }

        //    // Check if there are any contacts to send emails to
        //    var smtpSettings = _configuration.GetSection("SmtpSettings");
        //    // Ensure SMTP settings are configured
        //    var smtpClient = new SmtpClient
        //    {
        //        Host = smtpSettings["Host"],
        //        Port = smtpSettings.GetValue<int>("Port"),
        //        Credentials = new NetworkCredential(smtpSettings["Username"], smtpSettings["Password"]),
        //        EnableSsl = true,
        //        DeliveryMethod = SmtpDeliveryMethod.Network,
        //        UseDefaultCredentials = false
        //    };

        //    foreach (var contact in contacts)
        //    {
        //        if (!string.IsNullOrEmpty(contact.Email))
        //        {
        //            var mailMessage = new MailMessage
        //            {
        //                From = new MailAddress(smtpSettings["Username"], "Jooneli Inc."),
        //                Subject = subject,
        //                IsBodyHtml = true
        //            };
        //            mailMessage.To.Add(contact.Email);

        //            // Email HTML Template with Inline Images
        //            string emailBody = $@"
        //             <!DOCTYPE html>
        //            <html lang='en'>
        //            <head>
        //                <meta charset='UTF-8'>
        //                <meta name='viewport' content='width=device-width, initial-scale=1.0'>
        //                <title>Newsletter</title>
        //                <style>

        //                .newsletter-container {{
        //                    max-width: 600px;
        //                    margin: 20px auto;
        //                    background: rgb(255, 238, 0); /* Yellow background */
        //                    border: 1px solid #dddddd;
        //                    border-radius: 8px;

        //                    box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
        //                }}

        //                .header {{
        //                    text-align: center;
        //                    background: rgb(255, 255, 255);
        //                    color: black;
        //                    padding: 10px 0;
        //                    font-size: 24px;
        //                    border-radius: 8px 8px 0 0;
        //                }}

        //                .content {{

        //                    padding: 20px;
        //                    font-size: 16px;
        //                    color:rgb(0, 0, 0);
        //                    line-height: 1.6;
        //                }}

        //                .attachments {{
        //                    margin: 20px 0;
        //                    font-size: 14px;
        //                    color: #555555;
        //                }}

        //                .footer {{
        //                    text-align: right;
        //                    font-size: 18px;
        //                    color: rgb(0, 0, 0);
        //                    margin-top: 20px;
        //                    padding-top: 10px;
        //                    background-color: white; /* White background for footer */
        //                    border-top: 1px solid rgb(255, 238, 0); /* Divider with the yellow color */
        //                    padding: 20px;
        //                }}

        //                .promo-img {{
        //                    max-width: 100%;
        //                    height: auto;
        //                    margin: 10px 0;
        //                    display: block;
        //                }}

        //                </style>
        //             </head>
        //             <body>
        //             <div class='newsletter-container'>

        //             <div class='content'>
        //                <p>Dear {contact.Name},</p>
        //                {body}
        //                <br />";

        //            // ... (Handle Logo Attachment as before)
        //            var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "jooneli.png");
        //            if (System.IO.File.Exists(logoPath))
        //            {
        //                var logoStream = new MemoryStream(System.IO.File.ReadAllBytes(logoPath));
        //                var logoAttachment = new System.Net.Mail.Attachment(logoStream, "logo.png", "image/png")
        //                {
        //                    ContentId = "logo",
        //                    ContentDisposition = { Inline = true }
        //                };
        //                mailMessage.Attachments.Add(logoAttachment);
        //            }

        //            // Handling additional attachments as promotional images
        //            //if (attachments != null && attachments.Count > 0)
        //            //{
        //            //    foreach (var file in attachments)
        //            //    {
        //            //        if (file.Length > 0)
        //            //        {
        //            //            var ms = new MemoryStream();
        //            //            await file.CopyToAsync(ms);
        //            //            ms.Position = 0;

        //            //            var attachmentId = Guid.NewGuid().ToString();
        //            //            var attachment = new System.Net.Mail.Attachment(ms, file.FileName, file.ContentType)
        //            //            {
        //            //                ContentId = attachmentId,
        //            //                ContentDisposition = { Inline = true }
        //            //            };
        //            //            mailMessage.Attachments.Add(attachment);

        //            //            // Embed each image inside the email with responsive styling
        //            //            emailBody += $@"<img src='cid:{attachmentId}' alt='{file.FileName}' style='max-width: 100%; height: auto; display: block; margin: 10px 0;' />";
        //            //        }
        //            //    }
        //            //}
        //            try
        //            {
        //                if (attachments != null && attachments.Count > 0)
        //                {
        //                    foreach (var file in attachments)
        //                    {
        //                        if (file.Length > 0)
        //                        {
        //                            var ms = new MemoryStream();
        //                            await file.CopyToAsync(ms);
        //                            ms.Position = 0;
        //                            var attachment = new System.Net.Mail.Attachment(ms, file.FileName, file.ContentType);
        //                            mailMessage.Attachments.Add(attachment);
        //                        }
        //                    }
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                _logger.LogError($"Error adding attachments {contact.Email}: {ex.Message}");
        //                ModelState.AddModelError("", "Error adding attachments. Please try again.");
        //                return View();
        //            }


        //            // Append Footer
        //            emailBody += @"
        //            </div>
        //            <div class='footer'>
        //                <img src='cid:logo' alt='Jooneli Inc. Logo' style='max-width: 100%; height: auto; text-align: right;' />
        //                <p>Dhobighat-4, Lalitpur, Nepal</p>
        //                <p>Call: (977)-1-5153449</p>
        //                <p>Mob: 9801464981, 9801464982</p>
        //                <p>E-mail: sales@jooneli.com</p>
        //                <p>Website: http://www.jooneli.com</p>
        //             </div>
        //             </div>
        //             </body>
        //             </html>";

        //            mailMessage.Body = emailBody;

        //            try
        //            {
        //                await smtpClient.SendMailAsync(mailMessage);
        //                _logger.LogInformation($"Email sent successfully to: {contact.Email}");

        //                var sentEmail = new SentEmail
        //                {
        //                    RecipientEmail = contact.Email,
        //                    SentDate = DateTime.UtcNow
        //                };
        //                // Save the sent email to the database
        //                await _context.SentEmails.InsertOneAsync(sentEmail);
        //            }
        //            catch (Exception ex)
        //            {
        //                _logger.LogError($"Error sending email to {contact.Email}: {ex.Message}");
        //            }
        //        }
        //    }

        //    return RedirectToAction("Index", "Contacts");
        //}


        [HttpPost]
        public async Task<IActionResult> SendEmail(string subject, string body, List<IFormFile> attachments, string recipientType, string recipientEmail)
        {
            if (string.IsNullOrEmpty(subject) || string.IsNullOrEmpty(body))
            {
                ModelState.AddModelError("", "Subject and Body are required.");
                return View();
            }

            List<Contact> contacts;

            // Fetch recipients
            if (recipientType == "specific" && !string.IsNullOrEmpty(recipientEmail))
            {
                contacts = await _context.Contacts
                    .Find(c => c.Email == recipientEmail)
                    .ToListAsync();
            }
            else
            {
                contacts = await _context.Contacts
                    .Find(_ => true)
                    .ToListAsync();
            }

            var smtpSettings = _configuration.GetSection("SmtpSettings");
            var smtpClient = new SmtpClient
            {
                Host = smtpSettings["Host"],
                Port = smtpSettings.GetValue<int>("Port"),
                Credentials = new NetworkCredential(smtpSettings["Username"], smtpSettings["Password"]),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false
            };

            foreach (var contact in contacts)
            {
                if (!string.IsNullOrEmpty(contact.Email))
                {
                    using (var mailMessage = new MailMessage
                    {
                        From = new MailAddress(smtpSettings["Username"], "Jooneli Inc."),
                        Subject = subject,
                        IsBodyHtml = true
                    })
                    {
                        mailMessage.To.Add(contact.Email);

                        // Email HTML Template with Inline Images
                        string emailBody = $@"
                 <!DOCTYPE html>
                <html lang='en'>
                <head>
                    <meta charset='UTF-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <title>Newsletter</title>
                    <style>
                        .newsletter-container {{
                            max-width: 600px;
                            margin: 20px auto;
                            background: rgb(255, 238, 0);
                            border: 1px solid #dddddd;
                            border-radius: 8px;
                            box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
                        }}
                        .header {{
                            text-align: center;
                            background: rgb(255, 255, 255);
                            color: black;
                            padding: 10px 0;
                            font-size: 24px;
                            border-radius: 8px 8px 0 0;
                        }}
                        .content {{
                            padding: 20px;
                            font-size: 16px;
                            color:rgb(0, 0, 0);
                            line-height: 1.6;
                        }}
                        .attachments {{
                            margin: 20px 0;
                            font-size: 14px;
                            color: #555555;
                        }}
                        .footer {{
                            text-align: right;
                            font-size: 18px;
                            color: rgb(0, 0, 0);
                            margin-top: 20px;
                            padding-top: 10px;
                            background-color: white;
                            border-top: 1px solid rgb(255, 238, 0);
                            padding: 20px;
                        }}
                        .promo-img {{
                            max-width: 100%;
                            height: auto;
                            margin: 10px 0;
                            display: block;
                        }}
                    </style>
                 </head>
                 <body>
                 <div class='newsletter-container'>
                 <div class='content'>
                    <p>Dear {contact.Name},</p>
                    {body}
                    <br />";

                        // Add logo as inline image 
                        var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "image", "jooneli.png");
                        if (System.IO.File.Exists(logoPath))
                        {
                            var logoStream = new MemoryStream(System.IO.File.ReadAllBytes(logoPath));
                            var logoAttachment = new System.Net.Mail.Attachment(logoStream, "logo.png", "image/png")
                            {
                                ContentId = "logo",
                                ContentDisposition = { Inline = true }
                            };
                            mailMessage.Attachments.Add(logoAttachment);
                        }

                        // Add file attachments (limit to 10MB per file)
                        if (attachments != null && attachments.Count > 0)
                        {
                            foreach (var file in attachments)
                            {
                                if (file.Length > 0 && file.Length < 10 * 1024 * 1024)
                                {
                                    var ms = new MemoryStream();
                                    await file.CopyToAsync(ms);
                                    ms.Position = 0;
                                    var attachment = new System.Net.Mail.Attachment(ms, file.FileName, file.ContentType);
                                    mailMessage.Attachments.Add(attachment);
                                }
                            }
                        }

                        // Append Footer
                        emailBody += @"
                </div>
                <div class='footer'>
                    <img src='cid:logo' alt='Jooneli Inc. Logo' style='max-width: 100%; height: auto; text-align: right;' />
                    <p>Dhobighat-4, Lalitpur, Nepal</p>
                    <p>Call: (977)-1-5153449</p>
                    <p>Mob: 9801464981, 9801464982</p>
                    <p>E-mail: sales@jooneli.com</p>
                    <p>Website: http://www.jooneli.com</p>
                 </div>
                 </div>
                 </body>
                 </html>";

                        mailMessage.Body = emailBody /* your emailBody HTML here */;

                        try
                        {
                            await smtpClient.SendMailAsync(mailMessage);
                            _logger.LogInformation($"Email sent successfully to: {contact.Email}");
                            // Save the sent email to the database

                            var sentEmail = new SentEmail
                            {
                                RecipientEmail = contact.Email,
                                SentDate = DateTime.UtcNow
                            };
                            await _context.SentEmails.InsertOneAsync(sentEmail);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Error sending email to {contact.Email}: {ex.Message}");
                        }
                    }
                }
            }

            return RedirectToAction("Index", "Contacts");
        }



        //[HttpPost]
        //public async Task<IActionResult> SaveTemplate(IFormCollection formData)
        //{
        //    try
        //    {
        //        string name = formData["name"];
        //        string description = formData["description"];
        //        string templateHtml = formData["template"]; // Full HTML template
        //        string issueDate = formData["issueDate"];
        //        string expireDate = formData["expireDate"];
        //        string imageUrl = formData["imageUrl"];

        //        if (string.IsNullOrEmpty(templateHtml))
        //        {
        //            return BadRequest(new { message = "Template content is required." });
        //        }

        //        var campaign = new Campaign
        //        {
        //            Name = name,
        //            Description = description,
        //            TemplateContent = templateHtml, // Store full HTML
        //            IssueDate = DateTime.Parse(issueDate),
        //            ExpireDate = DateTime.Parse(expireDate),
        //            ImageUrl = imageUrl
        //        };

        //        await _context.Campaigns.InsertOneAsync(campaign);

        //        return Ok(new { message = "Template saved successfully!" });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error saving template");
        //        return StatusCode(500, new { message = "An error occurred while saving the template." });
        //    }
        //}

        [HttpPost]
        public async Task<IActionResult> SaveTemplate(
        [FromForm] string name,
        [FromForm] string description,
        [FromForm] string template,
        [FromForm] string issueDate,
        [FromForm] string expireDate,
        [FromForm] string imageUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(template))
                {
                    return BadRequest(new { message = "Template content is required." });
                }

                var campaign = new Campaign
                {
                    Name = name,
                    Description = description,
                    TemplateContent = template,
                    IssueDate = DateTime.Parse(issueDate),
                    ExpireDate = DateTime.Parse(expireDate),
                    ImageUrl = imageUrl
                };

                await _context.Campaigns.InsertOneAsync(campaign);

                return Ok(new { message = "Template saved successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving template");
                return StatusCode(500, new { message = "An error occurred while saving the template." });
            }
        }


        [HttpGet]
        public async Task<IActionResult> UpdateTemplate(string id)
        {
            // Fetch the existing campaign template from the database using the provided id
            var campaignTemplate = await _context.Campaigns.Find(c => c.Id == id).FirstOrDefaultAsync();
            if (campaignTemplate == null)
            {
                return NotFound();  // Return 404 if the campaign template is not found
            }
            return View("UpdateTemplate", campaignTemplate); // Specify the correct view path
        }


        [HttpPost]
        public async Task<IActionResult> UpdateTemplate(IFormCollection formData)
        {
            string templateId = formData["templateId"]; // Assuming templateId is passed in the form data
            string name = formData["name"];
            string description = formData["description"];
            string templateHtml = formData["template"];
            string issueDate = formData["issueDate"];
            string expireDate = formData["expireDate"];
            string imageUrl = formData["imageUrl"];


            // Find the existing template
            var existingCampaign = await _context.Campaigns.Find(c => c.Id == templateId).FirstOrDefaultAsync();
            if (existingCampaign == null)
            {
                return NotFound(new { message = "Template not found." });
            }
            // Update the template data
            existingCampaign.Name = name;
            existingCampaign.Description = description;
            existingCampaign.TemplateContent = templateHtml;
            existingCampaign.IssueDate = DateTime.Parse(issueDate);
            existingCampaign.ExpireDate = DateTime.Parse(expireDate);
            existingCampaign.ImageUrl = imageUrl;

            await _context.Campaigns.ReplaceOneAsync(c => c.Id == existingCampaign.Id, existingCampaign);

            return Ok(new { message = "Template updated successfully!" });
        }




        //// Optional: Method to handle image upload
        //[HttpPost]
        //public async Task<IActionResult> UploadImage()
        //{
        //    try
        //    {
        //        var file = Request.Form.Files[0]; // Assuming you're only uploading one file
        //        if (file.Length > 0)
        //        {
        //            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", file.FileName);
        //            using (var stream = new FileStream(filePath, FileMode.Create))
        //            {
        //                await file.CopyToAsync(stream);
        //            }

        //            // Return the URL of the uploaded file
        //            return Ok(new { fileName = $"/uploads/{file.FileName}" });
        //        }
        //        else
        //        {
        //            return BadRequest("No file uploaded.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Internal server error: {ex.Message}");
        //    }
        //}

        //public async Task<IActionResult> ViewTemplate(string id)
        //{
        //    var campaign = await _context.Campaigns.Find(c => c.Id == id).FirstOrDefaultAsync();
        //    if (campaign == null)
        //    {
        //        return NotFound("Campaign not found!");
        //    }
        //    // Return the template content as a view
        //    //return View("ViewTemplate", campaign);

        //    // Use the view from the Campaigns folder
        //    return View("~/Views/Campaigns/ViewTemplate.cshtml", campaign);

        //}



        public async Task<IActionResult> ViewTemplate(string id)
        {
            if (string.IsNullOrEmpty(id) || !ObjectId.TryParse(id, out _))
            {
                return BadRequest("Invalid campaign ID format.");
            }

            var campaign = await _context.Campaigns.Find(c => c.Id == id).FirstOrDefaultAsync();
            if (campaign == null)
            {
                return NotFound("Campaign not found!");
            }

            return View("~/Views/Campaigns/ViewTemplate.cshtml", campaign);
        }


        [HttpPost]
        public async Task<IActionResult> SendNow([FromBody] SendNowRequest request)
        {
            //if (request == null)
            //{
            //    return BadRequest("Request is null.");
            //}
            // Validate the request
            if (request == null || request.EmailIds == null || string.IsNullOrEmpty(request.Id))
            {
                _logger.LogWarning("Invalid request received.");
                return BadRequest("Invalid request.");
            }

            if (_context?.Campaigns == null)
            {
                _logger.LogError("Campaign collection is null.");
                return StatusCode(500, "Campaign collection is not available.");
            }


            // Fetch the campaign by ID
            // validate the request
            var campaign = await _context.Campaigns
                .Find(c => c.Id == request.Id)
                .FirstOrDefaultAsync(); // Use FirstOrDefaultAsync to fetch a single Campaign object

            // Check if the campaign exists
            if (campaign == null)
            {
                _logger.LogWarning("Campaign not found with ID: {Id}", request.Id);
                return NotFound("Campaign not found!");
            }

            // Fetch recipient emails by their IDs
            var recipients = await _context.Contacts
                .Find(c => request.EmailIds.Contains(c.Id))
                .Project(c => c.Email)
                .ToListAsync();

            if (!recipients.Any())
            {
                return BadRequest("No valid contacts found.");
            }
            try
            {
                // Send the campaign to the recipients
                bool emailSent = await SendCampaign(campaign.Id, recipients);

                // If emails were sent successfully, update the campaign status
                if (emailSent)
                {
                    campaign.IsSent = true;
                    await _context.Campaigns.ReplaceOneAsync(c => c.Id == campaign.Id, campaign);
                    return Ok("Campaign sent successfully!");
                }

                _logger.LogWarning("SendCampaign returned false.");
                return BadRequest("Failed to send emails. Check logs for details.");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while sending campaign.");
                return StatusCode(500, "Internal server error.");
            }


            //return BadRequest("Error sending campaign.");
        }


        // Get Scheduled Emails (returns a view model)
        public async Task<IActionResult> ScheduledEmails()
        {
            var model = await _context.ScheduledEmails
                .Find(_ => true) // Fetch all scheduled emails
                .ToListAsync();

            return View(model);
        }

        public async Task<IActionResult> ScheduledEmailsByDate(string date)
        {
            if (string.IsNullOrEmpty(date))
            {
                return BadRequest("Date is required.");
            }

            // Fetch all scheduled emails from the database
            var emails = await _context.ScheduledEmails
                .Find(_ => true) // Fetch all scheduled emails
                .ToListAsync(); // Convert to list asynchronously

            var filteredEmails = emails
                .AsEnumerable() // Switch to client-side evaluation
                .Where(e => e.LocalScheduledDate.ToString("g") == date)
                .ToList(); // Convert to list synchronously

            var model = new ScheduledEmailsByDateViewModel
            {
                Date = date,
                Emails = filteredEmails.Select(e => new EmailStatusViewModel
                {
                    RecipientEmail = e.RecipientEmail,
                    Status = "Pending" // Assuming status is pending for scheduled emails
                }).ToList()
            };

            return View(model);
        }

        public async Task<bool> SendScheduledEmail(string campaignName, DateTime scheduledDate)
        {
            _logger.LogInformation("SendScheduleEmail called for campaign: {CampaignName} at {ScheduleDate}", campaignName, scheduledDate);

            var scheduledEmails = await _context.ScheduledEmails
                .Find(e => e.CampaignName == campaignName && e.ScheduledDate <= scheduledDate)
                .ToListAsync();
            _logger.LogInformation("Found {Count} scheduled emails to send.", scheduledEmails.Count);

            foreach (var scheduledEmail in scheduledEmails)
            {
                // Fetch the campaign using FirstOrDefaultAsync instead of FindAsync
                var campaign = await _context.Campaigns
                    .Find(c => c.Name == campaignName)
                    .FirstOrDefaultAsync(); // Correctly fetch a single Campaign object

                if (campaign == null)
                {
                    _logger.LogError($"Campaign with name {campaignName} not found.");
                    continue;
                }

                var result = await SendCampaign(campaign.Id, new List<string> { scheduledEmail.RecipientEmail });
                if (result)
                {
                    await _context.ScheduledEmails.DeleteOneAsync(e => e.Id == scheduledEmail.Id);
                }
            }

            return true;
        }


        [HttpPost]
        public async Task<IActionResult> ScheduleSend([FromBody] ScheduleSendRequest request)
        {
            //if (request == null)
            //{
            //    return BadRequest("Request is null. ");
            //}

            if (request.EmailIds == null || !request.EmailIds.Any())
            {
                return BadRequest("No email recipients selected.");
            }

            if (string.IsNullOrEmpty(request.Id))
            {
                return BadRequest("Campaign ID is required.");
            }

            if (_context?.Contacts == null)
            {
                return StatusCode(500, "Contact collection is not available.");
            }

            if (_context?.ScheduledEmails == null)
            {
                return StatusCode(500, "ScheduledEmails collection is not available.");
            }

            if (request.ScheduleDate == null || request.ScheduleDate <= DateTime.UtcNow)
            {
                return BadRequest("Invalid schedule date. Please select a future date.");
            }

            if (_backgroundJobClient == null)
            {
                return StatusCode(500, "Background job client is not available.");
            }

            if (request.ScheduleDate == default || request.ScheduleDate <= DateTime.UtcNow)
            {
                return BadRequest("Invalid schedule date. Please select a future date.");
            }

            // Fetch the campaign using FirstOrDefaultAsync instead of FindAsync
            var campaign = await _context.Campaigns
                .Find(c => c.Id == request.Id)
                .FirstOrDefaultAsync(); // Use FirstOrDefaultAsync to fetch a single Campaign object

            // Check if the campaign exists
            if (campaign == null)
            {
                return NotFound("Campaign not found!");
            }
            if (string.IsNullOrEmpty(campaign.Name))
            {
                return BadRequest("Campaign name is missing.");
            }

            // convert to MongoDB-compatible DataTable
            // Fetch recipient emails by their IDs
            var recipients = await _context.Contacts
                .Find(c => request.EmailIds.Contains(c.Id))
                .Project(c => c.Email)
                .ToListAsync();

            if (recipients == null || !recipients.Any())
            {
                return BadRequest("No valid contacts found.");
            }

            // Create scheduled emails in the database
            var scheduledEmails = recipients.Select(email => new ScheduledEmail
            {
                CampaignName = campaign.Name,
                RecipientEmail = email,
                ScheduledDate = request.ScheduleDate
            }).ToList();

            if (scheduledEmails.Count > 0)
            {
                await _context.ScheduledEmails.InsertManyAsync(scheduledEmails);
            }

            try
            {
                // Schedule the background job to send emails at the specified date
                _backgroundJobClient.Schedule(() => SendScheduledEmail(campaign.Name, request.ScheduleDate), request.ScheduleDate);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error scheduling the job: {ex.Message}");
            }

            return Ok($"Campaign scheduled for {request.ScheduleDate} successfully.");
        }


        public async Task<bool> SendCampaign(string campaignId, List<string> recipients)
        {
            if (_context == null)
            {
                _logger.LogError("_context is null. Ensure dependency injection is working.");
                throw new InvalidOperationException("_context is not initialized.");
            }

            if (recipients == null || recipients.Count == 0)
            {
                _logger.LogError("Recipients list is null or empty for campaignId: {CampaignId}", campaignId);
                return false;
            }

            if (string.IsNullOrEmpty(campaignId))
            {
                _logger.LogError("Campaign ID is null or empty.");
                return false;
            }

            _logger.LogInformation("Fetching campaign with ID {CampaignId}", campaignId);
            // Fetch the campaign from the database
            var campaign = await _context.Campaigns
                .Find(c => c.Id == campaignId)
                .FirstOrDefaultAsync();

            if (campaign == null)
            {
                _logger.LogError("Campaign not found with ID: {CampaignId}", campaignId);
                return false;
            }

            if (_configuration == null)
            {
                _logger.LogError("_configuration is null. Ensure it is correctly injected.");
                throw new InvalidOperationException("_configuration is not initialized.");
            }

            _logger.LogInformation("Fetching SMTP settings from configuration");
            var smtpSettings = _configuration.GetSection("SmtpSettings");
            if (smtpSettings == null)
            {
                _logger.LogError("SMTP settings not found.");
                return false;
            }

            var host = smtpSettings["Host"];
            var port = smtpSettings.GetValue<int>("Port");
            var username = smtpSettings["Username"];
            var password = smtpSettings["Password"];

            if (string.IsNullOrEmpty(host) || port == 0 || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                _logger.LogError("Invalid SMTP configuration. Host: {Host}, Port: {Port}, Username: {Username}", host, port, username);
                return false;
            }

            _logger.LogInformation("Creating SMTP client");
            using var smtpClient = new SmtpClient
            {
                Host = host,
                Port = port,
                Credentials = new NetworkCredential(username, password),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false
            };

            foreach (var email in recipients)
            {
                _logger.LogInformation("Sending email to {Email}", email);

                string emailBody = $@"
                <!DOCTYPE html>
                <html lang='en'>
                <head>
                    <meta charset='UTF-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <title>{campaign.Name}</title>
                 </head>
                 <body>
                    <h1>{campaign.Name}</h1>
                    <p>{campaign.TemplateContent}</p>
                 </body>
                 </html>";

                using var message = new MailMessage
                {
                    From = new MailAddress(username, "Jooneli Inc."),
                    Subject = campaign.Name,
                    IsBodyHtml = true,
                    Body = emailBody
                };

                message.To.Add(email);

                // Add attachments if any
                var sentEmail = new SentEmail
                {
                    RecipientEmail = email,
                    SentDate = DateTime.UtcNow,
                    CampaignId = campaignId,
                    Status = "Sent",
                    IsOpened = false
                };

                try
                {
                    await smtpClient.SendMailAsync(message);
                    _logger.LogInformation("Email sent successfully to {Email}", email);
                }
                catch (Exception ex)
                {
                    sentEmail.Status = "Failed";
                    _logger.LogError(ex, "Error sending email to {Email}: {Message}", email, ex.Message);
                }

                await _context.SentEmails.InsertOneAsync(sentEmail);
            }

            return true;
        }


        // Get Total Emails Sent (returns a view model)
        public async Task<IActionResult> TotalEmailsSent()
        {
            var emails = await _context.SentEmails
                  .Find(_ => true) // Fetch all sent emails
                  .ToListAsync();
            var model = new TotalEmailsSentViewModel
            {
                Emails = emails.Select(e => new Email
                {
                    SentDate = e.SentDate,
                    RecipientEmail = e.RecipientEmail,
                    Status = e.Status,
                }).ToList()
            };

            return View(model);
        }

        public async Task<IActionResult> EmailsByDate(string date)
        {
            if (string.IsNullOrEmpty(date))
            {
                return BadRequest("Date is required.");
            }

            // Fetch all sent emails from the database
            var emails = await _context.SentEmails
                .Find(_ => true) // Fetch all sent emails
                .ToListAsync(); // Convert to list asynchronously

            var filteredEmails = emails
                .AsEnumerable() // Switch to client-side evaluation
                .Where(e => e.LocalSentDate.ToString("g") == date)
                .ToList(); // Convert to list synchronously

            var model = new EmailsByDateViewModel
            {
                Date = date,
                Emails = filteredEmails
            };

            return View(model);
        }

        // Get Emails for a Campaign (returns JSON)
        public async Task<IActionResult> GetEmailsForCampaign(string id)
        {
            // Validate the campaign ID
            var campaign = await _context.Campaigns
                .Find(c => c.Id == id)
                .FirstOrDefaultAsync();

            // Check if the campaign exists
            if (campaign == null)
            {
                return NotFound(); // Return 404 if the campaign is not found
            }

            // Fetch all sent emails for the specified campaign
            //var emails = await _context.SentEmails
            //    .Find(e => e.CampaignId == id)
            //    .ToListAsync();

            var emails = await _context.Contacts.Find(_ => true).ToListAsync();

            // Return the emails as JSON
            // Project to a simple DTO or anonymous type for JSON output
            // Select only the necessary fields for the response

            //var result = emails.Select(e => new
            //{
            //    e.Id,
            //    e.RecipientEmail,
            //    e.SentDate,
            //    e.Status,
            //    e.IsOpened
            //});

            var result = emails.Select(e => new
            {
                //e.Id,
                //e.RecipientEmail
                id = e.Id,
                address = e.Email
            });

            return Json(result);
        }


        // Get Campaigns (returns a view model)
        public async Task<IActionResult> Campaigns()
        {
            try
            {
                var campaigns = await _context.Campaigns
                                .Find(c => true) // Fetch all campaigns
                                .ToListAsync();
                return View(campaigns);
            }
            catch (Exception e)
            {
                Response.StatusCode = StatusCodes.Status500InternalServerError;
                return RedirectToAction("Dashboard");
            }
        }


        [HttpPost]
        public async Task<IActionResult> DeleteCampaign(string id)    //Delete Campaign
        {
            // Find the campaign by ID
            var campaign = await _context.Campaigns
                .Find(c => c.Id == id)
                .FirstOrDefaultAsync();



            if (campaign == null)
            {
                return NotFound(); // Return a 404 if campaign not found
            }

            try
            {
                if (campaign.Attachments != null)
                {
                    // 1. Delete campaign attachments from the server
                    foreach (var attachment in campaign.Attachments)
                    {
                        var filePath = attachment.FilePath;
                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath); // Delete the file from the server
                            _logger.LogInformation($"Deleted attachment file: {filePath}");
                        }
                    }
                }

                // 2. Delete images referenced in template content
                if (!string.IsNullOrEmpty(campaign.TemplateContent))
                {
                    await DeleteImagesFromTemplateContent(campaign.TemplateContent);
                }

                // 3. Delete campaign image if it exists
                if (!string.IsNullOrEmpty(campaign.ImageUrl))
                {
                    await DeleteImageFromUrl(campaign.ImageUrl);
                }

                // 4. Delete all sent emails related to this campaign
                var sentEmailsDeleteResult = await _context.SentEmails.DeleteManyAsync(e => e.CampaignId == id);
                _logger.LogInformation($"Deleted {sentEmailsDeleteResult.DeletedCount} sent emails for campaign {id}");

                // 5. Delete all scheduled emails related to this campaign
                var scheduledEmailsDeleteResult = await _context.ScheduledEmails.DeleteManyAsync(e => e.CampaignName == campaign.Name);
                _logger.LogInformation($"Deleted {scheduledEmailsDeleteResult.DeletedCount} scheduled emails for campaign {campaign.Name}");

                // Remove the campaign and its attachments from the database
                await _context.Campaigns.DeleteOneAsync(c => c.Id == campaign.Id);

                TempData["SuccessMessage"] = "Campaign and all related data deleted successfully!";
                _logger.LogInformation($"Campaign {id} and all related data deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while deleting campaign {id}");
                TempData["ErrorMessage"] = "An error occurred while deleting the campaign.";
            }

            return RedirectToAction("Campaigns"); // Redirect back to the campaigns list
        }
        /// <summary>
        /// Deletes images referenced in HTML template content
        /// </summary>
        /// <param name="htmlContent">HTML content containing image references</param>
        private async Task DeleteImagesFromTemplateContent(string htmlContent)
        {
            try
            {
                // Extract image URLs from HTML content using regex
                var imgRegex = new System.Text.RegularExpressions.Regex(
                    @"<img[^>]+src=[""']([^""']+)[""'][^>]*>",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                var matches = imgRegex.Matches(htmlContent);

                foreach (System.Text.RegularExpressions.Match match in matches)
                {
                    var imgUrl = match.Groups[1].Value;
                    await DeleteImageFromUrl(imgUrl);
                }

                // Also check for CSS background images
                var cssRegex = new System.Text.RegularExpressions.Regex(
                    @"background-image:\s*url\([""']?([^""')]+)[""']?\)",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                var cssMatches = cssRegex.Matches(htmlContent);

                foreach (System.Text.RegularExpressions.Match match in cssMatches)
                {
                    var imgUrl = match.Groups[1].Value;
                    await DeleteImageFromUrl(imgUrl);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting images from template content");
            }
        }

        /// <summary>
        /// Deletes an image file from URL if it's a local upload
        /// </summary>
        /// <param name="imageUrl">Image URL to delete</param>
        private async Task DeleteImageFromUrl(string imageUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(imageUrl))
                    return;

                // Check if it's a local upload (contains /uploads/)
                if (imageUrl.Contains("/uploads/"))
                {
                    // Extract filename from URL
                    var uri = new Uri(imageUrl, UriKind.RelativeOrAbsolute);
                    var fileName = Path.GetFileName(uri.LocalPath);

                    // Construct full file path
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    // Delete the file if it exists
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                        _logger.LogInformation($"Deleted image file: {filePath}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting image from URL: {imageUrl}");
            }
        }

        //[HttpPost("/upload")]
        //public async Task<IActionResult> UploadImage(IFormFile upload)
        //{
        //    try
        //    {

        //        if (upload == null || upload.Length == 0)
        //            return BadRequest("No file uploaded.");

        //        // Save the file to a folder (e.g., wwwroot/uploads)
        //        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        //        if (!Directory.Exists(uploadsFolder))
        //            Directory.CreateDirectory(uploadsFolder);

        //        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(upload.FileName);
        //        var filePath = Path.Combine(uploadsFolder, fileName);

        //        using (var stream = new FileStream(filePath, FileMode.Create))
        //        {
        //            await upload.CopyToAsync(stream);
        //        }

        //        // Return the URL of the uploaded image
        //        var fileUrl = $"{Request.Scheme}://{Request.Host}/uploads/{fileName}";
        //        //return Json(new { uploaded = true, url = fileUrl });


        //        return Json(new
        //        {
        //            uploaded = 1,
        //            fileName = fileName,
        //            url = fileUrl
        //        });

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("Error uploading image" + ex.Message);
        //        return StatusCode(500, new { uploaded = 0, error = new { message = "Image upload failed." } });
        //    }
        //}


        [HttpPost("/upload")]
        public async Task<IActionResult> UploadImage(IFormFile upload)
        {
            try
            {
                if (upload == null || upload.Length == 0)
                    return BadRequest("No file uploaded.");

                // Validate file type
                var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/bmp" };
                if (!allowedTypes.Contains(upload.ContentType.ToLower()))
                    return BadRequest("Invalid file type. Only image files are allowed.");

                // Create uploads folder
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                // Generate filename with .webp extension
                var fileName = Guid.NewGuid().ToString() + ".webp";
                var filePath = Path.Combine(uploadsFolder, fileName);

                // Convert image to WebP format
                using (var inputStream = upload.OpenReadStream())
                using (var image = await Image.LoadAsync(inputStream))
                {
                    // Configure WebP encoder options
                    var encoder = new WebpEncoder()
                    {
                        Quality = 80, // Adjust quality (0-100)
                        Method = WebpEncodingMethod.BestQuality,
                        TransparentColorMode = WebpTransparentColorMode.Preserve
                    };

                    // Save as WebP
                    await image.SaveAsync(filePath, encoder);
                }

                // Return the URL of the uploaded image
                var fileUrl = $"{Request.Scheme}://{Request.Host}/uploads/{fileName}";

                return Json(new
                {
                    uploaded = 1,
                    fileName = fileName,
                    url = fileUrl
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error uploading image: " + ex.Message);
                return StatusCode(500, new { uploaded = 0, error = new { message = "Image upload failed." } });
            }
        }


        public IActionResult CreateCampaign()
        {
            return View();
        }

        // POST: Save New Campaign
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCampaign(Campaign model)
        {
            if (ModelState.IsValid)
            {
                await _context.Campaigns.InsertOneAsync(model);

                return RedirectToAction("ChooseTemplate", new { id = model.Id }); // Redirect to template selection
            }
            return View(model);
        }

        // GET: Template Selection Page
        public async Task<IActionResult> ChooseTemplate(string id)
        {
            // Fetch the campaign by ID to ensure it exists
            var campaign = await _context.Campaigns
                .Find(c => c.Id == id)
                .FirstOrDefaultAsync();
            // Check if the campaign exists
            if (campaign == null)
            {
                return NotFound("Campaign not found.");
            }
            return View(campaign);
        }


    }

    //public class SendNowRequest
    //{
    //    public string Id { get; set; }
    //    public List<string> EmailIds { get; set; }
    //}

    //public class ScheduleSendRequest
    //{
    //    public string Id { get; set; }
    //    public List<string> EmailIds { get; set; }
    //    public DateTime ScheduleDate { get; set; }
    //}


}
