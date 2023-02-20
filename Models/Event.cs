using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ProjectCMS.Models
{
    public class Event
    {
        [ForeignKey("CateId")]
        [Key]
        public int EvId { get; set; }
        public string Name { get; set; }
        public DateTime First_Closure { get; set; }
        public DateTime Last_Closure { get; set;}
        public int CateId { get; set; }
        public Category Category { get; set; }
        public ICollection<Idea> Ideas { get; set; }
    }
}
