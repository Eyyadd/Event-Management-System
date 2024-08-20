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
        public IActionResult Index()
        {
            IEnumerable<Event> events = uniteOfWork.EventRepository.GetAll(); 
            return View(events);
        }
    }
}
