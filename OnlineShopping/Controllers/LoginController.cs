using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using OnlineShopping.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OnlineShopping.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "user")]

    public class LoginController : ControllerBase
    {

        private readonly IMongoCollection<Register> _User;

        public LoginController(IConfiguration configuration)
        {

            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("onlineshopping");
            _User = database.GetCollection<Register>("RegisteredUser");
        }
        /// <summary>
        /// user login
        /// </summary>
        /// <param name="loginDetails"></param>
        /// <returns></returns>

        [HttpPost("Login/user"), AllowAnonymous]
        public string Authenticate(Login loginDetails)
        {
            var user = _User.Find( x => x.LoginId == loginDetails.loginId).FirstOrDefault();
            if (user == null)
            {
                return null;
            }
            var tokenhandler = new JwtSecurityTokenHandler();

            var tokenKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("this is my custom Secret key for authentication"));

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Email, loginDetails.loginId),
                    new Claim(ClaimTypes.Role,user.isAdmin?"admin":"user"),
                }),
                Expires = DateTime.UtcNow.AddHours(1),

                SigningCredentials = new SigningCredentials(tokenKey,
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenhandler.CreateToken(tokenDescriptor);
            return tokenhandler.WriteToken(token);  
        }
       
    } 


}
