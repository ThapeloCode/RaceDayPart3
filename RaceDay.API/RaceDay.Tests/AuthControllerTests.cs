using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using RaceDay.API.Controllers;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.Models;
using Xunit;

namespace RaceDay.Tests
{
    public class AuthControllerTests
    {
        private RaceDayDbContext GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<RaceDayDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var databaseContext = new RaceDayDbContext(options);
            databaseContext.Database.EnsureCreated();
            return databaseContext;
        }

        private IConfiguration GetMockConfiguration()
        {
            var configurationMock = new Mock<IConfiguration>();
            var jwtSectionMock = new Mock<IConfigurationSection>();

            jwtSectionMock.Setup(x => x["Key"]).Returns("SuperSecretKeyForRaceDayJwtAuthentication2026!");
            jwtSectionMock.Setup(x => x["Issuer"]).Returns("RaceDayAPI");
            jwtSectionMock.Setup(x => x["Audience"]).Returns("RaceDayUsers");

            configurationMock.Setup(c => c.GetSection("Jwt")).Returns(jwtSectionMock.Object);
            return configurationMock.Object;
        }

        [Fact]
        public async Task Register_AddsUserSuccessfully()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            var configuration = GetMockConfiguration();
            var controller = new AuthController(dbContext, configuration);

            var registerDto = new RegisterDto
            {
                Username = "testrunner",
                Email = "runner@example.com",
                Password = "SecurePassword123!",
                Role = "Participant"
            };

            // Act
            var result = await controller.Register(registerDto);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            var userInDb = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == "runner@example.com");

            Assert.NotNull(userInDb);
            Assert.Equal("testrunner", userInDb.Username);
        }

        [Fact]
        public async Task Register_DuplicateEmail_ReturnsBadRequest()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            var configuration = GetMockConfiguration();

            // Seed existing user
            dbContext.Users.Add(new User
            {
                Username = "existing",
                Email = "duplicate@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                Role = "Participant"
            });
            await dbContext.SaveChangesAsync();

            var controller = new AuthController(dbContext, configuration);

            var registerDto = new RegisterDto
            {
                Username = "newrunner",
                Email = "duplicate@example.com",
                Password = "SecurePassword123!",
                Role = "Participant"
            };

            // Act
            var result = await controller.Register(registerDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsToken()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            var configuration = GetMockConfiguration();

            // Seed user with hashed password
            string password = "SecurePassword123!";
            dbContext.Users.Add(new User
            {
                Username = "loginrunner",
                Email = "login@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role = "Participant"
            });
            await dbContext.SaveChangesAsync();

            var controller = new AuthController(dbContext, configuration);

            var loginDto = new LoginDto
            {
                Email = "login@example.com",
                Password = password
            };

            // Act
            var result = await controller.Login(loginDto);

            // Assert (Fixed: removed .Result to match IActionResult return type)
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            var configuration = GetMockConfiguration();

            var controller = new AuthController(dbContext, configuration);

            var loginDto = new LoginDto
            {
                Email = "nonexistent@example.com",
                Password = "WrongPassword123!"
            };

            // Act
            var result = await controller.Login(loginDto);

            // Assert (Fixed: removed .Result to match IActionResult return type)
            Assert.IsType<UnauthorizedObjectResult>(result);
        }
    }
}