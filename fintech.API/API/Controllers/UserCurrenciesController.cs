using fintech.API.Application.DTOs.UserCurrenciesDtos;
using fintech.API.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace fintech.API.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "UserPolicy")]
    public class UserCurrenciesController(IUserCurrenciesService userCurrenciesService) : BaseController
    {
        [HttpPost]
        public async Task<IActionResult> AddUserCurrency( [FromBody] UserCurrenciesDto dto)
        {
            var newUserCurrency = await userCurrenciesService.AddUserCurrencyAsync(UserId, dto);
            return CreatedAtAction(nameof(GetUserCurrencies), newUserCurrency);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserCurrencies()
        {
            var userCurrencies = await userCurrenciesService.GetUserCurrenciesAsync(UserId);
            return Ok(userCurrencies);
            //TODO: this endpoint may be deleted in the future
        }
    }
}
