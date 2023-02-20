using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectCMS.Models
{
    public class Interactions
    {        

        public int UserId { get; set; }
        public User User { get; set; }

        public int IdeaId { get; set; }
        public Idea Idea { get; set; }

        public bool Voted { get; set; }
        public bool Viewed { get; set; }
        public bool Vote { get; set; }

    }
}
