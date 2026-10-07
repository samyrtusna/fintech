using fintech.API.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace fintech.API.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "UserPolicy")]
    public class UserController(IUserService userService) : BaseController
    {
        [HttpGet("informations")]
        public async Task<IActionResult> GetUserInformations()
        {
            var userInformations = await userService.GetUserInformations(UserId);
            return Ok(userInformations);
        }
    }
}
