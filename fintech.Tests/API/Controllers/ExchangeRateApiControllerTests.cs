using fintech.API.API.Controllers;
using fintech.API.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace fintech.Tests.API.Controllers
{
    public class ExchangeRateApiControllerTests
    {
        private readonly Mock<IExchangeRateApiService> _exchangeRateApiServiceMock;
        private readonly ExchangeRateApiController _controller;

        public ExchangeRateApiControllerTests()
        {
            _exchangeRateApiServiceMock =
                new Mock<IExchangeRateApiService>();

            _controller = new ExchangeRateApiController(
                _exchangeRateApiServiceMock.Object);
        }

        [Fact]
        public async Task GetExchangeRateSymbolsAsync_ReturnsOkWithSymbols()
        {
            var symbols = new Dictionary<string, string>
            {
                { "USD", "United States Dollar" },
                { "EUR", "Euro" },
                { "GBP", "British Pound" },
                { "DZD", "Algerian Dinar" }
            };

            _exchangeRateApiServiceMock
                .Setup(x => x.GetSymbolsAsync())
                .ReturnsAsync(symbols);

            var result =
                await _controller.GetExchangeRateSymbolsAsync();

            var okResult =
                Assert.IsType<OkObjectResult>(result.Result);

            Assert.Equal(200, okResult.StatusCode);
            Assert.Same(symbols, okResult.Value);

            _exchangeRateApiServiceMock.Verify(
                x => x.GetSymbolsAsync(),
                Times.Once);
        }
    }
}