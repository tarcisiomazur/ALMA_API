using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ALMA_API.Models.Db;

public class ManageProduction
{
    public int Id { get; set; }
    
    public int FarmId { get; set; }
    [JsonIgnore]
    public Farm Farm { get; set; }

    public List<Stall> Stalls { get; set; }
}