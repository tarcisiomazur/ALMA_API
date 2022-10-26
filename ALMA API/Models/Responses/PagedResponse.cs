namespace ALMA_API.Models.Responses;

public record PagedResponse : AppResponse
{
    public bool EndPage { get; set; }
};