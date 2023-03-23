﻿using ChatDAL.DTO.Chat;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatDAL.Command.Chat
{
    public class PostMessageCommand : IRequest<string>
    {
        public DtoPostChatMessage Message { get; set; }

        public PostMessageCommand(DtoPostChatMessage message)
        {
            Message = message;
        }
    }
}
