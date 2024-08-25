using BLL.Classes;
using BLL.Interfaces;
using DAL.Models;
using EventManagement.Areas.System.View_Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;

namespace EventManagement.Areas.Events.Controllers
{
    [Area("System")]
    public class TicketController : Controller
    {
        IUniteOfWork uniteOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public TicketController(IUniteOfWork uniteOfWork,UserManager<ApplicationUser> userManager)
        {
            this.uniteOfWork = uniteOfWork;
            _userManager=userManager;
        }


        //public IActionResult Book(int eventId)
        //{
        //    var eventObj = uniteOfWork.EventRepository.Get(eventId);
        //    if (eventObj is not null && eventObj.NoOfTickets > 0)
        //    {
        //        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //        var userEmail = User.FindFirstValue(ClaimTypes.Email);

        //        if (userId is null) // اعمل الاونتكاشن الل عاوزه
        //        {
        //            return RedirectToAction("Register","Account", "System");
        //        }
        //        eventObj.NoOfTickets -= 1;

        //        Tickets ticket = new Tickets();
        //        ticket.UserId = userId;
        //        ticket.EventId = eventId;
        //        ticket.ExpireDate = eventObj.EndDate;

        //        uniteOfWork.TicketRepository.Add(ticket);
        //        uniteOfWork.EventRepository.Update(eventObj);
        //        uniteOfWork.Save();

        //        return RedirectToAction("BookedTicketsForUser", "Ticket", new {userId=userId,area="System"});
        //    }

        //    string error = "No tickets available or event not found";
        //    return View("Book",error);  
        //}

        public IActionResult Book(int eventId)
        {
            var eventObj = uniteOfWork.EventRepository.Get(eventId);
            if (eventObj != null && eventObj.NoOfTickets > 0)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userEmail = User.FindFirstValue(ClaimTypes.Email);

                if (userId == null)
                {
                    return RedirectToAction("Register", "Account", "System");
                }
                eventObj.NoOfTickets -= 1;

                Tickets ticket = new Tickets();
                ticket.UserId = userId;
                ticket.EventId = eventId;
                ticket.ExpireDate = eventObj.EndDate;

                uniteOfWork.TicketRepository.Add(ticket);
                uniteOfWork.EventRepository.Update(eventObj);
                uniteOfWork.Save();

                return RedirectToAction("BookedTicketsForUser", new { UserId = userId });
            }

            string error = "No tickets available or event not found";
            return View("Book", error);
        }

        //public IActionResult BookedTicketsForUser(string UserId)
        //{

        //    var bookedTickets = uniteOfWork.TicketRepository.Find(t=>t.UserId==UserId);

        //    List<UserTicketsVM> userTicketsList = new List<UserTicketsVM>();
        //    UserTicketsVM ticketsVM = new UserTicketsVM();

        //    foreach (var ticket in bookedTickets)
        //    {
        //        var eventDetails = uniteOfWork.EventRepository.Get(ticket.EventId); 

        //        if (eventDetails is not null)
        //        {
        //            ticketsVM.EventName = eventDetails.Name;
        //            ticketsVM.TicketExpireDate = ticket.ExpireDate;
        //            ticketsVM.EventStartDate = eventDetails.StartDate;
        //            ticketsVM.TicketPrice = eventDetails.Price.ToString("C");

        //            userTicketsList.Add(ticketsVM);
        //        }
        //    }
        //    return View("BookedTicketsForUser", userTicketsList);
        //}

        public IActionResult BookedTicketsForUser(string UserId)
        {
            var bookedTickets = uniteOfWork.TicketRepository.GetAll(t => t.UserId == UserId)
                .GroupBy(t => t.EventId)
                .Select(g => new
                {
                    EventId = g.Key,
                    TicketCount = g.Count(),
                    Tickets = g.ToList()
                }).ToList();

            List<UserTicketsVM> userTicketsList = new List<UserTicketsVM>();

            foreach (var group in bookedTickets)
            {
                var eventDetails = uniteOfWork.EventRepository.Get(group.EventId);

                if (eventDetails is not null)
                {
                    userTicketsList.Add(new UserTicketsVM
                    {
                        EventName = eventDetails.Name,
                        TicketExpireDate = group.Tickets.FirstOrDefault()!.ExpireDate,
                        EventStartDate = eventDetails.StartDate,
                        TicketPrice = eventDetails.Price.ToString("C"),
                        NumberOfTickets = group.TicketCount
                    });
                }
            }
            return View("BookedTicketsForUser", userTicketsList);
        }

        public IActionResult Cancel(int TicketId) // لما يبقى عندنا ريفاند إن شاء الله
        {
            var ticket = uniteOfWork.TicketRepository.Get(TicketId);
            if (ticket is not null)
            {
                var eventObj = uniteOfWork.EventRepository.Get(ticket.EventId);
                if (eventObj is not null)
                {
                    eventObj.NoOfTickets += 1;

                    uniteOfWork.TicketRepository.Delete(ticket.Id); 
                    uniteOfWork.EventRepository.Update(eventObj);
                    uniteOfWork.Save();

                    RedirectToAction("Details", "Event", new { id = eventObj.Id });
                }
            }

            return NotFound();
        }


        public IActionResult UpdatePrice(int eventId, int price)
        {
            var eventObj = uniteOfWork.EventRepository.Get(eventId);
            if (eventObj is not null && eventObj.StartDate > DateTime.Now)
            {
                eventObj.Price = price;
                RedirectToAction("Details", "Event");
            }

            return NotFound();
        }
    }
}
