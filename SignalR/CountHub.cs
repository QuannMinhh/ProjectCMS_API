using Microsoft.AspNetCore.SignalR;

namespace ProjectCMS.SignalR
{
    public class InformationUpdateHub : Hub
    {
        protected string Message { get; set; }

        public InformationUpdateHub() 
        {
            Message= "Default Message";
        }
        public virtual async Task Update(int count)
        {
            await Clients.All.SendAsync(Message, count);            
        }
    }

   public class CountIdeaHub : InformationUpdateHub
    {
        public CountIdeaHub()
        {
            this.Message = "UpdateCountIdea";
        }       
    }

    public class CountUserHub : InformationUpdateHub
    {
        public CountUserHub()
        {
            this.Message = "UpdateCountUser";
        }
    }
    public class CountEventHub : InformationUpdateHub
    {
        public CountEventHub()
        {
            this.Message = "UpdateCountEvent";
        }
    }
    public class CountCateHub : InformationUpdateHub
    {
        public CountCateHub()
        {
            this.Message = "UpdateCountCate";
        }
    }
}
