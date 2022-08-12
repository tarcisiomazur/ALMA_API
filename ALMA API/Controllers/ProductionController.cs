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
[Route("api/production")]
public class ProductionController : BaseController
{
    public ProductionController(IConfiguration configuration, WebSocketConnectionManager connectionManager) : base(configuration, connectionManager) { }

    [HttpGet]
    [Route("farm/")]
    public ActionResult<BaseResponse> GetProdutionFarm(int? days)
    {
        try
        {
            using var db = new AppDbContext();
            if (HttpContext.Items["id"] is not int userId)
            {
                return new BaseResponse("Id do Usuário Incorreta");
            }

            var since = DateTime.Today.Subtract(TimeSpan.FromDays(days??30));
            
            var productions = (
                from production in db.Production
                join cow in db.Cow on production.CowId equals cow.Id
                join user in db.User on cow.FarmId equals user.FarmId
                where user.Id == userId
                where production.Time.Date > since.Date
                group production by production.Time.Date into farmProduction
                select new
                {
                    Time = farmProduction.Key,
                    Quantity = farmProduction.Sum(p=> p.Quantity),
                }
            ).ToList();
            return new FarmProductionResponse()
            {
                Success = true,
                Since = since.Date,
                Payload = productions
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
    public ActionResult<BaseResponse> GetProduction([Required]int id)
    {
        try
        {
            var userId = (int) (HttpContext.Items["id"] ?? 0);
            using var db = new AppDbContext();

            var firstProduction = (
                from production in db.Production
                join cow in db.Cow on production.CowId equals cow.Id
                join user in db.User on cow.FarmId equals user.FarmId
                where user.Id == userId
                where production.Id == id
                select production
            ).Take(1).FirstOrDefault();
            
            if (firstProduction is null)
            {
                return new BaseResponse("Produção não encontrada");
            }

            return new AppResponse()
            {
                Success = true,
                Payload = firstProduction
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
    public ActionResult<BaseResponse> RemoveProduction([Required] int id)
    {
        try
        {
            var userId = (int) (HttpContext.Items["id"] ?? 0);
            using var db = new AppDbContext();

            var firstProduction = (
                from production in db.Production
                join cow in db.Cow on production.CowId equals cow.Id
                join user in db.User on cow.FarmId equals user.FarmId
                where user.Id == userId
                where production.Id == id
                select production
            ).Take(1).FirstOrDefault();
            
            if (firstProduction is null)
            {
                return new BaseResponse("Produção não encontrada");
            }

            db.Production.Remove(firstProduction);
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
                Payload = firstProduction
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
    public ActionResult<BaseResponse> UpdateProduction(RequestUpdateProduction requestProduction)
    {
        try
        {
            using var db = new AppDbContext();
            var production = db.Production.Find(requestProduction.Id);
            if (production is null)
            {
                return new BaseResponse("Id não encontrado");
            }

            requestProduction.Merge(db.Entry(production));
            db.SaveChanges();
            return new AppResponse()
            {
                Success = true,
                Payload = production
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
    public ActionResult<AppResponse> CreateProduction(RequestUpdateProduction requestProduction)
    {
        try
        {
            var db = new AppDbContext();
            var production = requestProduction.Merge(db.Production.Add(new Production()));
            db.SaveChanges();
            return new AppResponse()
            {
                Success = true,
                Payload = production
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