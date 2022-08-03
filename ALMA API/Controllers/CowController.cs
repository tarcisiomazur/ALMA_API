using System.ComponentModel.DataAnnotations;
using ALMA_API.Middleware;
using ALMA_API.Models.Db;
using ALMA_API.Models.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ALMA_API.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace ALMA_API.Controllers;

[Authorize]
[ApiController]
[Route("api/cow")]
public class CowController : BaseController
{
    public CowController(IConfiguration configuration, WebSocketConnectionManager connectionManager) : base(configuration, connectionManager) { }


    [HttpGet]
    [Route("getAll")]
    public ActionResult<BaseResponse> GetAllCows()
    {
        try
        {
            using var db = new AppDbContext();
            if (HttpContext.Items["id"] is not int id)
            {
                return new BaseResponse("Id do Usuário Incorreta");
            }

            var user = db.User.Find(id);
            if (user == null)
            {
                return new BaseResponse("Usuário não encontrado");
            }

            var list = db.Cow.Where(cow => cow.deathDate == null && cow.FarmId == user.FarmId).ToList();
            ConnectionManager.SendToAllClients(id, "hi");
            return new AppResponse()
            {
                Success = true,
                Payload = list
            };
        }
        catch (Exception ex)
        {
            return new AppResponse()
            {
                MessageList = new List<string> { ex.ToString() }
            };
        }
    }

    [HttpDelete]
    [Route("remove")]
    public ActionResult<AppResponse> RemoveCow([Required] int id)
    {
        try
        {
            using var db = new AppDbContext();
            var list = db.Cow.Where(cow => cow.deathDate.IsNull()).ToList();
            return new AppResponse()
            {
                Success = true,
                Payload = list
            };
        }
        catch(Exception ex)
        {
            return new AppResponse()
            {
                MessageList = new List<string>{ex.ToString()}
            };
        }
    }

    [HttpPost]
    [Route("update")]
    public ActionResult<AppResponse> UpdateCow()
    {
        try
        {
            using var db = new AppDbContext();
            var list = db.Cow.Where(cow => cow.deathDate.IsNull()).ToList();
            return new AppResponse()
            {
                Success = true,
                Payload = list
            };
        }
        catch(Exception ex)
        {
            return new AppResponse()
            {
                MessageList = new List<string>{ex.ToString()}
            };
        }
    }
    
    [HttpPut]
    [Route("create")]
    public ActionResult<AppResponse> CreateCow()
    {
        try
        {
            using var db = new AppDbContext();
            var list = db.Cow.Where(cow => cow.deathDate.IsNull()).ToList();
            return new AppResponse()
            {
                Success = true,
                Payload = list
            };
        }
        catch(Exception ex)
        {
            return new AppResponse()
            {
                MessageList = new List<string>{ex.ToString()}
            };
        }
    }
}