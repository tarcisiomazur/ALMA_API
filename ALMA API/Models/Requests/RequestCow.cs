namespace ALMA_API.Models.Requests;

public class UpdateCow: BaseRequest
{
    public int Id { get; set; }
    public string? Tag { get; set; }
    public int? Weight { get; set; }
    public DateTime? BirthDate { get; set; }
    public DateTime? DeathDate { get; set; }
    public int? FarmId { get; set; }
}

public class CreateCow: BaseRequest
{
    public int? Id { get; set; }
    public string Tag { get; set; }
    public int Weight { get; set; }
    public DateTime? BirthDate { get; set; }
    public DateTime? DeathDate { get; set; }
    public int FarmId { get; set; }
}