using EventManagement.Areas.Admin.ViewModels;
using EventManagement.Models;
using Stripe;

namespace EventManagement.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =Role.AdminRole)]
    public class AdminDashboardController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUniteOfWork _uniteOfWork;
        private readonly IFileImageService _fileImageService;

        public AdminDashboardController(UserManager<ApplicationUser> userManager,IUniteOfWork uniteOfWork,IFileImageService fileImageService)
        {
            _userManager = userManager;
            _uniteOfWork = uniteOfWork;
            _fileImageService = fileImageService;
        }

        [HttpGet]
        public async Task<IActionResult> RetrieveUsers(int pageNumber=1,int totalPages=20)
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
                            UserName = user.UserName!,
                            Address = user.Address,
                            Age = user.Age,
                            Email = user.Email!
                        };
                        AllUserViewModel.Add(userView);
                    }
                }
                AllUserViewModel = PaginatedList<UserViewModel>.Create(AllUserViewModel, pageNumber, totalPages);
                return View("RetrieveUsers", AllUserViewModel);
            }
            return View("RetrieveUsers");
        }

        [HttpGet]
        public async Task<IActionResult> RetrieveOrganizers()
        {
            IList<ApplicationUser> organizers= _userManager.Users.AsNoTracking().ToList();
            if (organizers.Any())
            {
                IList<UserViewModel> AllOrganizer = new List<UserViewModel>();
                foreach (var user in organizers)
                {
                    if (await _userManager.IsInRoleAsync(user,"Organizer"))
                    {
                        UserViewModel organizer = new UserViewModel()
                        {
                            UserName = user.UserName!,
                            Address = user.Address,
                            Age = user.Age,
                            Email = user.Email!
                        };
                        AllOrganizer.Add(organizer);
                    }
                }
                return View("RetrieveOrganizers", AllOrganizer);
            }
            return View("RetrieveOrganizers");
        }


        //TODO Not Delete The user just delete the role only 
        [HttpPost]
        public async Task<IActionResult> DeleteOrganizer(string Email)
        {
            var deletedUser = await _userManager.FindByEmailAsync(Email);
            if (deletedUser is not null)
            {
                await _userManager.RemoveFromRoleAsync(deletedUser, Role.OrganizerRole);
                return RedirectToAction("RetrieveOrganizers");
                //IdentityResult DeletedResult = await _userManager.DeleteAsync(deletedUser);
                //if (DeletedResult.Succeeded)
                //{
                //    //TODO Deleted Successfuly 
                //    return RedirectToAction("RetrieveOrganizers");
                //}
                //foreach (var Error in DeletedResult.Errors)
                //{
                //    ModelState.AddModelError(Error.Code, Error.Description);
                //}
                ////TODO  Message Invalid Operation
            }
            //TODO  Message Not Valid
            return RedirectToAction("RetrieveOrganizers");
        }
        public IActionResult RetrieveCategories()
        {
            IEnumerable<Category> categories = _uniteOfWork.CategoryRepository.GetAll();
            return View("RetrieveCategories", categories);
        }

        public IActionResult CreateCategory()
        {
            CreateCategoryVM createCategoryVM = new CreateCategoryVM();
            return View("CreateCategory", createCategoryVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateCategory(CreateCategoryVM newCategoryVM)
        {
            if (newCategoryVM.ImageFile != null)
            {
                if (newCategoryVM.ImageFile.Length > 5 * 1024 * 1024)
                {
                    throw new InvalidOperationException("File Size cannot exceed 5 mb");
                }
                newCategoryVM.ImageURL = _fileImageService.SaveFile(newCategoryVM.ImageFile, "img", new string[] { ".jpg", ".jpeg", ".png" });
            }
            if (ModelState.IsValid)
            {
                Category newCategory = new Category();
                newCategory.Name = newCategoryVM.Name;
                newCategory.Description = newCategoryVM.Description;
                newCategory.ImageURL = newCategoryVM.ImageURL;
                _uniteOfWork.CategoryRepository.Add(newCategory);
                _uniteOfWork.Save();
                TempData["SuccessMessage"] = "Category Added Successfully!";
                return RedirectToAction("RetrieveCategories");
            }
            return RedirectToAction("CreateCategory", newCategoryVM);
        }

        [HttpDelete]
        public IActionResult DeleteCategory(int id)
        {
            Category category = _uniteOfWork.CategoryRepository.Get(id);
            if (category != null)
            {
                _uniteOfWork.CategoryRepository.Delete(id);
                _uniteOfWork.Save();
                return RedirectToAction("RetrieveCategories");
            }
            return RedirectToAction("RetrieveCategories");
        }
    }
}
