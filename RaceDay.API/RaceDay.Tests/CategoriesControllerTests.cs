using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Controllers;
using RaceDay.API.Data;
using RaceDay.API.Models;
using Xunit;

namespace RaceDay.Tests
{
    public class CategoriesControllerTests
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
        public async Task GetCategories_ReturnsAllCategories()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            dbContext.Categories.Add(new Category
            {
                CategoryId = 1,
                CategoryName = "10km Road Running"
            });
            await dbContext.SaveChangesAsync();

            var controller = new CategoriesController(dbContext);

            // Act
            var result = await controller.GetCategories();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Category>>>(result);
            var categories = Assert.IsAssignableFrom<IEnumerable<Category>>(actionResult.Value);
            var categoryList = categories.ToList();

            Assert.Single(categoryList);
            Assert.Equal("10km Road Running", categoryList[0].CategoryName);
        }
    }
}