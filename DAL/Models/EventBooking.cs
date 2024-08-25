using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    public class EventBooking
    {
        public int Id { get; set; }

        public int? TotalSoldTickets { get; set; }
        public int TotalNumberOfTickets { get; set; }

        public decimal? TotalPrice { get; set; }

        [ForeignKey(nameof(Events))]
        public int EventId { get; set; }
        public Event? Events { get; set; }

    }
}
