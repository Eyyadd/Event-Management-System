using EventManagement.Models;
using Hangfire;
using Microsoft.AspNetCore.Mvc;

namespace EventManagement.Areas.Organizer.Controllers
{
    [Area("Organizer")]
    [Authorize(Roles = Role.OrganizerRole)]
    public class EventOrganizerController : Controller
    {
        private readonly IUniteOfWork _uniteOfWork;
        private readonly IFileImageService _fileImageService;

        public EventOrganizerController(IUniteOfWork uniteOfWork,IFileImageService fileImageService)
        {
            this._uniteOfWork = uniteOfWork;
            this._fileImageService = fileImageService;
        }

        public IActionResult Index()
        {
            var OrganizerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (OrganizerId is not null)
            {
                IEnumerable<Event> events = _uniteOfWork.EventRepository.Find(ev => ev.OrganizerId == OrganizerId);
                return View(events);

            }
            ModelState.AddModelError(string.Empty, "Organzer not found");
            return View(ModelState);
        }
        public IActionResult Details(int id)
        {
            Event CurrentEvent = _uniteOfWork.EventRepository.Get(id);
            if (CurrentEvent is null)
            {
                return NotFound("Event Not Found");
            }
            return View(CurrentEvent);
        }

        [HttpGet]
        public IActionResult Create()
        {
            //ViewBag.Categories = new SelectList(uniteOfWork.CategoryRepository.GetAll(), "Id", "Name");
            CreateEventVM createEventVM = new CreateEventVM();
            createEventVM.categories = _uniteOfWork.CategoryRepository.GetAll();
            return View("Create", createEventVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveCreate(CreateEventVM newEventVM)
        {
            var organizerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (newEventVM.CoverFile != null)
            {
                if (newEventVM.CoverFile.Length > 5 * 1024 * 1024)
                {
                    throw new InvalidOperationException("File Size cannot exceed 5 mb");
                }
                newEventVM.Cover = _fileImageService.SaveFile(newEventVM.CoverFile, "img", new string[] { ".jpg", ".jpeg", ".png" });
            }
            if (newEventVM.Name!=null && newEventVM.location!=null && newEventVM.NoOfTickets!=null && newEventVM.Price!=null)
            {
                if (organizerId is not null)
                {
                    Event newEvent = new Event()
                    {
                        Name = newEventVM.Name,
                        CategoryId = newEventVM.CategoryId,
                        OrganizerId = organizerId,
                        EndDate = newEventVM.EndDate,
                        StartDate = newEventVM.StartDate,
                        Description = newEventVM.Description,
                        NoOfTickets = newEventVM.NoOfTickets,
                        Price = newEventVM.Price,
                        location = newEventVM.location,
                        Cover = newEventVM.Cover,
                    };
                    _uniteOfWork.EventRepository.Add(newEvent);
                    _uniteOfWork.Save();
                    TempData["SuccessMessage"] = "Event Added Successfully";
                    CreateEventHistory(newEvent.Id);
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError(string.Empty, "Organizer Not Found");
            }
            newEventVM.categories = _uniteOfWork.CategoryRepository.GetAll();
            return RedirectToAction("Create",newEventVM);
        }
        public IActionResult Edit(int id)
        {
            var eventForEdit = _uniteOfWork.EventRepository.Get(id);
            if (eventForEdit is null)
            {
                return NotFound("Event Not Found");
            }
            return View(eventForEdit);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Event @event)
        {
            if (id != @event.Id)
            {
                return NotFound("Event Not Found");
            }

            if (ModelState.IsValid)
            {
                _uniteOfWork.EventRepository.Update(@event);
                _uniteOfWork.Save();
                return RedirectToAction("Index");
            }
            return View(@event);
        }
        public IActionResult Delete(int id)
        {
            Event @event = _uniteOfWork.EventRepository.Get(id);
            if (@event is null)
            {
                return NotFound("Event Not Found");
            }

            return View(@event);
        }
        [HttpDelete]
        public IActionResult ConfirmDelete(int id)
        {
            Event EventToDelete = _uniteOfWork.EventRepository.Get(id);
            if (!ModelState.IsValid)
            {
                return NotFound();
            }
            //At least one ticket reserved
            var Tickets = _uniteOfWork.TicketRepository.Find(t => t.EventId == id);
            if (Tickets.Any())
            {
                ModelState.AddModelError("", "Can not delete this event because there are tickects reserved .");
                return View(EventToDelete);
            }
            _uniteOfWork.EventRepository.Delete(id);
            _uniteOfWork.Save();
            return RedirectToAction("Index");
        }


        public void CreateEventHistory(int eventId)
        {
            Event startedEvent = _uniteOfWork.EventRepository.Get(eventId);
            var durationFromNowToEndEvent = startedEvent.EndDate - DateTime.Now;
            BackgroundJob.Enqueue(() => CreateNewEventInHistory(eventId, startedEvent));
            BackgroundJob.Schedule(() => UpdateEndedEventInHistory(eventId), durationFromNowToEndEvent);
        }


        public void CreateNewEventInHistory(int id, Event startedEvent)
        {
            int TotalTicketsNumber = startedEvent.NoOfTickets;
            _uniteOfWork.EventBookingRepository.Add(new EventBooking()
            {
                EventId = id,
                TotalNumberOfTickets = startedEvent.NoOfTickets
            });
            _uniteOfWork.Save();
        }

        public void UpdateEndedEventInHistory(int id)
        {
            Event EndedEvent = _uniteOfWork.EventRepository.Get(id);
            EventBooking eventBooking = _uniteOfWork.EventBookingRepository.Find(e => e.EventId == id).FirstOrDefault();
            if (eventBooking is not null)
            {
                eventBooking.TotalSoldTickets = eventBooking.TotalSoldTickets - EndedEvent.NoOfTickets;
                eventBooking.TotalPrice = (eventBooking.TotalSoldTickets * EndedEvent.Price);
                _uniteOfWork.EventBookingRepository.Update(eventBooking);
                _uniteOfWork.Save();
            }
        }
        public IActionResult GetFromHistory()
        {
            var OrganizerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            List<Event> events = _uniteOfWork.EventRepository.Find(E => E.OrganizerId == OrganizerId).ToList();
            List<EventBooking> History = _uniteOfWork.EventBookingRepository
                                        .FilterIncluded("Events", H => events.Contains(H.Events)).ToList();
            return View(History);
        }

    }
}
