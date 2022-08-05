using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ALMA_API.Models.Db;

public enum CowState
{
    Growth,
    Dry,
    Pregnant,
    Lactation,
    Death
}

public class Cow
{
    public int Id { get; set; }

    public string Tag { get; set; }

    public int Weight { get; set; }

    public DateTime? BirthDate { get; set; }

    [Range(1, 5)] public int BCS { get; set; }

    public CowState State;

    public DateTime? LastInsemination { get; set; }

    public int FarmId { get; set; }

    [JsonIgnore] public Farm Farm { get; set; }

    [JsonIgnore] public List<Production> Production { get; set; }
    
    [Column(TypeName="DATE")]
    [JsonIgnore] public DateTime? LazyCalculation { get; set; }

    public double MeanProduction { get; set; }
}