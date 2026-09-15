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

        private ExchangeRateApiService CreateService(
            HttpMessageHandler handler)
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

        private void SetupApiKey()
        {
            var options = new Options
            {
                Key = "ExchangeRateApiKey",
                Value = "encrypted-api-key"
            };

            _optionsRepository
                .Setup(x => x.GetByKey("ExchangeRateApiKey"))
                .ReturnsAsync(options);

            _encryptionService
                .Setup(x => x.Decrypt("encrypted-api-key"))
                .Returns("test-api-key");
        }

        // =========================================================
        // GetSymbolsAsync
        // =========================================================

        [Fact]
        public async Task GetSymbolsAsync_ValidResponse_ReturnsSymbols()
        {
            // Arrange
            SetupApiKey();

            var handler = new TestHttpMessageHandler(request =>
            {
                Assert.Equal(
                    "https://api.example.com/symbols?access_key=test-api-key",
                    request.RequestUri!.ToString());

                return CreateJsonResponse(new
                {
                    success = true,
                    symbols = new Dictionary<string, string>
                    {
                        ["USD"] = "United States Dollar",
                        ["EUR"] = "Euro"
                    }
                });
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
        public async Task GetSymbolsAsync_ApiKeyIsLoadedAndDecrypted_UsesDecryptedKey()
        {
            // Arrange
            SetupApiKey();

            var handler = new TestHttpMessageHandler(request =>
            {
                Assert.Contains(
                    "access_key=test-api-key",
                    request.RequestUri!.Query);

                return CreateJsonResponse(new
                {
                    success = true,
                    symbols = new Dictionary<string, string>()
                });
            });

            var service = CreateService(handler);

            // Act
            await service.GetSymbolsAsync();

            // Assert
            _optionsRepository.Verify(
                x => x.GetByKey("ExchangeRateApiKey"),
                Times.Once);

            _encryptionService.Verify(
                x => x.Decrypt("encrypted-api-key"),
                Times.Once);
        }

        [Fact]
        public async Task GetSymbolsAsync_ApiKeyContainsSpecialCharacters_EscapesApiKey()
        {
            // Arrange
            var options = new Options
            {
                Key = "ExchangeRateApiKey",
                Value = "encrypted-api-key"
            };

            _optionsRepository
                .Setup(x => x.GetByKey("ExchangeRateApiKey"))
                .ReturnsAsync(options);

            _encryptionService
                .Setup(x => x.Decrypt("encrypted-api-key"))
                .Returns("key with special&characters");

            var handler = new TestHttpMessageHandler(request =>
            {
                Assert.Contains(
                    "access_key=key%20with%20special%26characters",
                    request.RequestUri!.Query);

                return CreateJsonResponse(new
                {
                    success = true,
                    symbols = new Dictionary<string, string>()
                });
            });

            var service = CreateService(handler);

            // Act
            await service.GetSymbolsAsync();
        }

        [Fact]
        public async Task GetSymbolsAsync_ApiReturnsSuccessFalse_ThrowsException()
        {
            // Arrange
            SetupApiKey();

            var handler = new TestHttpMessageHandler(_ =>
                CreateJsonResponse(new
                {
                    success = false,
                    symbols = new Dictionary<string, string>()
                }));

            var service = CreateService(handler);

            // Act
            var exception = await Assert.ThrowsAsync<Exception>(
                () => service.GetSymbolsAsync());

            // Assert
            Assert.Equal(
                "API request was successful but returned success = false.",
                exception.Message);
        }

        [Fact]
        public async Task GetSymbolsAsync_ApiReturnsNonSuccessStatus_ThrowsHttpRequestException()
        {
            // Arrange
            SetupApiKey();

            var handler = new TestHttpMessageHandler(_ =>
                new HttpResponseMessage(HttpStatusCode.Unauthorized));

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
            SetupApiKey();

            var handler = new TestHttpMessageHandler(request =>
            {
                Assert.Equal(
                    "https://api.example.com/latest?access_key=test-api-key&base=EUR&symbols=USD",
                    request.RequestUri!.ToString());

                return CreateJsonResponse(new
                {
                    success = true,
                    rates = new Dictionary<string, decimal>
                    {
                        ["USD"] = 1.08m
                    }
                });
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
            SetupApiKey();

            var handler = new TestHttpMessageHandler(_ =>
                CreateJsonResponse(new
                {
                    success = true,
                    rates = new Dictionary<string, decimal>
                    {
                        ["EUR"] = 1.00m
                    }
                }));

            var service = CreateService(handler);

            // Act
            var exception = await Assert.ThrowsAsync<Exception>(
                () => service.GetEchangeRateAsync("EUR", "USD"));

            // Assert
            Assert.Equal(
                "Rate for 'USD' was not found in the API response.",
                exception.Message);
        }

        [Fact]
        public async Task GetEchangeRateAsync_NonSuccessResponse_ThrowsHttpRequestExceptionWithErrorDetails()
        {
            // Arrange
            SetupApiKey();

            var handler = new TestHttpMessageHandler(_ =>
            {
                var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("Invalid API request.")
                };

                return response;
            });

            var service = CreateService(handler);

            // Act
            var exception = await Assert.ThrowsAsync<HttpRequestException>(
                () => service.GetEchangeRateAsync("EUR", "USD"));

            // Assert
            Assert.Contains(
                "Failed to retrieve exchange rate (BadRequest)",
                exception.Message);

            Assert.Contains(
                "Invalid API request.",
                exception.Message);
        }

        // =========================================================
        // API key caching
        // =========================================================

        [Fact]
        public async Task MultipleRequests_LoadApiKeyOnlyOnce()
        {
            // Arrange
            SetupApiKey();

            var requestCount = 0;

            var handler = new TestHttpMessageHandler(_ =>
            {
                requestCount++;

                return CreateJsonResponse(new
                {
                    success = true,
                    rates = new Dictionary<string, decimal>
                    {
                        ["USD"] = 1.10m
                    }
                });
            });

            var service = CreateService(handler);

            // Act
            await service.GetEchangeRateAsync("EUR", "USD");
            await service.GetEchangeRateAsync("EUR", "USD");

            // Assert
            Assert.Equal(2, requestCount);

            _optionsRepository.Verify(
                x => x.GetByKey("ExchangeRateApiKey"),
                Times.Once);

            _encryptionService.Verify(
                x => x.Decrypt("encrypted-api-key"),
                Times.Once);
        }

        [Fact]
        public async Task GetSymbolsAsync_MissingApiKey_ThrowsNotFoundException()
        {
            // Arrange
            _optionsRepository
                .Setup(x => x.GetByKey("ExchangeRateApiKey"))
                .ReturnsAsync((Options?)null);

            var handler = new TestHttpMessageHandler(_ =>
                throw new InvalidOperationException(
                    "HTTP request should not be made."));

            var service = CreateService(handler);

            // Act
            var exception = await Assert.ThrowsAsync<NotFoundException>(
                () => service.GetSymbolsAsync());

            // Assert
            Assert.Equal(
                "Exchange rate API key not found.",
                exception.Message);
        }

        private static HttpResponseMessage CreateJsonResponse(object content)
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(content)
            };
        }

        private sealed class TestHttpMessageHandler
            : HttpMessageHandler
        {
            private readonly Func<HttpRequestMessage, HttpResponseMessage> _handler;

            public TestHttpMessageHandler(
                Func<HttpRequestMessage, HttpResponseMessage> handler)
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