using System;
using System.Collections.Generic;

namespace ChatDAL.ReadDataModels
{
    public partial class ChatMessage
    {
        public long MessageId { get; set; }
        public long ChatId { get; set; }
        public string? SenderId { get; set; }
        public string? ReceiverId { get; set; }
        public string? MessageContent { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? DocumentId { get; set; }
        public DateTime? ReadAt { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int? Status { get; set; }

        public virtual Chat Chat { get; set; } = null!;
        public virtual Status? StatusNavigation { get; set; }
    }
}
