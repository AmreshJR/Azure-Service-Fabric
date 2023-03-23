using System;
using System.Collections.Generic;

namespace AuthenticationDAL.DataModels
{
    public partial class Chat
    {
        public Chat()
        {
            ChatMessages = new HashSet<ChatMessage>();
        }

        public long ChatUid { get; set; }
        public string ConversationUserIdList { get; set; } = null!;
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int? Status { get; set; }

        public virtual Status? StatusNavigation { get; set; }
        public virtual ICollection<ChatMessage> ChatMessages { get; set; }
    }
}
