using ALMA_API.Middleware;
using ALMA_API.Models.Db;
using ALMA_API.Models.Requests;
using ALMA_API.Models.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ALMA_API.Controllers;

[Authorize]
[ApiController]
[Route("api/manageproduction")]
public class ManageProductionController : BaseController
{
    public ManageProductionController(IConfiguration configuration, WebSocketConnectionManager connectionManager) : base(configuration, connectionManager) { }

    [HttpGet]
    public ActionResult<BaseResponse> Get()
    {
        try
        {
            var userId = (int) (HttpContext.Items["id"] ?? 0);
            using var db = new AppDbContext();

            var px = (
                    from manage in db.ManageProduction
                    join user in db.User on manage.FarmId equals user.FarmId
                    where user.Id == userId
                    select manage
                ).Include(production => production.Stalls).ThenInclude(list => list.ProductionLeft)
                .Include(production => production.Stalls).ThenInclude(list => list.ProductionRight);
            var production = px.FirstOrDefault();
            if (production is null)
            {
                return new BaseResponse("ManageProduction não encontrada");
            }
            
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
                MessageList = new List<string> { ex.ToString() }
            };
        }
    }
    
    [HttpGet]
    [Route("field")]
    public ActionResult<BaseResponse> GetFields()
    {
        try
        {
            var userId = (int) (HttpContext.Items["id"] ?? 0);
            using var db = new AppDbContext();
            var mp = (
                from manage in db.ManageProduction
                join user in db.User on manage.FarmId equals user.FarmId
                select manage
            ).FirstOrDefault();
            
            var stalls = (
                from st in db.Stall
                where st.ManageProduction == mp
                select st
            ).Include(stall => stall.ProductionLeft).Include(stall => stall.ProductionRight).ToList();
            
            if (stalls is null)
            {
                return new BaseResponse("Campos não encontrados");
            }

            var payload = stalls.Select(stall => new
            {
                stall.ProductionLeft,
                stall.ProductionRight,
                stall.Side,
                stall.Index,
            }).ToList();

            return new AppResponse()
            {
                Success = true,
                Payload = payload
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

    [HttpPost]
    [Route("field")]
    public ActionResult<BaseResponse> SetFields(SetField setField)
    {
        try
        {
            var userId = (int) (HttpContext.Items["id"] ?? 0);
            using var db = new AppDbContext();
            var mp = (
                from manage in db.ManageProduction
                join user in db.User on manage.FarmId equals user.FarmId
                select manage
            ).FirstOrDefault();
            var field = (
                from stall in db.Stall
                where stall.ManageProduction == mp
                where stall.Index == setField.Index
                select stall
            ).FirstOrDefault();

            if (field is null)
            {
                db.Stall.Add(new Stall
                {
                    ProductionLeft = setField.LeftCowId == null? null : new Production()
                    {
                        CowId = setField.LeftCowId.Value
                    },
                    ProductionRight = setField.RightCowId == null? null : new Production()
                    {
                        CowId = setField.RightCowId.Value
                    },
                    Side = setField.Side,
                    Index = setField.Index,
                    ManageProduction = mp!
                });
            }
            else
            {
                field.Side = setField.Side;
                if(field.ProductionLeft?.CowId != setField.LeftCowId)
                {
                    if (setField.LeftCowId != null)
                    {
                        field.ProductionLeft = new Production()
                        {
                            CowId = setField.LeftCowId.Value
                        };
                    }
                    else
                    {
                        field.ProductionLeft = null;
                    }
                }

                if (field.ProductionRight?.CowId != setField.RightCowId)
                {
                    if (setField.RightCowId != null)
                    {
                        field.ProductionRight = new Production()
                        {
                            CowId = setField.RightCowId.Value
                        };
                    }
                    else
                    {
                        field.ProductionRight = null;
                    }
                }
            }

            db.SaveChanges();
            return new BaseResponse {Success = true};
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