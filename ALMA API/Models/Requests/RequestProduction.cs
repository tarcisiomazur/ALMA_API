using System.Text.Json.Serialization;

namespace ALMA_API.Models.Requests;

public class RequestUpdateProduction: BaseRequest
{
    public int Id { get; set; }
}

public class RequestCreateProduction: BaseRequest
{
    public int? Id { get; set; }
}
