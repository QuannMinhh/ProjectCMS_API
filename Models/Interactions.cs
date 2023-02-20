using Duende.IdentityServer.EntityFramework.Entities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectCMS.Models
{
    public class Interactions
    {
        [Key]
        [Column(Order = 0)]
        [DisplayName("User")]
        [ForeignKey("UserId")]
        public int UserId { get; set; }
        public virtual User User { get; set; }
        [Key]
        [Column(Order = 1)]
        [DisplayName("Idea")]
        [ForeignKey("IdeaId")]
        public int IdeaId { get; set; }
        public Idea Idea { get; set; }

        public bool Voted { get; set; }
        public bool Viewed { get; set; }
        public bool Vote { get; set; }


        
       
    }
}
