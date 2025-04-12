using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xunit;
using Moq;
using AutoMapper;
using CarRentXpress.Application.Services;
using CarRentXpress.Application.Entities;
using CarRentXpress.Data.Entities;
using CarRentXpress.Core.Repositories;

namespace CarRentXpress.Tests.Services
{
    public class CarRentServiceTests
    {
        private readonly Mock<IRepository<CarRent>> _mockCarRentRepository;
        private readonly Mock<IRepository<Car>> _mockCarRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly CarRentService _carRentService;

        public CarRentServiceTests()
        {
            _mockCarRentRepository = new Mock<IRepository<CarRent>>();
            _mockCarRepository = new Mock<IRepository<Car>>();
            _mockMapper = new Mock<IMapper>();

            _carRentService = new CarRentService(
                _mockCarRentRepository.Object,
                _mockCarRepository.Object,
                _mockMapper.Object);
        }

        [Fact]
        public async Task RentCarAsync_WithValidCar_RentsCarSuccessfully()
        {
            // Arrange
            var carRentDto = new CarRentDto 
            { 
                CarId = "Car123", 
                StartDate = DateTime.Today, 
                EndDate = DateTime.Today.AddDays(3) 
            };
            var mappedCarRent = new CarRent 
            { 
                CarId = "Car123", 
                StartDate = carRentDto.StartDate, 
                EndDate = carRentDto.EndDate 
            };

            _mockMapper.Setup(m => m.Map<CarRent>(carRentDto))
                .Returns(mappedCarRent);

            var car = new Car { Id = "Car123" };
            _mockCarRepository
                .Setup(repo => repo.GetAsync(It.IsAny<List<Expression<Func<Car, bool>>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(car);

            await _carRentService.RentCarAsync(carRentDto);

            _mockCarRentRepository.Verify(repo => repo.CreateAsync(mappedCarRent, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateCarRentAsync_WithExistingCarRent_UpdatesSuccessfully()
        {
            // Arrange
            var carRentDto = new CarRentDto 
            { 
                Id = "Rent1", 
                CarId = "Car123", 
                StartDate = DateTime.Today, 
                EndDate = DateTime.Today.AddDays(5) 
            };
            var existingCarRent = new CarRent 
            { 
                Id = "Rent1", 
                CarId = "Car123", 
                StartDate = DateTime.Today, 
                EndDate = DateTime.Today.AddDays(3) 
            };
            _mockCarRentRepository
                .Setup(repo => repo.GetAsync(It.IsAny<List<Expression<Func<CarRent, bool>>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingCarRent);

            _mockMapper.Setup(m => m.Map(carRentDto, existingCarRent))
                .Callback(() => existingCarRent.EndDate = carRentDto.EndDate);

            await _carRentService.UpdateCarRentAsync(carRentDto);

            _mockMapper.Verify(m => m.Map(carRentDto, existingCarRent), Times.Once);
            _mockCarRentRepository.Verify(repo => repo.UpdateAsync(existingCarRent, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateCarRentAsync_WithNonExistingCarRent_ThrowsInvalidOperationException()
        {
            // Arrange
            var carRentDto = new CarRentDto { Id = "NonExistentRent", CarId = "Car123", StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(5) };

            _mockCarRentRepository
                .Setup(repo => repo.GetAsync(It.IsAny<List<Expression<Func<CarRent, bool>>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((CarRent)null);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _carRentService.UpdateCarRentAsync(carRentDto));
            Assert.Contains("CarRent with Id", ex.Message);
        }

        [Fact]
        public async Task GetCarRentByIdAsync_WhenCarRentExists_ReturnsMappedDto()
        {
            // Arrange
            var carRent = new CarRent 
            { 
                Id = "Rent1", 
                CarId = "Car123", 
                StartDate = DateTime.Today, 
                EndDate = DateTime.Today.AddDays(3) 
            };
            var expectedDto = new CarRentDto 
            { 
                Id = "Rent1", 
                CarId = "Car123", 
                StartDate = carRent.StartDate, 
                EndDate = carRent.EndDate 
            };

            _mockCarRentRepository
                .Setup(repo => repo.GetAsync(It.IsAny<List<Expression<Func<CarRent, bool>>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(carRent);

            _mockMapper.Setup(m => m.Map<CarRentDto>(carRent))
                .Returns(expectedDto);

            var result = await _carRentService.GetCarRentByIdAsync("Rent1");

            Assert.NotNull(result);
            Assert.Equal(expectedDto.Id, result.Id);
        }

        [Fact]
        public async Task GetCarRentByIdAsync_WhenCarRentDoesNotExist_ReturnsNull()
        {
            
            _mockCarRentRepository
                .Setup(repo => repo.GetAsync(It.IsAny<List<Expression<Func<CarRent, bool>>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((CarRent)null);

            var result = await _carRentService.GetCarRentByIdAsync("NonExistentRent");

            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllCarRentsAsync_ReturnsMappedDtoList()
        {
            var carRents = new List<CarRent>
            {
                new CarRent { Id = "Rent1", CarId = "Car123", StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(3) },
                new CarRent { Id = "Rent2", CarId = "Car456", StartDate = DateTime.Today.AddDays(1), EndDate = DateTime.Today.AddDays(4) }
            };

            _mockCarRentRepository
                .Setup(repo => repo.GetManyAsync(It.IsAny<IEnumerable<Expression<Func<CarRent, bool>>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(carRents.ToArray());

            var expectedDtos = new List<CarRentDto>
            {
                new CarRentDto { Id = "Rent1", CarId = "Car123", StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(3) },
                new CarRentDto { Id = "Rent2", CarId = "Car456", StartDate = DateTime.Today.AddDays(1), EndDate = DateTime.Today.AddDays(4) }
            };

            _mockMapper.Setup(m => m.Map<IEnumerable<CarRentDto>>(It.IsAny<IEnumerable<CarRent>>()))
                .Returns(expectedDtos);  // Ensure it returns the expected DTOs

            var result = await _carRentService.GetAllCarRentsAsync();

            Assert.Equal(expectedDtos.Count, result.Count);
        }

        [Fact]
        public async Task DeleteCarRentAsync_WhenCarRentExists_CallsHardDeleteAsync()
        {
            var carRent = new CarRent { Id = "Rent1", CarId = "Car123", StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(3) };

            _mockCarRentRepository
                .Setup(repo => repo.GetAsync(It.IsAny<List<Expression<Func<CarRent, bool>>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(carRent);

            // Act
            await _carRentService.DeleteCarRentAsync("Rent1");

            _mockCarRentRepository.Verify(repo => repo.HardDeleteAsync(carRent, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteCarRentAsync_WhenCarRentDoesNotExist_DoesNotCallHardDeleteAsync()
        {
            _mockCarRentRepository
                .Setup(repo => repo.GetAsync(It.IsAny<List<Expression<Func<CarRent, bool>>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((CarRent)null);

            await _carRentService.DeleteCarRentAsync("NonExistentRent");

            _mockCarRentRepository.Verify(repo => repo.HardDeleteAsync(It.IsAny<CarRent>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task IsCarAvailableAsync_WhenNoOverlappingRents_ReturnsTrue()
        {
            string carId = "Car123";
            DateTime startDate = DateTime.Today;
            DateTime endDate = DateTime.Today.AddDays(5);

            _mockCarRentRepository
                .Setup(repo => repo.GetManyAsync(It.IsAny<List<Expression<Func<CarRent, bool>>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<CarRent>().ToArray());

            var result = await _carRentService.IsCarAvailableAsync(carId, startDate, endDate);

            Assert.True(result);
        }

        [Fact]
        public async Task IsCarAvailableAsync_WhenThereAreOverlappingRents_ReturnsFalse()
        {
            string carId = "Car123";
            DateTime startDate = DateTime.Today;
            DateTime endDate = DateTime.Today.AddDays(5);

            var overlappingRent = new CarRent
            {
                Id = "Rent1",
                Car = new Car { Id = carId },
                StartDate = DateTime.Today.AddDays(2),
                EndDate = DateTime.Today.AddDays(6)
            };

            _mockCarRentRepository
                .Setup(repo => repo.GetManyAsync(It.IsAny<List<Expression<Func<CarRent, bool>>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<CarRent> { overlappingRent }.ToArray());

            var result = await _carRentService.IsCarAvailableAsync(carId, startDate, endDate);

            Assert.False(result);
        }
    }
}
