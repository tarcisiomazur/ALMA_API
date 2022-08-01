namespace ALMA_API.Models.Responses
{
    public record BaseResponse()
    {
        public bool Success { get; set; }
        public List<string> MessageList { get; set; } = new();

        public BaseResponse(params string[] errors) : this()
        {
            MessageList.AddRange(errors);
        }
    }
}
