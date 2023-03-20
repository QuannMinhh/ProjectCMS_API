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
        public int CateId { get; set; }
        [Required]
        public bool First_IsOverDeadline { get; set; }
        public bool Second_IsOverDeadline { get; set; }

    }
}
