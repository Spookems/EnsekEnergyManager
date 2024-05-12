using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EnsekEnergyManager.Infrastructure.Persistence.Context;
using EnsekEnergyManager.Controllers;
using EnsekEnergyManager.Infrastructure.Persistence.Init;
using Microsoft.Extensions.Logging;


namespace EnsekEnergyManager.Test
{
    [TestFixture]
    public class MeterReadingControllerTests
    {
        [TestFixture]
        public class AccountControllerTests
        {
            private Mock<CustomSeederRunner> _seederMock;
            private Mock<ApplicationDbContext> _dbContextMock;
            private Mock<ILogger<AccountController>> _loggerMock;
            private AccountController _controller;

            [SetUp]
            public void Setup()
            {
                _seederMock = new Mock<CustomSeederRunner>();
                _dbContextMock = new Mock<ApplicationDbContext>();
                _loggerMock = new Mock<ILogger<AccountController>>();

                _controller = new AccountController(_seederMock.Object, _dbContextMock.Object, _loggerMock.Object);
            }

            [Test]
            public async Task UploadMeterReadingsShouldReturnBadRequestIfFileExtensionIsNotCSV()
            {
                // Arrange
                var fileMock = new Mock<IFormFile>();
                fileMock.Setup(f => f.FileName).Returns("test.txt"); // Set file extension to .txt

                // Act
                var result = await _controller.UploadMeterReadings(fileMock.Object, CancellationToken.None) as BadRequestObjectResult;

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(400, result.StatusCode);
            }

            [Test]
            public async Task UploadMeterReadingsShouldReturnBadRequestIfFileExtensionIsNull()
            {
                // Arrange
                var fileMock = new Mock<IFormFile>();
                fileMock.Setup(f => f.FileName).Returns("test"); // Set file extension to null

                // Act
                var result = await _controller.UploadMeterReadings(fileMock.Object, CancellationToken.None) as BadRequestObjectResult;

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(400, result.StatusCode);
            }
        }
    }
}
