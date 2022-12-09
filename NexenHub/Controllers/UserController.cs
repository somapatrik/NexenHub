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
