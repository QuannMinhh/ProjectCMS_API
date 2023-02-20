using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectCMS.Models
{
    public class Interactions
    {
        [ForeignKey("UserId")]
        [DisplayName("User")]
        public int UserId { get; set; }

        [ForeignKey("IdeaId")]
        [DisplayName("Idea")]
        public int IdeaId { get; set; }

        public bool Voted { get; set; }
        public bool Viewed { get; set; }
        public bool Vote { get; set; }

    }
}
