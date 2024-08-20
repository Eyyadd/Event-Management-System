namespace EventManagement.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =("Admin"))]
    public class AdminDashboardController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public AdminDashboardController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> RetriveUser(int pageNumber=1,int totalPages=20)
        {
            IList<ApplicationUser> Allusers = _userManager.Users.AsNoTracking().ToList();
            if (Allusers.Any())
            {
                IList<UserViewModel> AllUserViewModel = new List<UserViewModel>();
                foreach (var user in Allusers)
                {
                    if (!(await _userManager.IsInRoleAsync(user, "Admin")||await _userManager.IsInRoleAsync(user,"Organizer")))
                    {
                        UserViewModel userView = new UserViewModel()
                        {
                            UserName = user.UserName,
                            Address = user.Address,
                            Age = user.Age,
                            Email = user.Email
                        };
                        AllUserViewModel.Add(userView);
                    }
                }
                AllUserViewModel = PaginatedList<UserViewModel>.Create(AllUserViewModel, pageNumber, totalPages);
                return View("RetriveUser", AllUserViewModel);
            }
            return View("RetriveUser");
        }

        [HttpGet]
        public async Task<IActionResult> DeleteOrganizer(string Email)
        {
            var deletedUser = await _userManager.FindByEmailAsync(Email);
            if (deletedUser is not null)
            {
                IdentityResult DeletedResult = await _userManager.DeleteAsync(deletedUser);
                if (DeletedResult.Succeeded)
                {
                    //TODO Deleted Successfuly 
                    return RedirectToAction("RetriveUser");
                }
                foreach (var Error in DeletedResult.Errors)
                {
                    ModelState.AddModelError(Error.Code, Error.Description);
                }
                //TODO  Message Invalid Operation
            }
            //TODO  Message Not Valid
            return RedirectToAction("RetriveUser");
        }
    }
}
