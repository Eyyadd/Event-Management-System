namespace EventManagement.Areas.Events.Controllers
{
    [Area("System")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public AccountController(UserManager<ApplicationUser> userManager,
                                  SignInManager<ApplicationUser> signInManager
                                )
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterUserViewModel registerUser)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser mappingUser = new ApplicationUser();
                mappingUser.UserName = registerUser.UserName;
                mappingUser.Email = registerUser.Email;
                mappingUser.PhoneNumber = registerUser.Phone;
                mappingUser.Address = registerUser.Address;
                mappingUser.Age = registerUser.Age;


                IdentityResult user = await _userManager.CreateAsync(mappingUser, registerUser.Password);

                if (user.Succeeded)
                {
                    await _signInManager.SignInAsync(mappingUser, false);
                    return RedirectToAction("Index", "Event", "System");
                }

                foreach (var Error in user.Errors)
                {
                    ModelState.AddModelError(Error.Code, Error.Description);
                }

            }
            return View(registerUser);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginUserViewModel loginUser)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await _userManager.FindByNameAsync(loginUser.UserName);
                var checkUser = await _signInManager.PasswordSignInAsync(user, loginUser.Password, loginUser.RememberMe, false);
                if (checkUser.Succeeded)
                {
                    return RedirectToAction("Index", "Event","System");
                }
                ModelState.AddModelError(string.Empty, "Invalid Login ");
            }
            return View(loginUser);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Event", "System");
        }


        public IActionResult AccessDenied()
        {
            return View();
        }

    }
}
