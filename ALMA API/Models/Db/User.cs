using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace ALMA_API.Models.Db;

[Index(nameof(Email), IsUnique = true)]
public class User
{
    [JsonIgnore]
    public int Id { get; set; }
    public string Email { get; set; }
    [JsonIgnore]
    public string Salt { get; set; }
    [JsonIgnore]
    public string Password { get; set; }
    public bool ChangePassword { get; set; }
    public string Role { get; set; }
    [JsonIgnore]
    public int FarmId { get; set; }
    public Farm Farm { get; set; }
}
