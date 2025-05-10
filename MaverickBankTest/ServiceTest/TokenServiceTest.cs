using MaverickBank.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace MaverickBankTest.ServiceTest
{
    [TestFixture]
    public class TokenServiceTest
    {
        private TokenService _tokenService;
        private Mock<ILogger<TokenService>> _mockLogger;
        private Mock<IConfiguration> _mockConfig;

        [SetUp]
        public void SetUp()
        {
            _mockLogger = new Mock<ILogger<TokenService>>(); // ✅ Initialize logger mock
            _mockConfig = new Mock<IConfiguration>();

            _mockConfig.Setup(config => config["Keys:JwtToken"])
                .Returns("This is a secret key for jwt token generation.");

            _tokenService = new TokenService(_mockConfig.Object, _mockLogger.Object);
        }

        [Test]
        public async Task GenerateToken_ShouldReturn_ValidToken()
        {
            // Act
            var token = await _tokenService.GenerateToken(1, "testuser", "Admin");

            // Assert
            Assert.IsNotNull(token, "Token should not be null");

            var handler = new JwtSecurityTokenHandler();
            var decodedToken = handler.ReadJwtToken(token);

            Assert.AreEqual("1", decodedToken.Claims.FirstOrDefault(c => c.Type == "nameid")?.Value);
            Assert.AreEqual("testuser", decodedToken.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value);
            Assert.AreEqual("Admin", decodedToken.Claims.FirstOrDefault(c => c.Type == "role")?.Value);

            _mockLogger.Verify(log =>
                log.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Generating token for user testuser with role Admin.")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);

            _mockLogger.Verify(log =>
                log.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Token generated successfully for user testuser.")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}
