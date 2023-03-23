using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatDAL.DTO.Common
{
    public class DtoRegisterInput
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }
    
    public class DtoLogin
    {
        public string Email { get; set; }

        public string Password { get; set; }
    }

    public class DtoLoginResponse
    {
        public string Token { get; set; }
        public string UserName { get; set; }
        public long UserId { get; set; }
        public string ErroMsg { get; set; }

    }

    public class DtoOAuthLogin
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string Id { get; set; }
        public string IdToken { get; set; }
        public string LastName { get; set; }
        public string Name { get; set; }
        public string PhotoUrl { get; set; }
        public string Provider { get; set; }
    }
}
