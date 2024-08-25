namespace EventManagement.Areas.Admin.ViewModels
{
    public class CreateCategoryVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? ImageURL { get; set; }
        public IFormFile? ImageFile { get; set; }
        public string? Description { get; set; }
    }
}
