using System;
using System.Collections.Generic;

namespace AuthenticationDAL.DataModels
{
    public partial class User
    {
        public User()
        {
            NotificationCreatedByNavigations = new HashSet<Notification>();
            NotificationUsers = new HashSet<Notification>();
        }

        public long UserId { get; set; }
        public string UserName { get; set; } = null!;
        public string EmailId { get; set; } = null!;
        public string? AuthId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int? Status { get; set; }
        public bool IsExternal { get; set; }

        public virtual AspNetUser? Auth { get; set; }
        public virtual Status? StatusNavigation { get; set; }
        public virtual ICollection<Notification> NotificationCreatedByNavigations { get; set; }
        public virtual ICollection<Notification> NotificationUsers { get; set; }
    }
}
