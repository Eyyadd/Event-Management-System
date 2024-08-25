using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    public class Tickets
    {
        public int Id { get; set; }
        public DateTime ExpireDate { get; set; }

        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public ApplicationUser? User { get; set; }

        [ForeignKey(nameof(Event))]
        public int EventId { get; set; }
        public Event? Event { get; set; }

    }
}
