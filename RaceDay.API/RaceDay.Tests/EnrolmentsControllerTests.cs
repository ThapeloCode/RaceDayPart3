using System.Security.Claims;
using Microsoft.AspNetCore.Http;
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

        private static void SetAuthenticatedUser(EnrolmentsController controller, int userId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, "testuser@example.com")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };
        }

        [Fact]
        public async Task GetEnrolments_ReturnsAllEnrolments()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            dbContext.Events.Add(new Event { EventId = 100, EventName = "Pretoria Marathon", OrganiserId = 1 });
            dbContext.Categories.Add(new Category { CategoryId = 2, CategoryName = "10km Run", EventId = 100 });
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

            // Assert (Fixed: unwrap OkObjectResult or ActionResult Value correctly)
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var enrolments = Assert.IsAssignableFrom<IEnumerable<Enrolment>>(okResult.Value);
            var enrolmentList = enrolments.ToList();

            Assert.Single(enrolmentList);
            Assert.Equal(10, enrolmentList[0].UserId);
        }

        [Fact]
        public async Task GetEnrolment_ValidId_ReturnsEnrolment()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            dbContext.Events.Add(new Event { EventId = 100, EventName = "Pretoria Marathon", OrganiserId = 1 });
            dbContext.Categories.Add(new Category { CategoryId = 2, CategoryName = "10km Run", EventId = 100 });
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
            var result = await controller.GetEnrolment(1);

            // Assert (Fixed)
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var enrolment = Assert.IsType<Enrolment>(okResult.Value);
            Assert.Equal(1, enrolment.EnrolmentId);
            Assert.Equal(100, enrolment.EventId);
        }

        [Fact]
        public async Task GetEnrolment_InvalidId_ReturnsNotFound()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            var controller = new EnrolmentsController(dbContext);

            // Act
            var result = await controller.GetEnrolment(999);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreateEnrolment_BindsUserIdFromClaimsAndSaves()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            var controller = new EnrolmentsController(dbContext);

            // Mock authenticated User ID claim = 42
            SetAuthenticatedUser(controller, 42);

            var newEnrolment = new Enrolment
            {
                EventId = 100,
                CategoryId = 2
            };

            // Act
            var result = await controller.CreateEnrolment(newEnrolment);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var createdEnrolment = Assert.IsType<Enrolment>(createdAtActionResult.Value);

            Assert.Equal(42, createdEnrolment.UserId);
            Assert.Equal(100, createdEnrolment.EventId);

            var enrolmentInDb = await dbContext.Enrolments.FirstOrDefaultAsync(e => e.EnrolmentId == createdEnrolment.EnrolmentId);
            Assert.NotNull(enrolmentInDb);
            Assert.Equal(42, enrolmentInDb.UserId);
        }

        [Fact]
        public async Task DeleteEnrolment_ValidId_RemovesEnrolmentAndReturnsNoContent()
        {
            // Arrange
            var dbContext = GetDatabaseContext();

            // Seed parent records (Fixed: using EventName instead of Title)
            dbContext.Events.Add(new Event { EventId = 100, EventName = "Pretoria Marathon", OrganiserId = 1 });
            dbContext.Categories.Add(new Category { CategoryId = 2, CategoryName = "10km Run", EventId = 100 });

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
            var result = await controller.DeleteEnrolment(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            var enrolmentInDb = await dbContext.Enrolments.FindAsync(1);
            Assert.Null(enrolmentInDb);
        }

        [Fact]
        public async Task DeleteEnrolment_InvalidId_ReturnsNotFound()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            var controller = new EnrolmentsController(dbContext);

            // Act
            var result = await controller.DeleteEnrolment(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}