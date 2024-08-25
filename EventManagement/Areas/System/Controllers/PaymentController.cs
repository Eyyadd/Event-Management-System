using EventManagement.Areas.System.View_Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EventManagement.Areas.System.Controllers
{
    [Area("System")]
    public class PaymentController : Controller
    {
        private readonly IOptions<StripeSettings> _stripeSettings;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUniteOfWork _eventRepo;

        public PaymentController(IOptions<StripeSettings> stripeSettings, UserManager<ApplicationUser> userManager, IUniteOfWork eventRepo)
        {
            _stripeSettings = stripeSettings;
            _userManager = userManager;
            _eventRepo = eventRepo;
        }

        // Action to check if the user is authenticated
        [HttpGet]
        public JsonResult IsAuthenticated()
        {
            return Json(User.Identity.IsAuthenticated);
        }

        public IActionResult CheckOut(int eventId)
        {
            // Ensure the user is authenticated before proceeding
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Register", "Account", new { area = "System" });
            }

            ViewBag.StripePublishableKey = _stripeSettings.Value.PublishableKey;
            ViewBag.EventId = eventId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessPayment(string stripeToken, int eventId)
        {
            var eventEntity = _eventRepo.EventRepository.Get(eventId);
            if (eventEntity == null)
            {
                return NotFound("Event not found");
            }

            var options = new ChargeCreateOptions
            {
                Amount = (long)(eventEntity.Price * 100), // Assuming Price is in dollars, convert to cents
                Currency = "usd",
                Description = eventEntity.Name,
                Source = stripeToken,
                ReceiptEmail = User.FindFirstValue(ClaimTypes.Email)
            };

            var service = new ChargeService();
            Charge charge = await service.CreateAsync(options);

            if (charge.Status == "succeeded")
            {
                return RedirectToAction("Book", "Ticket", new { area = "System", eventId = eventId });
            }

            return View("FailurePayment");
        }
    }
}
