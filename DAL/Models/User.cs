using Microsoft.AspNetCore.Identity;

namespace DAL.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string Address { get; set; }
        public int Age { get; set; }
        public List<Tickets>? Tickets { get; set; }
        public List<Feedback>? Feedbacks { get; set; }
    }
}
