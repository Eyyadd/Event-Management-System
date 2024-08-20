namespace EventManagementSystem.View_Models
{
    public class RegisterUserViewModel
    {
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DisplayName("Confirm Password")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DisplayName("Confirm Mail")]
        [DataType(DataType.EmailAddress)]
        [Compare(nameof(Email))]
        public string ConfirmEmail { get; set; }
        public string Phone { get; set; }

        [Range(18,70,ErrorMessage ="Age must be in range 18 - 70 ")]
        public int Age { get; set; }
        public string Address { get; set; }

    }
}
