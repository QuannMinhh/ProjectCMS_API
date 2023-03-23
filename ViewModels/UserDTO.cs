using System.ComponentModel.DataAnnotations;

namespace ProjectCMS.ViewModels
{
    public class UserDTO
    {
        public int? UserId { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string password { get; set; }
        [Required]
        public string Email { get; set; }
        public string? Phone { get; set; }
        public DateTime? DoB { get; set; }
        public string? Address { get; set; }
        public string? Avatar { get; set; }
        public DateTime AddedDate { get; set; } = DateTime.Now;
        [Required]
        public string Role { get;  set; }
        [Required]
        public int DepartmentID { get; set; }
    }
}
