using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ALMA_API.Middleware;
using ALMA_API.Models.Db;
using ALMA_API.Models.Requests;
using ALMA_API.Models.Responses;
using ALMA_API.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ALMA_API.Controllers
{
    [Route("api/Auth")]
    [ApiController]
    public class AuthController : BaseController
    {
        public AuthController(IConfiguration configuration, WebSocketConnectionManager connectionManager) : base(configuration, connectionManager) { }
        
        [AllowAnonymous]
        [HttpPut]
        [Route("register")]
        public ActionResult<AuthResponse> Register(RequestRegister requestRegister)
        {
            var responseRegister = new AuthResponse();
            using var db = new AppDbContext();

            if (db.User.Any(x => x.Email == requestRegister.Email))
            {
                responseRegister.MessageList.Add("Este Email já está em uso");
            }
            
            var farm = db.Farm.FirstOrDefault(farm=> farm.DeviceId == requestRegister.DeviceId);
            if (farm is null)
            {
                responseRegister.MessageList.Add("DeviceId não registrado");
            }

            if (responseRegister.MessageList.Count != 0) return responseRegister;
            var salt = CryptoUtil.GenerateSalt();
            var user = new User
            {
                Email = requestRegister.Email,
                Salt = salt,
                Password = CryptoUtil.HashMultiple(requestRegister.Password, salt),
                Role = "USER",
                Farm = farm!
            };
            db.User.Add(user);
            db.SaveChanges();

            return GetAuthResponseFromUser(user);
        }
        
        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        public ActionResult<BaseResponse> Login(RequestLogin requestLogin)
        {
            using var db = new AppDbContext();
            var existingUser = db.User.SingleOrDefault(x => x.Email == requestLogin.Email);
            if (existingUser != null)
            {
                var isPasswordVerified = CryptoUtil.VerifyPassword(requestLogin.Password, existingUser.Salt,
                    existingUser.Password);
                if (isPasswordVerified)
                {
                    return GetAuthResponseFromUser(existingUser);
                }
                return new BaseResponse("Senha incorreta");
            }
            return new BaseResponse("Email não encontrado");
        }
        
        [AllowAnonymous]
        [HttpPost]
        [Route("recover")]
        public ActionResult<BaseResponse> Recover(RequestRecover requestRecover)
        {
            var newPassword = CryptoUtil.GenerateTemporaryPassword();
            using (var db = new AppDbContext())
            {
                var user = db.User.FirstOrDefault(x => x.Email == requestRecover.Email);
                if (user == null)
                {
                    return new BaseResponse("Email não encontrado");
                }
                user.Salt = CryptoUtil.GenerateSalt();
                user.Password = CryptoUtil.HashMultiple(newPassword, user.Salt);
                user.ChangePassword = true;
                db.SaveChanges();
            }
            
            var (ok, err) =
                GoogleMail.SendMessage(requestRecover.Email, "Recuperar Email", $"A sua senha temporária é: {newPassword}");
            return ok ? new BaseResponse { Success = true } : new BaseResponse(err!);
        }
        
        [HttpPost]
        [Route("change")]
        public ActionResult<BaseResponse> Change(RequestChange requestChange)
        {
            using var db = new AppDbContext();
            var user = db.User.FirstOrDefault(x => x.Email == requestChange.Email);
            if (user == null)
            {
                return new BaseResponse("Email não encontrado");
            }

            if (!user.ChangePassword && !CryptoUtil.VerifyPassword(requestChange.OldPassword, user.Salt, user.Password))
            {
                return new BaseResponse("Senha antiga incorreta");
            }

            user.Salt = CryptoUtil.GenerateSalt();
            user.Password = CryptoUtil.HashMultiple(requestChange.NewPassword, user.Salt);
            user.ChangePassword = false;
            db.SaveChanges();
            return new BaseResponse {Success = true};
        }

        private AuthResponse GetAuthResponseFromUser(User existingUser)
        {
            var claimList = new List<Claim>
            {
                new(ClaimTypes.Name, existingUser.Email),
                new(ClaimTypes.Role, existingUser.Role),
                new(ClaimTypes.UserData, existingUser.Id.ToString())
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expireDate = DateTime.UtcNow.AddDays(60);
            var timeStamp = DateUtil.ConvertToTimeStamp(expireDate);
            var token = new JwtSecurityToken(
                claims: claimList,
                notBefore: DateTime.UtcNow,
                expires: expireDate,
                signingCredentials: creds);

            return new AuthResponse
            {
                Success = true,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpireDate = timeStamp
            };
        }

    }
}