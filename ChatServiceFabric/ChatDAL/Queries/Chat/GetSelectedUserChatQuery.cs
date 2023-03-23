using ChatDAL.DTO.Chat;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatDAL.Queries.Chat
{
    public class GetSelectedUserChatQuery : IRequest<DtoChat>
    {
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }

        public GetSelectedUserChatQuery(string receiverId, string senderId)
        {
            ReceiverId = receiverId;
            SenderId = senderId;
        }
    }
}
