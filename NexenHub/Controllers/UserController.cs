using Microsoft.AspNetCore.Mvc;
using NexenHub.Class;
using static NexenHub.Controllers.UtilityController;

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

    }
}
