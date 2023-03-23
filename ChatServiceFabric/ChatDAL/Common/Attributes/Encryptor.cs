using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace ChatDAL.Common.CustomActionFilter
{
    public class Encryptor : Attribute,IActionFilter
    {
        public string ActionContent { get; set; }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            var result = context.Result as ObjectResult;
            if (result?.Value != null)
            {
                var plaintext = JsonConvert.SerializeObject(result.Value);
                if (plaintext != null)
                {
                    var ciphertext = Utilities.Utilities.EncryptString(plaintext);
                    result.Value = ciphertext;
                }
            }
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {

        }
    }
}
