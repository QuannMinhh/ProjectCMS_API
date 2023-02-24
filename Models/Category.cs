using System.ComponentModel.DataAnnotations;

namespace ProjectCMS.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public DateTime AddedDate { get; set; }

        public ICollection<Idea> Ideas { get; set; }
    }
}
