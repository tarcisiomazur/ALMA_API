using System.Text.Json.Serialization;
using ALMA_API.Models.Db;

namespace ALMA_API.Models.Requests;

public class UpdateCow: BaseRequest
{
    public int Id { get; set; }
    public string? Tag { get; set; }
    public string? Identification { get; set; }
    public DateTime? BirthDate { get; set; }
    public int BCS { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public CowState? State { get; set; }
    public DateTime? LastCalving { get; set; }
    public DateTime? LastInsemination { get; set; }
    public string? Note { get; set; }
    public int? FarmId { get; set; }
    
}

public class CreateCow: BaseRequest
{
    public int? Id { get; set; }
    public string Tag { get; set; }
    public string Identification { get; set; }
    public DateTime? BirthDate { get; set; }
    public int BCS { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public CowState State { get; set; }
    public DateTime? LastCalving { get; set; }
    public DateTime? LastInsemination { get; set; }
    public string Note { get; set; }
    public int FarmId { get; set; }
}