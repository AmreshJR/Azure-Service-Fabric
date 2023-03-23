using OpenAI.GPT3.Managers;
using OpenAI.GPT3.ObjectModels.SharedModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatDAL.BusinessRules.ChatGPT
{
    public interface IChatGPT
    {
        OpenAIService ConnectApi(string input);

        Task<string> TriggerChatGPT(string input);
    }
}
