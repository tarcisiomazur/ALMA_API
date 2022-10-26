using System.Security.Claims;
using ALMA_API.Middleware;
using ALMA_API.Models.Db;
using ALMA_API.Models.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ALMA_API.Controllers;

[Authorize]
[ApiController]
[Route("api/userData")]
public class UserController : BaseController
{
    public UserController(IConfiguration configuration, WebSocketConnectionManager connectionManager) : base(configuration, connectionManager) { }
    
    [HttpGet]
    public ActionResult<BaseResponse> GetUserData()
    {
        try
        {
            using var db = new AppDbContext();
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.UserData), out var id))
                return new BaseResponse("Id do Usuário Incorreta");
            if (db.User.Find(id) is not { } user)
            {
                return Unauthorized(new BaseResponse("Usuário não encontrado"));
            }

            db.Entry(user).Reference(p => p.Farm).Load();
            return new AppResponse(user);

        }
        catch(Exception ex)
        {
            return new BaseResponse(ex.ToString());
        }
    }

    [AllowAnonymous]
    [HttpGet]
    [Route("/")]
    public ActionResult<DateTime> Index()
    {
        return DateTime.Now;
    }
}