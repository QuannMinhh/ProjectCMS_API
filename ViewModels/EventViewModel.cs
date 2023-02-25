using ProjectCMS.Models;
using System.ComponentModel.DataAnnotations;

namespace ProjectCMS.ViewModels
{
    public class EventViewModel
    {
        [Required]
        public int EvId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public DateTime First_Closure { get; set; }
        [Required]
        public DateTime Last_Closure { get; set; }
        [Required]
        public Category Category { get; set; }
        [Required]
        public int CateId { get; set; }
    }
}
