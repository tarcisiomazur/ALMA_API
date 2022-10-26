using Microsoft.EntityFrameworkCore;

namespace ALMA_API.Models.Db;

[Index(nameof(DeviceId))]
public class Farm
{
    public int Id { get; set; }
    public string DeviceId { get; set; }
    
    public string PropertyName { get; set; }
    
    public ManageProduction ManageProduction { get; set; }
}