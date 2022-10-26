using System.Text.Json.Serialization;

namespace ALMA_API.Models.Db;

public class Stall
{
    public int Id { get; set; }
    
    public int Index { get; set; }
    
    public bool? Side { get; set; }

    [JsonIgnore]
    public ManageProduction ManageProduction { get; set; }

    public Production? ProductionLeft { get; set; }

    public Production? ProductionRight { get; set; }
}