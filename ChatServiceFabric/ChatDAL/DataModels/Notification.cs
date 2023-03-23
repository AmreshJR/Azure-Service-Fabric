using System;
using System.Collections.Generic;

namespace ChatDAL.DataModels
{
    public partial class Notification
    {
        public int NotificationId { get; set; }
        public int NotificationTypeId { get; set; }
        public long UserId { get; set; }
        public string? Title { get; set; }
        public string? Message { get; set; }
        public long CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool? IsReaded { get; set; }
        public long? CommonId { get; set; }

        public virtual User CreatedByNavigation { get; set; } = null!;
        public virtual NotificationType NotificationType { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
