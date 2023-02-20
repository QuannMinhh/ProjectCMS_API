using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectCMS.Models
{
    public class Comment
    {
        [Key]
        [Column(Order = 0)]
        [DisplayName("User")]
        [ForeignKey("UserId")]
        public int UserId { get; set; }
        public virtual User User { get; set; }
        [Key]
        [Column(Order = 1)]
        [DisplayName("Idea")]
        [ForeignKey("IdeaId")]
        public int IdeaId { get; set; }
        public Idea Idea { get; set; }
        [Key]
        [Column(Order = 2)]
        public DateTime AddedDate { get; set; }

        public string Content { get; set; }

        //[ForeignKey("IdeaId")]
        //public Idea Idea { get; set; }

        //[ForeignKey("UserId")]
        //public User User { get; set; }
    }
}
