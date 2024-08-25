namespace EventManagement.Areas.System.View_Models
{
    public class ChangePasswordViewModel
    {
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Compare(nameof(NewPassword))]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}
