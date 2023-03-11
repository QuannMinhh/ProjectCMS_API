using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectCMS.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string? Phone { get; set; }
        public DateTime? DoB { get; set; }
        public string? Address { get; set; }
        public string? Avatar { get; set; }
        public DateTime AddedDate { get; set; }
        public string Role { get; set; }
        public string? RefreshToken { get; set; } = string.Empty;
        public DateTime? TokenCreate { get; set; }
        public DateTime? TokenExpires { get; set; }

        [ForeignKey("DepId")]
        public int DepartmentID { get; set; }

        public ICollection<Idea> Ideas { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<Interactions> Iteractions { get; set; }
    }
}
