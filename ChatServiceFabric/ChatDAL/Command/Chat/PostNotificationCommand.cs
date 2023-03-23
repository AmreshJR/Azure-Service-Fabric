using ChatDAL.DTO.Chat;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatDAL.Command.Chat
{
    public class PostNotificationCommand : IRequest<Unit>
    {
        public DtoPostNotification DtoNotification { get; set; }

        public PostNotificationCommand(DtoPostNotification dtoNotification)
        {
            DtoNotification = dtoNotification;
        }
    }
}
