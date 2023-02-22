using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectCMS.Models
{
    public class Interactions
    {
        [Key]
        public int InteracId { get; set; }

        public User User { get; set; }
        public int UserId { get; set; }

        [ForeignKey("IdeaId")]
        public Idea Idea { get; set; }
        public int IdeaId { get; set; }
        

        public bool Voted { get; set; }
        public bool Viewed { get; set; }
        public bool Vote { get; set; }

    }
}
