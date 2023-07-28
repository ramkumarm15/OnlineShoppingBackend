using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using OnlineShopping.Data;
using OnlineShopping.Models;

namespace OnlineShopping.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        
        private readonly IMongoCollection<Register> _User;

        public UserController(IConfiguration configuration)
        {
           
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("onlineshopping");
            _User = database.GetCollection<Register>("RegisteredUser");
        }

        /// <summary>
        /// Register User
        /// </summary>
        /// <param name="newuser"></param>

        [HttpPost("Register"), AllowAnonymous]
        public async Task<PrepareResponse> CreateUser(UserDto data)
        {
            var response = new PrepareResponse();
            if (await _User.Find(f => f.LoginId == data.LoginId).AnyAsync())
            {
                response.IsSuccess = false;
                response.Message = "username already exists";
                return response;
            }
            try
            {
                var user = new Register
                {
                    FirstName = data.FirstName,
                    LastName = data.LastName,
                    Email = data.Email,
                    LoginId = data.LoginId,
                    Password = data.Password,
                    ConformPassword = data.ConformPassword,
                    Contactnumber = data.Contactnumber,
                    isAdmin = false,
                };
                await _User.InsertOneAsync(user);
                response.IsSuccess = true;
                response.Message = "Data inserted";
                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                return response;
            }
        }
       
    }
}
