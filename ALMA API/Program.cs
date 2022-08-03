using System.Text;
using ALMA_API.Controllers;
using ALMA_API.Middleware;
using ALMA_API.Utils;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using WebSocketMiddleware = Microsoft.AspNetCore.WebSockets.WebSocketMiddleware;

DotEnv.Load(Path.Combine(Directory.GetCurrentDirectory(), "config.env"));
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddSingleton<WebSocketConnectionManager>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(builder.Configuration.GetSection("SecretKey").Value)),
        };
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                {
                    context.Response.Headers.Add("Token-Expired", "true");
                }
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                if (DateUtil.ExpireIn(context.SecurityToken.ValidTo, TimeSpan.FromDays(30)))
                {
                    context.Response.Headers.Add("Update-Token", "true");
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("ApiCorsPolicy");

//app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<AuthMiddleware>();
app.UseWebSockets(new WebSocketOptions()
{
    KeepAliveInterval = TimeSpan.FromSeconds(10) //TODO reduzir
});
app.UseMiddleware<WebSocketServerMiddleware>();
app.MapControllers();

app.Run();