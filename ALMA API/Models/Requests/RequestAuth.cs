using System.ComponentModel.DataAnnotations;
using System.Configuration;

namespace ALMA_API.Models.Requests;

public class RequestRecover : BaseRequest
{
    [Required] [EmailAddress] public string Email { get; set; }
}

public class RequestLogin : BaseRequest
{
    [Required] [EmailAddress] public string Email { get; set; }
    [Required] public string Password { get; set; }
}

public class RequestRegister : BaseRequest
{
    [Required] [EmailAddress] public string Email { get; set; }
    [Required] public string Password { get; set; }

    [Required]
    [StringValidator(MinLength = 2, MaxLength = 10)]
    public string DeviceId { get; set; }
}

public class RequestChange : BaseRequest
{
    [Required] [EmailAddress] public string Email { get; set; }
    public string OldPassword { get; set; }
    [Required] public string NewPassword { get; set; }
}