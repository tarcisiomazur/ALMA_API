using Microsoft.AspNetCore.Mvc;

namespace ALMA_API.Controllers;

public abstract class BaseController : ControllerBase
{
    protected readonly IConfiguration Configuration;

    protected BaseController(IConfiguration configuration) {
        Configuration = configuration;
    }
}