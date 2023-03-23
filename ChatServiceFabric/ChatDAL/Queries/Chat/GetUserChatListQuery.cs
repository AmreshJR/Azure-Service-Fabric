using ChatDAL.DTO.Chat;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatDAL.Queries.Chat
{
    public class GetUserChatListQuery : IRequest<(List<DtoChat>,string)>
    {
        public long UserId { get; set; }

        public GetUserChatListQuery(long userId)
        {
            UserId = userId;
        }
    }
}
