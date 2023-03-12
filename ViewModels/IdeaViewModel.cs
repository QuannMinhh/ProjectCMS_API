using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectCMS.ViewModels
{
    public class IdeaViewModel
    {
        [Required]
        [StringLength(100)]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
        public int Vote { get; set; } = 0;
        public int Viewed { get; set; } = 0;
        public DateTime SubmitedDate { get; set; } = DateTime.Now;
        [Required]
        public int eId { get; set; }
        [Required]
        public int cId { get; set; }
        [Required]
        public int uId { get; set; }

        [AllowedExtensions(new string[] { ".pdf" })]
        [MaxFileSize(5 * 1024 * 1024)]
        public IFormFile? IdeaFile { get; set; }

    }
    
}
