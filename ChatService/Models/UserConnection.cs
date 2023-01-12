namespace ChatService.Models
{
    public class UserConnection
    {
        public UserConnection()
        {

        }

        public UserConnection(string name)
        {
            this.UserNickName = name;
        }

        public string Id { get; set; }
        public string UserNickName { get; set; }
        public string Room { get; set; } 

        public void SetId(string id)
        {
            this.Id = id;
        }
    }
}
