using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? ImageURL {  get; set; }
        public string? Description { get; set; }
        public List<Event>? Events { get; set; }
    }
}
