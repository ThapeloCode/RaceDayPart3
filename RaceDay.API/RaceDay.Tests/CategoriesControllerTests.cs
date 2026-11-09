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

            // Assert (Unwrapping OkObjectResult from result.Result)
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var categories = Assert.IsAssignableFrom<IEnumerable<Category>>(okResult.Value);
            var categoryList = categories.ToList();

            Assert.Single(categoryList);
            Assert.Equal("10km Road Running", categoryList[0].CategoryName);
        }

        [Fact]
        public async Task GetCategory_ValidId_ReturnsCategory()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            dbContext.Categories.Add(new Category
            {
                CategoryId = 1,
                CategoryName = "21km Half Marathon"
            });
            await dbContext.SaveChangesAsync();

            var controller = new CategoriesController(dbContext);

            // Act
            var result = await controller.GetCategory(1);

            // Assert (Unwrapping OkObjectResult from result.Result)
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var category = Assert.IsType<Category>(okResult.Value);
            Assert.Equal(1, category.CategoryId);
            Assert.Equal("21km Half Marathon", category.CategoryName);
        }

        [Fact]
        public async Task GetCategory_InvalidId_ReturnsNotFound()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            var controller = new CategoriesController(dbContext);

            // Act
            var result = await controller.GetCategory(999);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreateCategory_SavesAndReturnsCreatedAtAction()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            var controller = new CategoriesController(dbContext);

            var newCategory = new Category
            {
                CategoryName = "40km Mountain Biking"
            };

            // Act
            var result = await controller.CreateCategory(newCategory);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var createdCategory = Assert.IsType<Category>(createdAtActionResult.Value);
            Assert.Equal("40km Mountain Biking", createdCategory.CategoryName);

            var categoryInDb = await dbContext.Categories.FirstOrDefaultAsync(c => c.CategoryName == "40km Mountain Biking");
            Assert.NotNull(categoryInDb);
        }

        [Fact]
        public async Task DeleteCategory_ValidId_RemovesCategoryAndReturnsNoContent()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            dbContext.Categories.Add(new Category
            {
                CategoryId = 1,
                CategoryName = "5km Fun Walk"
            });
            await dbContext.SaveChangesAsync();

            var controller = new CategoriesController(dbContext);

            // Act
            var result = await controller.DeleteCategory(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            var categoryInDb = await dbContext.Categories.FindAsync(1);
            Assert.Null(categoryInDb);
        }

        [Fact]
        public async Task DeleteCategory_InvalidId_ReturnsNotFound()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            var controller = new CategoriesController(dbContext);

            // Act
            var result = await controller.DeleteCategory(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}