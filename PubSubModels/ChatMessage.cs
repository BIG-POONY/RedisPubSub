using System;

namespace PubSubModels
{
    public class ChatMessage
    {
        public string Message { set; get; }
        public string User { set; get; }

        private DateTime _created;
        public DateTime Created 
        {
            get { return _created; }
        }

        public ChatMessage(string user, string msg)
        {
            User = user;
            Message = msg;

            _created = DateTime.UtcNow;
        }
    }
}
