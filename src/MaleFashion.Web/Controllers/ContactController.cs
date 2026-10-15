using System.Net;
using MaleFashion.Application.Contracts.Services;
using MaleFashion.Web.ViewModels.Contact;
using Microsoft.AspNetCore.Mvc;

namespace MaleFashion.Web.Controllers;

public class ContactController : Controller
{
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ContactController> _logger;

    public ContactController(
        IEmailService emailService,
        IConfiguration configuration,
        ILogger<ContactController> logger)
    {
        _emailService = emailService;
        _configuration = configuration;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(new ContactViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(ContactViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // প্রাপকের ইমেইল: appsettings-এ "Contact:RecipientEmail", না থাকলে "Email:FromEmail"
        var recipient = _configuration["Contact:RecipientEmail"]
                        ?? _configuration["Email:FromEmail"];

        if (string.IsNullOrWhiteSpace(recipient))
        {
            _logger.LogError("Contact form recipient is not configured (Contact:RecipientEmail).");
            ModelState.AddModelError(string.Empty, "The contact form is temporarily unavailable. Please try again later.");
            return View(model);
        }

        var htmlMessage = $"""
            <h3>New message from the MaleFashion contact form</h3>
            <p><strong>Name:</strong> {WebUtility.HtmlEncode(model.Name)}</p>
            <p><strong>Email:</strong> {WebUtility.HtmlEncode(model.Email)}</p>
            <p><strong>Message:</strong></p>
            <p>{WebUtility.HtmlEncode(model.Message).Replace("\n", "<br />")}</p>
            """;

        try
        {
            await _emailService.SendEmailAsync(
                recipient,
                $"MaleFashion contact: {model.Name}",
                htmlMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send contact form email.");
            ModelState.AddModelError(string.Empty, "We could not send your message right now. Please try again later.");
            return View(model);
        }

        TempData["ContactSuccess"] = "Thank you! Your message has been sent. We will get back to you soon.";
        return RedirectToAction(nameof(Index));
    }
}
