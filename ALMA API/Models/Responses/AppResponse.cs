namespace ALMA_API.Models.Responses;

public record AppResponse(): BaseResponse
{
    public object Payload { get; set; }

    public AppResponse(object payload) : this()
    {
        Success = true;
        Payload = payload;
    }
}