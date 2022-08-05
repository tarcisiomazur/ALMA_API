using System.Text.Json.Serialization;

namespace ALMA_API.Models.Db;

public class Production
{
    public long Id { get; set; }
    public DateTime Time { get; set; }
    public double Quantity { get; set; }
    [JsonIgnore]
    public Cow Cow { get; set; }
    
    public int CowId { get; set; }
}