namespace ALMA_API.Models.Responses;

public record FarmProductionResponse: AppResponse
{
    public DateTime Since { get; set; }
}