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

            // Assert (Fixed)
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var events = Assert.IsAssignableFrom<IEnumerable<Event>>(okResult.Value);
            var eventList = events.ToList();

            Assert.Single(eventList);
            Assert.Equal("Pretoria Marathon", eventList[0].EventName);
        }

        [Fact]
        public async Task GetEvent_ValidId_ReturnsEvent()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            dbContext.Events.Add(new Event
            {
                EventId = 1,
                EventName = "Tshwane Cycling Challenge",
                EventType = "Cycling",
                Location = "Union Buildings",
                EventDate = DateTime.UtcNow.AddDays(15),
                OrganiserId = 2
            });
            await dbContext.SaveChangesAsync();

            var controller = new EventsController(dbContext);

            // Act
            var result = await controller.GetEvent(1);

            // Assert (Fixed)
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var eventItem = Assert.IsType<Event>(okResult.Value);
            Assert.Equal(1, eventItem.EventId);
            Assert.Equal("Tshwane Cycling Challenge", eventItem.EventName);
        }

        [Fact]
        public async Task GetEvent_InvalidId_ReturnsNotFound()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            var controller = new EventsController(dbContext);

            // Act
            var result = await controller.GetEvent(999);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetOrganiserEvents_ReturnsMatchingEvents()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            dbContext.Events.AddRange(
                new Event { EventId = 1, EventName = "Event A", OrganiserId = 5, Location = "Loc A", EventType = "Running" },
                new Event { EventId = 2, EventName = "Event B", OrganiserId = 5, Location = "Loc B", EventType = "Walking" },
                new Event { EventId = 3, EventName = "Event C", OrganiserId = 9, Location = "Loc C", EventType = "Cycling" }
            );
            await dbContext.SaveChangesAsync();

            var controller = new EventsController(dbContext);

            // Act
            var result = await controller.GetOrganiserEvents(5);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var events = Assert.IsAssignableFrom<IEnumerable<Event>>(okResult.Value);
            var eventList = events.ToList();

            Assert.Equal(2, eventList.Count);
            Assert.All(eventList, e => Assert.Equal(5, e.OrganiserId));
        }

        [Fact]
        public async Task CreateEvent_ValidOrganiser_ReturnsCreatedAtAction()
        {
            // Arrange
            var dbContext = GetDatabaseContext();

            // Seed Organiser User
            dbContext.Users.Add(new User
            {
                UserId = 1,
                Username = "OrganiserUser",
                Email = "org@raceday.com",
                PasswordHash = "hashed",
                Role = "Organiser"
            });
            await dbContext.SaveChangesAsync();

            var controller = new EventsController(dbContext);

            var newEvent = new Event
            {
                EventName = "Gauteng 10km Classic",
                EventType = "Running",
                Location = "Centurion",
                EventDate = DateTime.UtcNow.AddDays(20),
                OrganiserId = 1
            };

            // Act
            var result = await controller.CreateEvent(newEvent);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var createdEvent = Assert.IsType<Event>(createdAtActionResult.Value);
            Assert.Equal("Gauteng 10km Classic", createdEvent.EventName);

            var eventInDb = await dbContext.Events.FirstOrDefaultAsync(e => e.EventName == "Gauteng 10km Classic");
            Assert.NotNull(eventInDb);
        }

        [Fact]
        public async Task CreateEvent_InvalidOrganiser_ReturnsBadRequest()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            var controller = new EventsController(dbContext);

            var newEvent = new Event
            {
                EventName = "Orphaned Event",
                EventType = "Running",
                Location = "Pretoria",
                EventDate = DateTime.UtcNow.AddDays(5),
                OrganiserId = 999 // Non-existent user
            };

            // Act
            var result = await controller.CreateEvent(newEvent);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.NotNull(badRequestResult.Value);
        }

        [Fact]
        public async Task DeleteEvent_ValidId_RemovesEventAndReturnsNoContent()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            dbContext.Events.Add(new Event
            {
                EventId = 1,
                EventName = "Event to Delete",
                EventType = "Walking",
                Location = "Pretoria North",
                OrganiserId = 1
            });
            await dbContext.SaveChangesAsync();

            var controller = new EventsController(dbContext);

            // Act
            var result = await controller.DeleteEvent(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            var eventInDb = await dbContext.Events.FindAsync(1);
            Assert.Null(eventInDb);
        }

        [Fact]
        public async Task AddCategory_ValidEvent_AddsCategoryToEvent()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            dbContext.Events.Add(new Event
            {
                EventId = 1,
                EventName = "Trail Run Challenge",
                EventType = "Running",
                Location = "Magaliesburg",
                OrganiserId = 1
            });
            await dbContext.SaveChangesAsync();

            var controller = new EventsController(dbContext);

            var newCategory = new Category
            {
                CategoryName = "21km Trail"
            };

            // Act
            var result = await controller.AddCategory(1, newCategory);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var createdCategory = Assert.IsType<Category>(okResult.Value);
            Assert.Equal(1, createdCategory.EventId);

            var categoryInDb = await dbContext.Categories.FirstOrDefaultAsync(c => c.CategoryName == "21km Trail");
            Assert.NotNull(categoryInDb);
            Assert.Equal(1, categoryInDb.EventId);
        }
    }
}