namespace ALMA_API.Models.Responses
{
    public record AuthResponse : BaseResponse
    {
        public string Token { get; set; }
        public int ExpireDate { get; set; }
    }
}
