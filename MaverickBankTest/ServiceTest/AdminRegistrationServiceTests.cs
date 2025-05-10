using MaverickBank.Contexts;
using MaverickBank.Models;
using MaverickBank.Models.DTOs;
using MaverickBank.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MaverickBankTest.ServiceTest
{
    [TestFixture]
    public class AdminRegistrationServiceTests
    {
        private DbContextOptions<MaverickBankContext> _options;
        private Mock<ILogger<AdminRegistrationService>> _loggerMock;

        [SetUp]
        public void Setup()
        {
            _options = new DbContextOptionsBuilder<MaverickBankContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _loggerMock = new Mock<ILogger<AdminRegistrationService>>();

            using var context = new MaverickBankContext(_options);
            context.Users.Add(new User
            {
                UserId = 1,
                Username = "existinguser",
                PasswordHash = "hashedpassword",
                Role = "Admin"
            });
            context.SaveChanges();
        }

        [Test]
        public async Task RegisterAdminAsync_ShouldRegisterAdmin_WhenValidDataIsProvided()
        {
            using var context = new MaverickBankContext(_options);
            var service = new AdminRegistrationService(context, _loggerMock.Object);

            var dto = new RegisterAdminDTO
            {
                Username = "newadmin",
                Password = "newpassword",
                FullName = "Test Admin",
                Email = "admin@example.com"
            };

            var response = await service.RegisterAdminAsync(dto);

            Assert.IsNotNull(response);
            Assert.AreEqual("newadmin", response.Username);
            Assert.AreEqual("Admin", response.Role);

            var user = await context.Users.FirstOrDefaultAsync(u => u.Username == "newadmin");
            Assert.IsNotNull(user);
            Assert.AreEqual("Admin", user.Role);

            var admin = await context.Admins.FirstOrDefaultAsync(a => a.UserId == user.UserId);
            Assert.IsNotNull(admin);
            Assert.AreEqual("Test Admin", admin.FullName);
            Assert.AreEqual("admin@example.com", admin.Email);
        }

        [Test]
        public void RegisterAdminAsync_ShouldThrowException_WhenUsernameAlreadyExists()
        {
            using var context = new MaverickBankContext(_options);
            var service = new AdminRegistrationService(context, _loggerMock.Object);

            var dto = new RegisterAdminDTO
            {
                Username = "existinguser", // Already seeded
                Password = "somepassword",
                FullName = "Test Admin 2",
                Email = "admin2@example.com"
            };

            var exception = Assert.ThrowsAsync<Exception>(async () => await service.RegisterAdminAsync(dto));
            Assert.AreEqual("Username already exists", exception.Message);
        }

        [Test]
        public async Task RegisterAdminAsync_ShouldCreateAdminWithCorrectTimestamp()
        {
            using var context = new MaverickBankContext(_options);
            var service = new AdminRegistrationService(context, _loggerMock.Object);

            var dto = new RegisterAdminDTO
            {
                Username = "newadminwithtimestamp",
                Password = "password123",
                FullName = "Timestamp Admin",
                Email = "timestampadmin@example.com"
            };

            var beforeSave = DateTime.Now;

            await service.RegisterAdminAsync(dto);

            var user = await context.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
            var admin = await context.Admins.FirstOrDefaultAsync(a => a.UserId == user.UserId);

            Assert.IsNotNull(admin);
            Assert.That(admin.FullName, Is.EqualTo(dto.FullName));
            Assert.That(admin.Email, Is.EqualTo(dto.Email));
            Assert.GreaterOrEqual(admin.CreatedAt, beforeSave);
            Assert.LessOrEqual(admin.CreatedAt, DateTime.Now.AddSeconds(1));
        }

        [Test]
        public async Task RegisterAdminAsync_ShouldHashPassword()
        {
            using var context = new MaverickBankContext(_options);
            var service = new AdminRegistrationService(context, _loggerMock.Object);

            var dto = new RegisterAdminDTO
            {
                Username = "hashcheck",
                Password = "plainpass",
                FullName = "Hash Test",
                Email = "hash@test.com"
            };

            await service.RegisterAdminAsync(dto);

            var user = await context.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
            Assert.IsNotNull(user);
            Assert.AreNotEqual("plainpass", user.PasswordHash);

            // Verify LogDebug was called (workaround for Moq + extension method issue)
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Debug,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Hashing password")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
                Times.Once
            );
        }

        [Test]
        public async Task RegisterAdminAsync_ShouldAllowSameEmailForDifferentUsers_IfNotRestricted()
        {
            using var context = new MaverickBankContext(_options);
            var service = new AdminRegistrationService(context, _loggerMock.Object);

            var dto1 = new RegisterAdminDTO
            {
                Username = "admin1",
                Password = "pass1",
                FullName = "Admin 1",
                Email = "duplicate@test.com"
            };

            var dto2 = new RegisterAdminDTO
            {
                Username = "admin2",
                Password = "pass2",
                FullName = "Admin 2",
                Email = "duplicate@test.com"
            };

            await service.RegisterAdminAsync(dto1);
            await service.RegisterAdminAsync(dto2);

            var admins = await context.Admins.Where(a => a.Email == "duplicate@test.com").ToListAsync();
            Assert.That(admins.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task RegisterAdminAsync_ShouldAssignAdminRoleAlways()
        {
            using var context = new MaverickBankContext(_options);
            var service = new AdminRegistrationService(context, _loggerMock.Object);

            var dto = new RegisterAdminDTO
            {
                Username = "rolecheck",
                Password = "adminpass",
                FullName = "Role Check",
                Email = "rolecheck@test.com"
            };

            var result = await service.RegisterAdminAsync(dto);

            var user = await context.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
            Assert.IsNotNull(user);
            Assert.That(user.Role, Is.EqualTo("Admin"));
            Assert.That(result.Role, Is.EqualTo("Admin"));
        }
    }
}
