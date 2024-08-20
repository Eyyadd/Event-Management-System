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
                userViewModel.UserName = user.UserName;
                userViewModel.Address = user.Address;
                userViewModel.Age = user.Age;
                userViewModel.Email = user.Email;

                return View("MyProfile", userViewModel);
            }
            return View("MyProfile");
        }

        //TODO Update User Info
        //[HttpPost]
        //public IActionResult Update(UserViewModel userViewModel)
        //{
        //    return View("Update",userViewModel);
        //}

        //TODO Change Password
        //[HttpPost]
        //public IActionResult ChangPassword(UserViewModel userViewModel)
        //{
        //    return View("Update",userViewModel);
        //}
    }
}
