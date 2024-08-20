using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    public class Feedback
    {
        public int Id { get; set; }
        public string Description;

        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public ApplicationUser? User { get; set; }

        [ForeignKey(nameof(Event))]
        public int EventId {  get; set; }
        public Event? Event { get; set; }


    }
}
