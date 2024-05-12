using EnsekEnergyManager.Classes;
using EnsekEnergyManager.Infrastructure.Persistence.Context;
using EnsekEnergyManager.Infrastructure.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace EnsekEnergyManager.Test
{
    [TestFixture]
    public class SeederTests
    {
        private Mock<ApplicationDbContext> _dbContextMock;
        private Mock<ILogger<AccountSeeder>> _loggerMock;
        private Mock<ILogger<MeterSeeder>> _loggerMeterMock;
        private AccountSeeder _seeder;
        private MeterSeeder _meterSeeder;

        Mock<DbSet<Account>> _mockAccounts = new();
        Mock<DbSet<MeterReading>> _mockMeterReadings = new();

        [SetUp]
        public void Setup()
        {
            _dbContextMock = new Mock<ApplicationDbContext>();
            _loggerMock = new Mock<ILogger<AccountSeeder>>();
            _loggerMeterMock = new Mock<ILogger<MeterSeeder>>();
            _seeder = new AccountSeeder(_loggerMock.Object, _dbContextMock.Object);
            _meterSeeder = new MeterSeeder(_loggerMeterMock.Object, _dbContextMock.Object);

            var accounts = new List<Account>
            {
                new Account { AccountId = 1, FirstName = "John", LastName = "Doe" },
                new Account { AccountId = 2, FirstName = "Jane", LastName = "Doe" }
            };

            IQueryable<Account> accountData = accounts.AsQueryable();

            _mockAccounts.As<IQueryable<Account>>().Setup(m => m.Provider).Returns(accountData.Provider);
            _mockAccounts.As<IQueryable<Account>>().Setup(m => m.Expression).Returns(accountData.Expression);
            _mockAccounts.As<IQueryable<Account>>().Setup(m => m.ElementType).Returns(accountData.ElementType);

            _mockAccounts.Setup(d => d.Add(It.IsAny<Account>())).Callback<Account>((s) => accounts.Add(s));

            var MeterReadings = new List<MeterReading>
            {
                new MeterReading { AccountId = 1, MeterReadingDateTime = DateTime.Now, MeterReadValue = "Test" },
                new MeterReading { AccountId = 2, MeterReadingDateTime = DateTime.Now, MeterReadValue = "Test" }
            };

            IQueryable<MeterReading> meterData = MeterReadings.AsQueryable();

            _mockMeterReadings.As<IQueryable<Account>>().Setup(m => m.Provider).Returns(accountData.Provider);
            _mockMeterReadings.As<IQueryable<Account>>().Setup(m => m.Expression).Returns(accountData.Expression);
            _mockMeterReadings.As<IQueryable<Account>>().Setup(m => m.ElementType).Returns(accountData.ElementType);

            _mockMeterReadings.Setup(d => d.Add(It.IsAny<MeterReading>())).Callback<MeterReading>((s) => MeterReadings.Add(s));
        }

        #region Account Tests

        [Test]
        public async Task SeedAccountsAsyncShouldSeedNewAccountsWhenMoviesAreMissing()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var movies = new List<AccountSeeder.AccountObject>
            {
                new AccountSeeder.AccountObject { AccountId = 1, FirstName = "John", LastName = "Doe" },
            };

            // Set up existing accounts in the database
            _dbContextMock.Setup(m => m.Set<Account>()).Returns(_mockAccounts.Object);

            // Act
            List<Account> addedAccounts = await _seeder.SeedAccountsAsync(movies, cancellationToken);

            // Assert
            Assert.That(addedAccounts.Count == 0);
        }

        [Test]
        public async Task SeedAccountsAsyncShouldNotSeedNewAccountsWhenMoviesAreAlreadyPresent()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var movies = new List<AccountSeeder.AccountObject>
            {
                new AccountSeeder.AccountObject { AccountId = 1, FirstName = "John", LastName = "Doe" },
                new AccountSeeder.AccountObject { AccountId = 2, FirstName = "Jane", LastName = "Doe" },
                new AccountSeeder.AccountObject { AccountId = 3, FirstName = "James", LastName = "Doe" }
            };

            // Set up existing accounts in the database
            _dbContextMock.Setup(m => m.Set<Account>()).Returns(_mockAccounts.Object);

            // Act
            List<Account> addedAccounts = await _seeder.SeedAccountsAsync(movies, cancellationToken);


            // Assert
            Assert.That(addedAccounts.Count == 1);
            Assert.That(addedAccounts.First().FirstName == "James");
        }

        [Test]
        public async Task SeedAccountsAsyncShouldReturnEmptyListWhenMoviesAreEmpty()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var movies = new List<AccountSeeder.AccountObject>();

            _dbContextMock.Setup(m => m.Set<Account>()).Returns(_mockAccounts.Object);


            // Act
            var result = await _seeder.SeedAccountsAsync(movies, cancellationToken);

            // Assert
            Assert.That(result.Count, Is.EqualTo(0)); // No accounts should be added
        }
        #endregion

        #region Reader Tests

        [Test]
        public async Task ParseMeterReadingsAsyncShouldReturnEmptyListWhenFileIsEmpty()
        {
            // Arrange
            var filePath = "empty_test_file.csv";

            // Act
            var meterReadings = await _meterSeeder.ParseMeterReadingsAsync(filePath);

            // Assert
            Assert.IsEmpty(meterReadings);
        }

        [Test]
        public async Task ParseMeterReadingsAsyncShouldReturnEmptyListWhenFilePathIsInvalid()
        {
            // Arrange
            var filePath = "invalid_file_path.csv"; // Invalid file path

            // Act
            var meterReadings = await _meterSeeder.ParseMeterReadingsAsync(filePath);

            // Assert
            Assert.IsNotNull(meterReadings);
            Assert.AreEqual(0, meterReadings.Count);
        }

        [Test]
        [Ignore("Not set up")]
        public async Task ParseMeterReadingsAsyncShouldParseCorrectlyFromValidCSVFile()
        {
            // Arrange
            var filePath = "valid_test_file.csv"; // To Do
            var expectedMeterReadingsCount = 2; // Expected number of meter readings in the valid CSV file

            // Act
            var meterReadings = await _meterSeeder.ParseMeterReadingsAsync(filePath);

            // Assert
            Assert.IsNotNull(meterReadings);
            Assert.AreEqual(expectedMeterReadingsCount, meterReadings.Count);
        }

        [Test]
        [Ignore("Not set up")]
        public async Task ParseMeterReadingsAsyncShouldParseOnlyFilesWithAssociatedAccountId()
        {
            // Arrange
            var filePath = "valid_test_file.csv"; // To Do
            var expectedMeterReadingsCount = 2; // Expected number of meter readings in the valid CSV file

            // Act
            var meterReadings = await _meterSeeder.ParseMeterReadingsAsync(filePath);

            // Assert
            Assert.IsNotNull(meterReadings);
            Assert.AreEqual(expectedMeterReadingsCount, meterReadings.Count);
        }
        #endregion
    }
}