using System;
using System.Collections.Generic;

namespace AuthenticationDAL.DataModels
{
    public partial class Status
    {
        public Status()
        {
            ChatMessages = new HashSet<ChatMessage>();
            Chats = new HashSet<Chat>();
            Users = new HashSet<User>();
        }

        public int StatusId { get; set; }
        public string? Action { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<ChatMessage> ChatMessages { get; set; }
        public virtual ICollection<Chat> Chats { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
