using ProjectCMS.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectCMS.ViewModels
{
    public class InteractionsViewModel
    {
        public int UserId { get; set; }

        public int IdeaId { get; set; }

    }

    public class EditInteractionModel
    {
        public int InteractionId { get; set; }
        public bool Vote { get; set; }
    }
}
