using System.Text.Json.Serialization;

namespace ALMA_API.Models.Db;

public class Production
{
    public long Id { get; set; }
    public DateTime Time { get; set; } = DateTime.Now;

    public double Quantity { get; set; } = 0;
    [JsonIgnore]
    public Cow Cow { get; set; }
    
    public int CowId { get; set; }
}