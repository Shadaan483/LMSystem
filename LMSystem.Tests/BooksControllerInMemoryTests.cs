using System;
using System.Linq;
using System.Threading.Tasks;
using LMSystem.Controllers;
using LMSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LMSystem.Tests
{
    public class BooksControllerInMemoryTests
    {
        // Creates a fresh, isolated in-memory LibraryContext for each test.
        // LibraryContext.OnModelCreating seeds 4 books via HasData - since that
        // seed is part of the EF model itself (not a migration), it applies to
        // every database name, including in-memory ones. We clear it out so each
        // test starts from a known, empty Books13 table.
        private LibraryContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<LibraryContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new LibraryContext(options);
            context.Books13.RemoveRange(context.Books13);
            context.SaveChanges();

            return context;
        }

        private void SeedBooks(LibraryContext context, int count)
        {
            for (int i = 1; i <= count; i++)
            {
                context.Books13.Add(new Book
                {
                    Title = $"Test Book {i:D2}",
                    Author = $"Author {i:D2}",
                    ISBN = $"978-{i:D10}",
                    PublishedDate = new DateTime(2020, 1, 1),
                    IsAvailable = true
                });
            }

            context.SaveChanges();
        }

        [Fact]
        public async Task Index_FiltersBooks_WhenSearchQueryProvided()
        {
            // Arrange
            using var context = CreateContext();
            SeedBooks(context, 5);

            // Give exactly one seeded book a distinctive, searchable title
            var target = context.Books13.First();
            target.Title = "Mastering ASP.NET Core";
            context.SaveChanges();

            var controller = new BooksController(context);

            // Act
            var result = await controller.Index(searchQuery: "ASP.NET", page: 1) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = Assert.IsType<BookListViewModel>(result!.Model);
            Assert.Single(model.Books);
            Assert.Equal("Mastering ASP.NET Core", model.Books.First().Title);
        }

        [Fact]
        public async Task Index_ReturnsCorrectPageSize_ForPageOne()
        {
            // Arrange: 10 records, page size 5 -> page 1 should hold the first 5, 2 total pages
            using var context = CreateContext();
            SeedBooks(context, 10);
            var controller = new BooksController(context);

            // Act
            var result = await controller.Index(searchQuery: null, page: 1) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = Assert.IsType<BookListViewModel>(result!.Model);
            Assert.Equal(5, model.Books.Count());
            Assert.Equal(2, model.TotalPages);
            Assert.Equal(1, model.CurrentPage);
        }

        [Fact]
        public async Task Index_ReturnsCorrectPageSize_ForPageTwo()
        {
            // Arrange: 10 records, page size 5 -> page 2 should hold the remaining 5
            using var context = CreateContext();
            SeedBooks(context, 10);
            var controller = new BooksController(context);

            // Act
            var result = await controller.Index(searchQuery: null, page: 2) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = Assert.IsType<BookListViewModel>(result!.Model);
            Assert.Equal(5, model.Books.Count());
            Assert.Equal(2, model.TotalPages);
            Assert.Equal(2, model.CurrentPage);
        }
    }
}
