using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Controllers;
using RaceDay.API.Data;
using RaceDay.API.Models;
using Xunit;

namespace RaceDay.Tests
{
    public class ResultsControllerTests
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
        public async Task GetResults_ReturnsAllResults()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            dbContext.Results.Add(new Result
            {
                ResultId = 1,
                UserId = 1,
                EventId = 1,
                CategoryId = 1,
                FinishTime = TimeSpan.FromHours(1).Add(TimeSpan.FromMinutes(45)),
                Position = 5,
                User = new User { UserId = 1, Username = "TestUser", Email = "test@example.com", PasswordHash = "hash", Role = "Participant" },
                Event = new Event { EventId = 1, EventName = "Pretoria Marathon", EventType = "Running", Location = "Pretoria CBD", EventDate = DateTime.UtcNow, OrganiserId = 1 },
                Category = new Category { CategoryId = 1, CategoryName = "10km Road Running" }
            });
            await dbContext.SaveChangesAsync();

            var controller = new ResultsController(dbContext);

            // Act
            var result = await controller.GetResults();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Result>>>(result);
            var results = Assert.IsAssignableFrom<IEnumerable<Result>>(actionResult.Value);
            var resultList = results.ToList();

            Assert.Single(resultList);
            Assert.Equal(5, resultList[0].Position);
        }
    }
}