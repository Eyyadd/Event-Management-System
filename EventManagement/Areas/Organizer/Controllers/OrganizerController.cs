using EventManagement.Areas.System.View_Models;
using Microsoft.AspNetCore.Mvc;

namespace EventManagement.Areas.Organizer.Controllers
{
    [Area("Organizer")]
    [Authorize(Roles =Role.OrganizerRole)]
    public class OrganizerController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public OrganizerController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> MyProfile(string Email)
        {
            UserViewModel OrganizerViewModel = new UserViewModel();
            var user = await _userManager.FindByEmailAsync(Email);
            if (user is not null)
            {
                OrganizerViewModel.UserName = user.UserName!;
                OrganizerViewModel.Address = user.Address;
                OrganizerViewModel.Age = user.Age;
                OrganizerViewModel.Email = user.Email!;

                return View("MyProfile", OrganizerViewModel);
            }
            return View("MyProfile");
        }

        [HttpGet]
        public IActionResult ChangPassword()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            var EmailUser = User.FindFirstValue(ClaimTypes.Email);
            if (ModelState.IsValid)
            {
                if (EmailUser is not null)
                {
                    var currentUser = await _userManager.FindByEmailAsync(EmailUser);
                    if (currentUser is not null)
                    {
                        var checkOldPasswrod = await _userManager.CheckPasswordAsync(currentUser, model.OldPassword);
                        if (checkOldPasswrod)
                        {
                            var result = await _userManager.ChangePasswordAsync(currentUser, model.OldPassword, model.NewPassword);
                            if (result.Succeeded)
                            {
                                //TODO Model Changed Successfuly
                                return RedirectToAction("MyProfile", "Organizer", new { Email = EmailUser, area = "Organizer" });
                            }
                            foreach (var Error in result.Errors)
                            {
                                ModelState.AddModelError(Error.Code, Error.Description);
                            }
                            return RedirectToAction("ChangePassword", ModelState);
                        }
                        return RedirectToAction("ChangePassword", model);
                    }
                }
            }
            return View(model);
        }
    }
}
