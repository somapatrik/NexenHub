using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NexenHub.Class;
using NexenHub.Models;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace NexenHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        GlobalDatabase dbglob = new GlobalDatabase();

        [HttpGet("login/{id}/{password}")]
        public ActionResult<bool> GetLogin(string id, string password)
        {
            return dbglob.Login(id, password);
        }

        [HttpGet("name/{id}")]
        public ActionResult<string> GetNameUser(string id)
        {
            string name = dbglob.GetNameUser(id);
            return name;
        }

        [HttpPost("cardLogin")]
        public async Task<ActionResult<Models.User>> PostCardLogin()
        {
            Stream body = HttpContext.Request.Body;
            string cardHex;

            StreamReader reader = new StreamReader(body, Encoding.UTF8);
            cardHex = await reader.ReadToEndAsync();
            
            return dbglob.CardLogin(cardHex);
        }

    }
}
