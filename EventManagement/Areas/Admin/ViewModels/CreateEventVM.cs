namespace EventManagement.Areas.Admin.ViewModels
{
    public class CreateEventVM
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string location { get; set; }
        public int NoOfTickets { get; set; }
        public string? Cover { get; set; }
        public IFormFile? CoverFile { get; set; }
        public decimal Price { get; set; }

        public IEnumerable<Category>? categories { get; set; }
        public int CategoryId { get; set; }
        
    }
}
