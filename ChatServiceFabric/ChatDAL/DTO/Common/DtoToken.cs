namespace ChatDAL.DTO.Common
{
    public class DtoToken
    {
        public string JWT_Secret { get; set; }
        public string Client_URL { get; set; }
        public string JwtIssuer { get; set; }
        public string JwtExpireDays { get; set; }
    }
}
