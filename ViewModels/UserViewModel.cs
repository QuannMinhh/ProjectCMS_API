namespace ProjectCMS.ViewModels
{
    public class UserViewModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string password { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public DateTime? DoB { get; set; }
        public string? Address { get; set; }
        public string? Avatar { get; set; }
        public DateTime AddedDate { get; set; }
        public string Role { get; set; }
        public string  Department { get; set; }
    }
}
