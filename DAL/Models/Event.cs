using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string location { get; set; }
        public int NoOfTickets { get; set; }
        public string Cover { get; set; }
        public decimal Price {  get; set; }

        [ForeignKey("Category")]
        public int? CategoryId {  get; set; }
        public Category? Category { get; set; }  

        public List<Tickets>? Tickets { get; set; }
        public List<Feedback>? Feedbacks { get; set; }

        [ForeignKey(nameof(user))]
        public string? OrganizerId {  get; set; }
        public ApplicationUser? user { get; set; }
    }
}
