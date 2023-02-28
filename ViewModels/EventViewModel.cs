using System.ComponentModel.DataAnnotations;

namespace ProjectCMS.ViewModels
{
    public class EventViewModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        public DateTime First_Closure { get; set; }
        [Required]
<<<<<<< HEAD
        public int CateId { get; set; }
=======
        public DateTime Last_Closure { get; set; }
>>>>>>> Back-End2
    }
}
