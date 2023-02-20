using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectCMS.Models
{
    public class Interactions
    {
     
        [DisplayName("User")]
        public int UserId { get; set; }
   
        [DisplayName("Idea")]
        public int IdeaId { get; set; }

        public bool Voted { get; set; }
        public bool Viewed { get; set; }
        public bool Vote { get; set; }

        [ForeignKey("IdeaId")]
        public Idea Idea { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

    }
}
