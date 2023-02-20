using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectCMS.Models
{
    public class Idea
    {
        [Key]
        public int IdeaId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int Vote { get; set; }
        public int Viewed { get; set; }
        public DateTime SubmitedDate { get; set;}
        public int eId { get; set; }

        [ForeignKey("Event")]
        public Event Events { get; set; }
    }
}
