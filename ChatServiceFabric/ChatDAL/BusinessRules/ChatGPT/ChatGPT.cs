using OpenAI.GPT3;
using OpenAI.GPT3.Interfaces;
using OpenAI.GPT3.Managers;
using OpenAI.GPT3.ObjectModels.RequestModels;
using OpenAI.GPT3.ObjectModels;
using ChatDAL.Constant;
using OpenAI.GPT3.ObjectModels.SharedModels;
using OpenAI.GPT3.ObjectModels.ResponseModels;

namespace ChatDAL.BusinessRules.ChatGPT
{
    public class ChatGPT : IChatGPT
    {
        public OpenAIService ApiConnection;
        public ChatGPT()
        {
            ApiConnection = ConnectApi(BusinessConstants.ChatGPTConnection.ApiKey);
        }
        public OpenAIService ConnectApi(string key)
        {
            try
            {
                var setting = (new OpenAiOptions()
                {
                    ApiKey = key,
                });

                var gpt3 = new OpenAIService(setting);
                return gpt3;
            }
            catch(Exception ex)
            {
                return new OpenAIService(null);
            }
        }

        public async Task<string> TriggerChatGPT(string input)
        {
            var result = string.Empty;

            try
            {
                var settings = new CompletionCreateRequest()
                {
                    Prompt = input,
                    Model = Models.TextDavinciV3,
                    Temperature = 0.5F,
                    MaxTokens = 2000,
                    N = 3
                };
                var completionResult = await ApiConnection.Completions.CreateCompletion(settings);
                if(completionResult.Successful)
                {
                    result = completionResult.Choices[0].Text;
                }

            }
            catch(Exception ex)
            {

            }
            return result;
        }
    }
}
