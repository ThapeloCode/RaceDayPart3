using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Controllers;
using RaceDay.API.Data;
using RaceDay.API.Models;
using Xunit;

namespace RaceDay.Tests
{
    public class EventsControllerTests
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
        public async Task GetEvents_ReturnsAllEvents()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            dbContext.Events.Add(new Event
            {
                EventId = 1,
                EventName = "Pretoria Marathon",
                EventType = "Running",
                Location = "Pretoria CBD",
                EventDate = DateTime.UtcNow.AddDays(10),
                OrganiserId = 1
            });
            await dbContext.SaveChangesAsync();

            var controller = new EventsController(dbContext);

            // Act
            var result = await controller.GetEvents();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Event>>>(result);
            var events = Assert.IsAssignableFrom<IEnumerable<Event>>(actionResult.Value);
            var eventList = events.ToList();

            Assert.Single(eventList);
            Assert.Equal("Pretoria Marathon", eventList[0].EventName);
        }
    }
}