using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectCMS.Models
{
    public class Idea
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public DateTime AddedDate { get; set; }
        public int Vote { get; set; }
        public int Viewed { get; set; }  
        public string? IdeaFile { get; set; }

        [ForeignKey("EvId")]
        public Event Event { get; set; } 
        public int EvId { get; set; }

        [ForeignKey("CateId")]
        public Category Category { get; set; }
        public int CateId { get; set; }

        public User User { get; set; }
        public int UserId { get; set; }
        public ICollection<Interactions> Interactions { get; set; }
        public ICollection<Comment> Comments { get; set; }  
    }
}
