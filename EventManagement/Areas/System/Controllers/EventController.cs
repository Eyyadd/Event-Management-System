namespace EventManagement.Areas.Events.Controllers
{
    [Area("System")]
    public class EventController : Controller
    {
        private readonly IUniteOfWork uniteOfWork;

        public EventController(IUniteOfWork uniteOfWork)
        {
            this.uniteOfWork = uniteOfWork;
        }
        public IActionResult Index(int Id, string? searchQuery = null)
        {
            IEnumerable<Event> events;
            var category = uniteOfWork.CategoryRepository.Get(Id);

            if (category == null)
            {
                return RedirectToAction("Index", "Category");
            }

            if (!string.IsNullOrEmpty(searchQuery))
            {
                events = uniteOfWork.EventRepository.Find(e => e.CategoryId == Id && e.Name.Contains(searchQuery));
            }
            else
            {
                events = uniteOfWork.EventRepository.GetAll().Where(e => e.CategoryId == Id).ToList();
            }

            ViewBag.SearchQuery = searchQuery;
            ViewBag.CategoryId = Id;
            ViewBag.CategoryName = category.Name;
            return View(events);
        }

        [AllowAnonymous]
        public IActionResult Details(int id)
        {
            Event Ev = uniteOfWork.EventRepository.Get(id);
            if (Ev == null)
            {
                return NotFound();
            }
            return View(Ev);
        }


    }
}
