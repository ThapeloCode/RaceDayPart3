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

        [Fact]
        public async Task Register_AddsUserSuccessfully()
        {
            // Arrange
            var dbContext = GetDatabaseContext();

            // Mock IConfiguration for JWT settings if required by AuthController
            var configurationMock = new Mock<IConfiguration>();

            var controller = new AuthController(dbContext, configurationMock.Object);

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
    }
}