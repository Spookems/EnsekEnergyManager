using NUnit.Framework;
using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CsvHelper;
using System.Globalization;
using System.Collections.Generic;
using EnsekEnergyManager.Infrastructure.Persistence.Context;
using EnsekEnergyManager.Classes;
using EnsekEnergyManager.Controllers;


namespace EnsekEnergyManager.Test
{

    [TestFixture]
    public class MeterReadingControllerTests
    {
        [Test]
        public async Task UploadMeterReadings_ValidCsv_ReturnsOk()
        {
            // Arrange
            var file = CreateFormFile("1,2024-05-12T10:00:00,100\n2,2024-05-12T11:00:00,200");

            var mockDb = new Mock<ApplicationDbContext>();
            mockDb.Setup(db => db.Accounts).Returns(Mock.Of<DbSet<Account>>(_ => _.Provider == Enumerable.Empty<Account>().AsQueryable().Provider && _.Expression == Enumerable.Empty<Account>().AsQueryable().Expression));

            var controller = new AccountController(mockDb.Object);
            var cancellationToken = new CancellationToken();

            // Act
            var result = await controller.UploadMeterReadings(file, cancellationToken);

            // Assert
            var okResult = Assert.IsInstanceOf<OkObjectResult>(result);
            var message = Assert.IsInstanceOf<string>(okResult.Value);
            Assert.That(message.Contains("Successfully processed"));
        }

        [Test]
        public async Task UploadMeterReadings_InvalidExtension_ReturnsBadRequest()
        {
            // Arrange
            var file = CreateFormFile("1,2024-05-12T10:00:00,100\n2,2024-05-12T11:00:00,200", "test.txt");

            var controller = new AccountController(Mock.Of<ApplicationDbContext>());
            var cancellationToken = new CancellationToken();

            // Act
            var result = await controller.UploadMeterReadings(file, cancellationToken);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        // Helper method to create a form file with specified content and name
        private static IFormFile CreateFormFile(string content, string fileName = "test.csv")
        {
            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            return new FormFile(memoryStream, 0, memoryStream.Length, "file", fileName);
        }
    }


    private static IFormFile CreateFormFile(string content, string fileName)
    {
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        return new FormFile(stream, 0, stream.Length, "file", fileName);
    }

    private static DbSet<Account> CreateMockDbSet(List<Account> data)
    {
        var queryableData = data.AsQueryable();
        var mockSet = new Mock<DbSet<Account>>();
        mockSet.As<IAsyncEnumerable<Account>>().Setup(m => m.GetEnumerator()).Returns(new TestAsyncEnumerator<Account>(queryableData.GetEnumerator()));
        mockSet.As<IQueryable<Account>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<Account>(queryableData.Provider));
        mockSet.As<IQueryable<Account>>().Setup(m => m.Expression).Returns(queryableData.Expression);
        mockSet.As<IQueryable<Account>>().Setup(m => m.ElementType).Returns(queryableData.ElementType);
        mockSet.As<IQueryable<Account>>().Setup(m => m.GetEnumerator()).Returns(() => queryableData.GetEnumerator());
        return mockSet.Object;
    }
}
