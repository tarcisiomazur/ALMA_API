using System.ComponentModel.DataAnnotations;
using System.Configuration;

namespace ALMA_API.Models.Requests;

public class SetField : BaseRequest
{
    [Required] public int Index { get; set; }
    public bool? Side { get; set; }
    [Required] public int? RightCowId { get; set; }
    [Required] public int? LeftCowId { get; set; }
}
