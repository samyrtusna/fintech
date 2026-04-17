using fintech.Application.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace fintech.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected int UserId
        {
            get
            {
                var claimValue = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if(!int.TryParse(claimValue, out int id))
                {
                    throw new BadRequestException("Invalid user ID in token.");
                }
                return id;
            }
        }
    }
}
