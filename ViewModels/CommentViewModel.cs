using System.ComponentModel.DataAnnotations;

namespace ProjectCMS.ViewModels
{
    public class CommentViewModel
    {
        public int UserId { get; set; }
        public int IdeaId { get; set; }
        public DateTime AddedDate { get; set; } = DateTime.Now;

        [Required]
        [MinLength(1, ErrorMessage= "Content must have at least one character"), MaxLength(50, ErrorMessage = "Content must be less than 50 characters")]
        public string Content { get; set; }
    }
}
