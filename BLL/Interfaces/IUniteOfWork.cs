using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IUniteOfWork
    {
        public IRepository<Event> EventRepository { get; }
        public IRepository<Tickets> TicketRepository { get; }
        public IRepository<Category> CategoryRepository { get; }
        public IRepository<Feedback> FeedBackRepository { get; }
        public IRepository<EventBooking> EventBookingRepository { get; }
        void Save();
    }
}
