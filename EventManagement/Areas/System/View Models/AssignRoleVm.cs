namespace EventManagement.Areas.System.View_Models
{
    public class AssignRoleVm
    {
        public string RoleId { get; set; }
        public string UserId { get; set; }

        public List<SelectListItem>? Users { get; set; } = new List<SelectListItem>();
        public List<SelectListItem>? Roles { get; set; } = new List<SelectListItem>();
    }
}
