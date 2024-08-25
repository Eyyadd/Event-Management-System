namespace EventManagement.Areas.Events.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Role.AdminRole)]
    public class RoleController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleController(
                                UserManager<ApplicationUser> userManager,
                                RoleManager<IdentityRole> roleManager
                             )
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult Create()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoleViewModel roleVm)
        {
            if (ModelState.IsValid)
            {
                var checkrole = await _roleManager.FindByNameAsync(roleVm.Name);
                if (checkrole is null)
                {

                    IdentityRole MappingRole = new IdentityRole();
                    MappingRole.Name = roleVm.Name;

                    var result = await _roleManager.CreateAsync(MappingRole);
                    if (result.Succeeded)
                    {
                        TempData["SuccessMessage"] = "Role Added Successfully";
                        return RedirectToAction("RetrieveUsers", "AdminDashboard", new { area = "Admin" });
                    }
                    foreach (var Error in result.Errors)
                    {
                        ModelState.AddModelError(Error.Code, Error.Description);
                    }
                }
                else
                {
                    if (checkrole.Name == "Admin")
                    {
                        await _roleManager.AddClaimAsync(checkrole, new Claim("NumberOfAdmins", "1"));
                    }
                    ModelState.AddModelError(string.Empty, "The Role Already Exist");
                }
            }
            return View(roleVm);
        }

        [HttpGet]
        public IActionResult AssignRole()
        {
            var users = _userManager.Users.ToList();
            var roles = _roleManager.Roles.ToList();

            AssignRoleViewModel assignRoleVm = new AssignRoleViewModel();

            foreach (var user in users)
            {
                assignRoleVm.Users.Add(new SelectListItem
                {
                    Value = user.Id,
                    Text = user.UserName
                });
            }

            foreach (var role in roles)
            {
                assignRoleVm.Roles.Add(new SelectListItem
                {
                    Value = role.Id,
                    Text = role.Name
                });
            }

            return View(assignRoleVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignRole(AssignRoleViewModel assignRole)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(assignRole.UserId);
                var role = await _roleManager.FindByIdAsync(assignRole.RoleId);
                if (user is not null && role is not null)
                {
                    await _userManager.AddToRoleAsync(user, role.Name);
                    return RedirectToAction("RetrieveUsers", "AdminDashboard", new { area = "Admin" });
                }
                ModelState.AddModelError(string.Empty, "Invalid Assign");
            }
            return View(assignRole);
        }
    }
}
