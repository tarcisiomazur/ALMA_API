using System.ComponentModel;
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

    [DefaultValue("")] public string Identification { get; set; }

    [Column(TypeName = "DATE")]
    public DateTime? BirthDate { get; set; }

    [DefaultValue(3)] [Range(1, 5)] public int BCS { get; set; }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    [DefaultValue(CowState.Lactation)] public CowState State { get; set; }

    [Column(TypeName = "DATE")]
    public DateTime? LastCalving { get; set; }
    
    [Column(TypeName = "DATE")]
    public DateTime? LastInsemination { get; set; }

    [DefaultValue("")] public string Note { get; set; }

    public int FarmId { get; set; }

    [JsonIgnore] public Farm Farm { get; set; }

    [JsonIgnore] public List<Production> Production { get; set; }

    [Column(TypeName = "DATE")]
    [JsonIgnore]
    public DateTime? LazyCalculation { get; set; }

    [DefaultValue(0)] public double MeanProduction { get; set; }
}