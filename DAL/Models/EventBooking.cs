using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    public class EventBooking
    {
        public int Id { get; set; }

        public int TotalSoldTickets { get; set; }

        public int TotalPrice { get; set; }

        [ForeignKey(nameof(Events))]
        public int EventId { get; set; }
        public Event? Events { get; set; }


        [ForeignKey(nameof(Feedback))]
        public int FeedbackId { get; set; }

        public Feedback? Feedback;
    }
}
