using fintech.API.Application.DTOs.ExchangeRateApiDtos;
using fintech.API.Application.Exceptions;
using fintech.API.Application.Interfaces.Repositories;
using fintech.API.Application.Interfaces.Services;
using fintech.API.Domain.Entities;
using fintech.API.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Net;
using System.Net.Http.Json;

namespace fintech.Tests.Infrastructure.Services
{
    public class ExchangeRateApiServiceTests
    {
        private readonly Mock<IEncryptionService> _encryptionService;
        private readonly Mock<IHttpClientFactory> _httpClientFactory;
        private readonly Mock<IOptionsRepository> _optionsRepository;
        private readonly IConfiguration _configuration;

        private ExchangeRateApiService CreateService(HttpMessageHandler handler)
        {
            var httpClient = new HttpClient(handler);

            _httpClientFactory
                .Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(httpClient);

            return new ExchangeRateApiService(
                _configuration,
                _encryptionService.Object,
                _httpClientFactory.Object,
                _optionsRepository.Object);
        }

        public ExchangeRateApiServiceTests()
        {
            var settings = new Dictionary<string, string?>
            {
                ["ExchangeRateAPI:BaseUrl"] = "https://api.example.com"
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(settings)
                .Build();

            _encryptionService = new Mock<IEncryptionService>();
            _httpClientFactory = new Mock<IHttpClientFactory>();
            _optionsRepository = new Mock<IOptionsRepository>();
        }

        // =========================================================
        // GetSymbolsAsync
        // =========================================================

        [Fact]
        public async Task GetSymbolsAsync_ValidResponse_ReturnsSymbols()
        {
            // Arrange
            var handler = new TestHttpMessageHandler(request =>
            {
                Assert.Equal(
                    "https://api.example.com/currencies",
                    request.RequestUri!.ToString());

                var data = new List<ExchangeRateSymbolsResponseDto>
                {
                    new() { IsoCode = "USD", Symbol = "United States Dollar" },
                    new() { IsoCode = "EUR", Symbol = "Euro" }
                };

                return CreateJsonResponse(data);
            });

            var service = CreateService(handler);

            // Act
            var result = await service.GetSymbolsAsync();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("United States Dollar", result["USD"]);
            Assert.Equal("Euro", result["EUR"]);
        }

        [Fact]
        public async Task GetSymbolsAsync_ApiReturnsNullData_ThrowsException()
        {
            // Arrange
            var handler = new TestHttpMessageHandler(_ => CreateJsonResponse((object?)null));
            var service = CreateService(handler);

            // Act
            var exception = await Assert.ThrowsAsync<Exception>(
                () => service.GetSymbolsAsync());

            // Assert
            Assert.Equal("API request was successful but returned no data.", exception.Message);
        }

        [Fact]
        public async Task GetSymbolsAsync_ApiReturnsNonSuccessStatus_ThrowsHttpRequestException()
        {
            // Arrange
            var handler = new TestHttpMessageHandler(_ =>
                new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    Content = new StringContent("Unauthorized access")
                });

            var service = CreateService(handler);

            // Act
            var exception = await Assert.ThrowsAsync<HttpRequestException>(
                () => service.GetSymbolsAsync());

            // Assert
            Assert.Contains("Failed to retrieve symbols.", exception.Message);
            Assert.Contains("Unauthorized", exception.Message);
        }

        // =========================================================
        // GetEchangeRateAsync
        // =========================================================

        [Fact]
        public async Task GetEchangeRateAsync_ValidResponse_ReturnsExchangeRate()
        {
            // Arrange
            var handler = new TestHttpMessageHandler(request =>
            {
                Assert.Equal(
                    "https://api.example.com/rates?base=EUR&quotes=USD",
                    request.RequestUri!.ToString());

                var data = new List<ExchangeRateResponseDto>
                {
                    new() { Rate = 1.08m }
                };

                return CreateJsonResponse(data);
            });

            var service = CreateService(handler);

            // Act
            var result = await service.GetEchangeRateAsync("EUR", "USD");

            // Assert
            Assert.Equal(1.08m, result);
        }

        [Fact]
        public async Task GetEchangeRateAsync_RateNotFound_ThrowsException()
        {
            // Arrange
            var handler = new TestHttpMessageHandler(_ => CreateJsonResponse(new List<ExchangeRateResponseDto>()));
            var service = CreateService(handler);

            // Act
            var exception = await Assert.ThrowsAsync<Exception>(
                () => service.GetEchangeRateAsync("EUR", "USD"));

            // Assert
            Assert.Equal("Rate for 'USD' was not found in the API response.", exception.Message);
        }

        [Fact]
        public async Task GetEchangeRateAsync_NonSuccessResponse_ThrowsHttpRequestExceptionWithErrorDetails()
        {
            // Arrange
            var handler = new TestHttpMessageHandler(_ =>
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("Invalid API request.")
                };
            });

            var service = CreateService(handler);

            // Act
            var exception = await Assert.ThrowsAsync<HttpRequestException>(
                () => service.GetEchangeRateAsync("EUR", "USD"));

            // Assert
            Assert.Contains("Failed to retrieve exchange rate (BadRequest)", exception.Message);
            Assert.Contains("Invalid API request.", exception.Message);
        }

        private static HttpResponseMessage CreateJsonResponse<T>(T content)
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(content)
            };
        }

        private sealed class TestHttpMessageHandler : HttpMessageHandler
        {
            private readonly Func<HttpRequestMessage, HttpResponseMessage> _handler;

            public TestHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> handler)
            {
                _handler = handler;
            }

            protected override Task<HttpResponseMessage> SendAsync(
                HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                return Task.FromResult(_handler(request));
            }
        }
    }
}