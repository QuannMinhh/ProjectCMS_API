namespace ProjectCMS.Models
{
    public class Idea
    {
        [Key]
        public int IdeaId { get; set; }

        public User User { get; set; }
        public int UserId { get; set; }
        
        public string Title { get; set; }
        public string Content { get; set; }
        public int Vote { get; set; }
        public int Viewed { get; set; }
        public DateTime SubmitedDate { get; set;}

        [ForeignKey("EvId")]
        public Event Event { get; set; } 
        public int EvId { get; set; }

        [ForeignKey("CateId")]
        public Category Category { get; set; }
        public int CateId { get; set; }

        public ICollection<Interactions> Interactions { get; set; }
        public ICollection<Comment> Comments { get; set; }  
    }
}
