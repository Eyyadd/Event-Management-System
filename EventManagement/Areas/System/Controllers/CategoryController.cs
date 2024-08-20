
namespace EventManagement.Areas.System.Controllers
{
    [Area("System")]
    public class CategoryController : Controller
    {
        private readonly IUniteOfWork _uniteOfWork;

        public CategoryController(IUniteOfWork uniteOfWork)
        {
            this._uniteOfWork = uniteOfWork;
        }
        public IActionResult Index()
        {
            IEnumerable<Category> categories = _uniteOfWork.CategoryRepository.GetAll();
            return View("Index",categories);
        }
    }
}
