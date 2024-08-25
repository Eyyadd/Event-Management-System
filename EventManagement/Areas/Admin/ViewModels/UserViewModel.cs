namespace EventManagement.Areas.Admin.ViewModels
{
    public class UserViewModel
    {
        public string UserName { get; set; }

        [EmailAddress]
        public string Email { get; set; }
        public string Address { get; set; }

        [Range(18,70,ErrorMessage ="The legal age must be from 18 to 70")]
        public int Age { get; set; }
    }
}
