using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ALMA_API.Models.Db;

public class Cow
{
    public int Id { get; set; }
    public string Tag { get; set; }
    public int Weight { get; set; }
    public DateTime? birthDate { get; set; }
    public DateTime? deathDate { get; set; }
    public int FarmId { get; set; }
    [JsonIgnore]
    public Farm Farm { get; set; }
    [JsonIgnore]
    public List<Production> Production;
}