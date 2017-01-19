namespace MvvmFramework.Models
{
    public class SingleUser
    {
        public SingleUser(string name, long id, string statusmessage, string status, string channel)
        {
            Name = name;
            Id = id;
            StatusMessage = statusmessage;
            Status = status;
            Channel = channel;
        }

        public string Name { get; set; }

        public long Id { get; set; }
       
        public string StatusMessage { get; set; }

        public string Status { get; set; }

        public string Channel { get; set; }
    }
}
