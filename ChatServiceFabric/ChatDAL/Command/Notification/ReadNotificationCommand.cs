using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatDAL.Command.Notification
{
    public class ReadNotificationCommand : IRequest<string>
    {
        public int NotificationId { get; set; }

        public ReadNotificationCommand(int notificationId)
        {
            NotificationId = notificationId;
        }
    }
}
