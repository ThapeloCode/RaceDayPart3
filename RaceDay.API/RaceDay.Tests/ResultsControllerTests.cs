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

        [Fact]
        public async Task GetResult_ValidId_ReturnsResult()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            dbContext.Results.Add(new Result
            {
                ResultId = 2,
                UserId = 1,
                EventId = 1,
                CategoryId = 1,
                FinishTime = TimeSpan.FromHours(2),
                Position = 12
            });
            await dbContext.SaveChangesAsync();

            var controller = new ResultsController(dbContext);

            // Act
            var result = await controller.GetResult(2);

            // Assert (Fixed)
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var resultItem = Assert.IsType<Result>(okResult.Value);
            Assert.Equal(2, resultItem.ResultId);
            Assert.Equal(12, resultItem.Position);
        }

        [Fact]
        public async Task GetResult_InvalidId_ReturnsNotFound()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            var controller = new ResultsController(dbContext);

            // Act
            var result = await controller.GetResult(999);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetResultsByEvent_ReturnsSortedByFinishTime()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            dbContext.Results.AddRange(
                new Result { ResultId = 1, EventId = 10, UserId = 1, CategoryId = 1, FinishTime = TimeSpan.FromHours(3), Position = 3 },
                new Result { ResultId = 2, EventId = 10, UserId = 2, CategoryId = 1, FinishTime = TimeSpan.FromHours(1).Add(TimeSpan.FromMinutes(30)), Position = 1 },
                new Result { ResultId = 3, EventId = 10, UserId = 3, CategoryId = 1, FinishTime = TimeSpan.FromHours(2), Position = 2 }
            );
            await dbContext.SaveChangesAsync();

            var controller = new ResultsController(dbContext);

            // Act
            var result = await controller.GetResultsByEvent(10);

            // Assert (Fixed: unwrap OkObjectResult directly from result.Result)
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var results = Assert.IsAssignableFrom<IEnumerable<Result>>(okResult.Value);
            var resultList = results.ToList();

            Assert.Equal(3, resultList.Count);
            Assert.Equal(TimeSpan.FromHours(1).Add(TimeSpan.FromMinutes(30)), resultList[0].FinishTime);
            Assert.Equal(TimeSpan.FromHours(2), resultList[1].FinishTime);
            Assert.Equal(TimeSpan.FromHours(3), resultList[2].FinishTime);
        }

        [Fact]
        public async Task CreateResult_ValidForeignKeys_ReturnsCreatedAtAction()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            dbContext.Users.Add(new User { UserId = 1, Username = "Runner", Email = "r@ex.com", PasswordHash = "h", Role = "Participant" });
            dbContext.Events.Add(new Event { EventId = 1, EventName = "Event", EventType = "Run", Location = "Pretoria", OrganiserId = 2 });
            dbContext.Categories.Add(new Category { CategoryId = 1, CategoryName = "Cat", EventId = 1 });
            await dbContext.SaveChangesAsync();

            var controller = new ResultsController(dbContext);

            var newResult = new Result
            {
                UserId = 1,
                EventId = 1,
                CategoryId = 1,
                FinishTime = TimeSpan.FromMinutes(50),
                Position = 1
            };

            // Act
            var result = await controller.CreateResult(newResult);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var createdResult = Assert.IsType<Result>(createdAtActionResult.Value);
            Assert.Equal(1, createdResult.Position);

            var inDb = await dbContext.Results.FirstOrDefaultAsync(r => r.ResultId == createdResult.ResultId);
            Assert.NotNull(inDb);
        }

        [Fact]
        public async Task CreateResult_InvalidForeignKeys_ReturnsBadRequest()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            var controller = new ResultsController(dbContext);

            var invalidResult = new Result
            {
                UserId = 999, // Non-existent
                EventId = 999,
                CategoryId = 999,
                FinishTime = TimeSpan.FromMinutes(50),
                Position = 1
            };

            // Act
            var result = await controller.CreateResult(invalidResult);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.NotNull(badRequestResult.Value);
        }

        [Fact]
        public async Task DeleteResult_ValidId_RemovesResultAndReturnsNoContent()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            dbContext.Results.Add(new Result
            {
                ResultId = 1,
                UserId = 1,
                EventId = 1,
                CategoryId = 1,
                FinishTime = TimeSpan.FromMinutes(45),
                Position = 1
            });
            await dbContext.SaveChangesAsync();

            var controller = new ResultsController(dbContext);

            // Act
            var result = await controller.DeleteResult(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            var resultInDb = await dbContext.Results.FindAsync(1);
            Assert.Null(resultInDb);
        }

        [Fact]
        public async Task DeleteResult_InvalidId_ReturnsNotFound()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            var controller = new ResultsController(dbContext);

            // Act
            var result = await controller.DeleteResult(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}