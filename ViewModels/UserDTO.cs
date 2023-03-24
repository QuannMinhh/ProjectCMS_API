using System.ComponentModel.DataAnnotations;

namespace ProjectCMS.ViewModels
{
    public class UserDTO
    {
        public int? UserId { get; set; }
        [Required]
        [MinLength(5, ErrorMessage = "Username must more than 3 characters"), MaxLength(20, ErrorMessage = "Username must less than 20 characters ")]

        public string UserName { get; set; }
        [Required]
        [MinLength(5, ErrorMessage = "Username must more than 8 characters")]
        public string password { get; set; }
        
        [Required(ErrorMessage = "The email address is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        public string? Phone { get; set; }
        public DateTime? DoB { get; set; }
        public string? Address { get; set; }
        public string? Avatar { get; set; }
        public DateTime AddedDate { get; set; } = DateTime.Now;
        [Required(ErrorMessage = "Role must not be null")]
        public string Role { get;  set; }
        [Required(ErrorMessage ="DepartmentID mus not be null")]
        public int DepartmentID { get; set; }
    }
}
