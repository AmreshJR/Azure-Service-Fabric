using ChatDAL.DTO.Chat;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatDAL.Command.Chat
{
    public class ReadMessageCommand : IRequest<string>
    {
        public DtoReadMessage ReadMessage { get; set; }

        public ReadMessageCommand(DtoReadMessage readMessage)
        {
            ReadMessage = readMessage;
        }
    }
}
