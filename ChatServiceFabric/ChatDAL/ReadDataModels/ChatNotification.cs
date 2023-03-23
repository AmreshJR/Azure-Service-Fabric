using System;
using System.Collections.Generic;

namespace ChatDAL.ReadDataModels
{
    public partial class ChatNotification
    {
        public int NotificationId { get; set; }
        public string? Message { get; set; }
        public DateTime? CreatedOn { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; } = null!;
    }
}
