using ChatDAL.DTO.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatDAL.BusinessRules.Chat.Write
{
    public interface IChat
    {
        (DtoPostResponse, string) PostMessage(DtoPostChatMessage ObjMessage);
        string ReadNotication(int NotificationId);
        string ReadMessage(DtoReadMessage objReadMessage);
        DtoChat GetSelectedUserChat(string SenderId, string ReceiverId);
    }
}
