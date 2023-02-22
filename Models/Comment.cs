using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectCMS.Models
{
    public class Comment
    {
        [Key]    
        public int CommentId { get; set; }

        public User User { get; set; }
        public int UserId { get; set; }

        [ForeignKey("IdeaId")]
        public Idea Idea { get; set; }
        public int IdeaId { get; set; }
  
        public DateTime AddedDate { get; set; }

        public string Content { get; set; }
    }
}
