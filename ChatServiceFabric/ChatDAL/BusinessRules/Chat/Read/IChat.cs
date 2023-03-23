using ChatDAL.DTO.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatDAL.BusinessRules.Chat.Read
{
    public interface IChat
    {
        (List<DtoChat>, string) GetUserChatList(long UserId);
        (DtoNotificationList, string) GetNotificationList(long UserId);
        (List<DtoGetAllUsers>, string ErrorMsg) GetAllUsers();
        (DtoPagenationResponse, string) GetMessage(DtoPagenationData PageData);
        DtoChat GetSelectedUserChat(string SenderId, string ReceiverId);
    }
}
