using System.ComponentModel.DataAnnotations;

namespace ProjectCMS.Models
{
    public class Department
    {
        [Key]
        public int DepId { get; set; }
        public string Name { get; set;}
        public ICollection<User> Users { get; set; }
    }
}
