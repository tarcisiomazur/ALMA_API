using ALMA_API.Middleware;
using Microsoft.AspNetCore.Mvc;

namespace ALMA_API.Controllers;

public abstract class BaseController : ControllerBase
{
    protected readonly IConfiguration Configuration;

    protected readonly WebSocketConnectionManager ConnectionManager;
    
    protected BaseController(IConfiguration configuration, WebSocketConnectionManager connectionManager)
    {
        Configuration = configuration;
        ConnectionManager = connectionManager;
    }
}