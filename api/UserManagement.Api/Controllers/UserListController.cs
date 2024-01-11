using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="Admin")]
    public class UserListController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<string>>> Get()
        {
            var userlist= await Task.FromResult(new string[] { "Virat", "Messi", "Ozil", "Lara", "MS Dhoni" }.ToList());
            return Ok(userlist);
        }
    }
}
