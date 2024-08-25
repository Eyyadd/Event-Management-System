using EventManagement.Areas.System.View_Models;

namespace EventManagement.Areas.System.Controllers
{
    [Area("System")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public UserController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<IActionResult> MyProfile(string Email)
        {
            UserViewModel userViewModel = new UserViewModel();
            var user=await _userManager.FindByEmailAsync(Email);
            if (user is not null)
            {
                userViewModel.UserName = user.UserName!;
                userViewModel.Address = user.Address;
                userViewModel.Age = user.Age;
                userViewModel.Email = user.Email!;

                return View("MyProfile", userViewModel);
            }
            return View("MyProfile");
        }

        //TODO Update User Info
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(UserViewModel userViewModel)
        {
            return View("Update", userViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveUpdate(UserViewModel userViewModel) {
            var oldUser= await _userManager.FindByEmailAsync(userViewModel.Email);

            if (ModelState.IsValid)
            {
                if (oldUser is not null)
                {
                    oldUser.UserName = userViewModel.UserName;
                    oldUser.Address = userViewModel.Address;
                    oldUser.Email = userViewModel.Email;
                    oldUser.Age = userViewModel.Age;
                    IdentityResult result = await _userManager.UpdateAsync(oldUser);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("MyProfile", "User", new {area="System",Email=userViewModel.Email});
                    }
                    foreach (var Erorr in result.Errors)
                    {
                        ModelState.AddModelError(Erorr.Code, Erorr.Description);
                    }
                    return View("Update", userViewModel);
                }
                return NotFound("User Not Found");
            }
            return View("Update",userViewModel);
        }

        //TODO Change Password
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
                                return RedirectToAction("MyProfile", "User", new { Email=EmailUser,area = "System" });
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
