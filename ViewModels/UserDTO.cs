namespace ProjectCMS.ViewModels
{
    public class UserDTO
    {
        
        public string userName { get; set; }
        public string password { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public DateTime? DoB { get; set; }
        public string? Address { get; set; }
        public string? Avatar { get; set; }
        public DateTime AddedDate { get; set; } = DateTime.Now;
        public string Role { get;  set; }
        public int DepartmentID { get; set; }
    }
}
