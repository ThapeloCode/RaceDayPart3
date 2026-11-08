using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Controllers;
using RaceDay.API.Data;
using RaceDay.API.Models;
using Xunit;

namespace RaceDay.Tests
{
    public class EnrolmentsControllerTests
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
        public async Task GetEnrolments_ReturnsAllEnrolments()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            dbContext.Enrolments.Add(new Enrolment
            {
                EnrolmentId = 1,
                UserId = 10,
                EventId = 100,
                CategoryId = 2,
                EnrolmentDate = DateTime.UtcNow
            });
            await dbContext.SaveChangesAsync();

            var controller = new EnrolmentsController(dbContext);

            // Act
            var result = await controller.GetEnrolments();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Enrolment>>>(result);
            var enrolments = Assert.IsAssignableFrom<IEnumerable<Enrolment>>(actionResult.Value);
            var enrolmentList = enrolments.ToList();

            Assert.Single(enrolmentList);
            Assert.Equal(10, enrolmentList[0].UserId);
        }
    }
}