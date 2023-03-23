using System;
using System.Collections.Generic;

namespace AuthenticationDAL.DataModels
{
    public partial class NotificationType
    {
        public NotificationType()
        {
            Notifications = new HashSet<Notification>();
        }

        public int NotificationTypeId { get; set; }
        public string NotificationType1 { get; set; } = null!;

        public virtual ICollection<Notification> Notifications { get; set; }
    }
}
