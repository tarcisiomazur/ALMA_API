using System.ComponentModel.DataAnnotations;
using ALMA_API.Middleware;
using ALMA_API.Models.Db;
using ALMA_API.Models.Requests;
using ALMA_API.Models.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ALMA_API.Controllers;

[Authorize]
[ApiController]
[Route("api/cow")]
public class CowController : BaseController
{
    private const int PageLength = 5;
    public CowController(IConfiguration configuration, WebSocketConnectionManager connectionManager) : base(configuration, connectionManager) { }
    
    
    
    [HttpGet]
    [Route("cowsReport")]
    public ActionResult<BaseResponse> GetCowsReport()
    {
        try
        {
            using var db = new AppDbContext();
            if (HttpContext.Items["id"] is not int userId)
            {
                return new BaseResponse("Id do Usuário Incorreta");
            }

            var cows = (
                from cow in db.Cow
                join user in db.User on cow.FarmId equals user.FarmId
                where user.Id == userId
                where cow.State != CowState.Death
                group cow by cow.State into cowList
                select new
                {
                    State = cowList.Key,
                    Count = cowList.Count()
                }
            ).ToList();
            for (int i = 0; i < 5; i++)
            {
                if (cows.All(x => x.State != (CowState) i))
                {
                    cows.Add(new
                    {
                        State = (CowState) i,
                        Count = 0,
                    });
                }
            }
            return new AppResponse(cows);
        }
        catch (Exception ex)
        {
            return new AppResponse()
            {
                MessageList = new List<string> {ex.ToString()}
            };
        }
    }
    
    [HttpGet]
    [Route("allIds")]
    public ActionResult<BaseResponse> GetAllCowIds()
    {
        try
        {
            using var db = new AppDbContext();
            if (HttpContext.Items["id"] is not int userId)
            {
                return new BaseResponse("Id do Usuário Incorreta");
            }

            var cows = (
                from cow in db.Cow
                join user in db.User on cow.FarmId equals user.FarmId
                where user.Id == userId
                where cow.State != CowState.Death
                orderby cow.Identification
                select new
                {
                    cow.Id,
                    cow.Identification
                }
            ).ToList();
            return new AppResponse(cows);
        }
        catch (Exception ex)
        {
            return new AppResponse()
            {
                MessageList = new List<string> {ex.ToString()}
            };
        }
    }
    
    [HttpGet]
    [Route("all")]
    public ActionResult<BaseResponse> GetAllCows([Required]int page)
    {
        try
        {
            using var db = new AppDbContext();
            if (HttpContext.Items["id"] is not int userId)
            {
                return new BaseResponse("Id do Usuário Incorreta");
            }

            db.CowsWithUpdateMeanProduction(userId);
            var cows = (
                from cow in db.Cow
                join user in db.User on cow.FarmId equals user.FarmId
                where user.Id == userId
                orderby cow.Identification
                select cow
            ).Skip((page - 1) * PageLength).Take(PageLength).ToList();
            return new PagedResponse()
            {
                Success = true,
                Payload = cows,
                EndPage = cows.Count != PageLength
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
    
    [HttpGet]
    public ActionResult<BaseResponse> GetCow([Required]int id, string? status, string? filter)
    {
        try
        {
            var userId = (int) (HttpContext.Items["id"] ?? 0);
            using var db = new AppDbContext();

            var firstCow = (
                from cow in db.Cow
                join user in db.User on cow.FarmId equals user.FarmId
                where user.Id == userId
                where cow.Id == id
                select cow
            ).Take(1).FirstOrDefault();
            
            if (firstCow is null)
            {
                return new BaseResponse("Animal não encontrado");
            }

            return new AppResponse()
            {
                Success = true,
                Payload = firstCow
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
    public ActionResult<BaseResponse> RemoveCow([Required] int id)
    {
        try
        {
            var userId = (int) (HttpContext.Items["id"] ?? 0);
            using var db = new AppDbContext();

            var firstCow = (
                from cow in db.Cow
                join user in db.User on cow.FarmId equals user.FarmId
                where user.Id == userId
                where cow.Id == id
                select cow
            ).Take(1).FirstOrDefault();
            
            if (firstCow is null)
            {
                return new BaseResponse("Animal não encontrado");
            }

            db.Cow.Remove(firstCow);
            try
            {
                db.SaveChanges();
                ConnectionManager.SendToAllClients(userId, "update cows");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new BaseResponse(ex.ToString());
            }

            return new AppResponse()
            {
                Success = true,
                Payload = firstCow
            };
        }
        catch (Exception ex)
        {
            return new AppResponse()
            {
                MessageList = new List<string> {ex.ToString()}
            };
        }
    }

    [HttpPut]
    public ActionResult<BaseResponse> UpdateCow(RequestUpdateCow requestCow)
    {
        try
        {
            using var db = new AppDbContext();
            var cow = db.Cow.Find(requestCow.Id);
            if (cow is null)
            {
                return new BaseResponse("Id não encontrado");
            }

            requestCow.Merge(db.Entry(cow));
            db.SaveChanges();
            return new AppResponse()
            {
                Success = true,
                Payload = cow
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
    public ActionResult<AppResponse> CreateCow(RequestCreateCow requestCow)
    {
        try
        {
            var db = new AppDbContext();
            var cow = requestCow.Merge(db.Cow.Add(new Cow()));
            db.SaveChanges();
            return new AppResponse()
            {
                Success = true,
                Payload = cow
            };
        }
        catch (Exception ex)
        {
            return new AppResponse()
            {
                MessageList = new List<string> {ex.ToString()}
            };
        }
    }
}