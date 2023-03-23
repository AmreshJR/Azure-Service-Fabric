using ChatDAL.DTO.Chat;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatDAL.Queries.Notification
{
    public class GetNotificationListQuery : IRequest<(DtoNotificationList,string)>
    {
        public long UserId { get; set; }

        public GetNotificationListQuery(long userId)
        {
            UserId = userId;
        }
    }
}
