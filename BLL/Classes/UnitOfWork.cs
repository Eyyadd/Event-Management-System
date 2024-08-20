using BLL.Interfaces;
using DAL.Data;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Classes
{
    public class UnitOfWork : IUniteOfWork
    {
        private readonly ApplicationDbContext applicationDbContext;

        public UnitOfWork(ApplicationDbContext applicationDbContext
            , IRepository<Event> EventRepository
            ,IRepository<Tickets> TicketRepository
            ,IRepository<Category> CategoryRepository
            ,IRepository<Feedback> FeedBackRepository
            ,IRepository<EventBooking> EventBookingRepository)
        {
            this.applicationDbContext = applicationDbContext;
            this.EventRepository = EventRepository;
            this.TicketRepository = TicketRepository;
            this.CategoryRepository = CategoryRepository;
            this.FeedBackRepository = FeedBackRepository;
            this.EventBookingRepository = EventBookingRepository;
        }

        public IRepository<Event> EventRepository { get; }
        public IRepository<Tickets> TicketRepository { get; }
        public IRepository<Category> CategoryRepository { get; }

        public IRepository<Feedback> FeedBackRepository { get; }

        public IRepository<EventBooking> EventBookingRepository { get; }

        public void Save()
        {
            applicationDbContext.SaveChanges();
        }
    }
}
