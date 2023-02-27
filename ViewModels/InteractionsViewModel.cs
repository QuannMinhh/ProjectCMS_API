using ProjectCMS.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectCMS.ViewModels
{
    public class InteractionsViewModel
    {
        public int UserId { get; set; }

        public int IdeaId { get; set; }

        public bool Voted { get; set; } = false;
        public bool Viewed { get; set; } = true;
        public bool Vote { get; set; } = false;
    }
}
