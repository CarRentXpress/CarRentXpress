using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using AutoMapper;
using CarRentXpress.Application.Services;
using CarRentXpress.Application.Services.Interfaces;
using CarRentXpress.Core.Repositories;
using CarRentXpress.Data.Entities;
using CarRentXpress.DTOs;

namespace CarRentXpress.Tests.Services
{
    public class CarServiceTests
    {
        private readonly Mock<IRepository<Car>> _mockCarRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ICarService _carService;

        public CarServiceTests()
        {
            _mockCarRepository = new Mock<IRepository<Car>>();
            _mockMapper = new Mock<IMapper>();
            _carService = new CarService(_mockCarRepository.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task AddCarAsync_ShouldAssignNewId_WhenCarIdIsEmpty_AndCallCreateAsync()
        {
            var carDto = new CarDto { Id = "", PricePerDay = 100 };
            var mappedCar = new Car { Id = "", PricePerDay = carDto.PricePerDay };
            _mockMapper.Setup(m => m.Map<Car>(carDto)).Returns(mappedCar);
            _mockCarRepository.Setup(repo => repo.CreateAsync(It.IsAny<Car>(), It.IsAny<CancellationToken>()))
                              .Returns(Task.CompletedTask);
            await _carService.AddCarAsync(carDto, CancellationToken.None);

            _mockCarRepository.Verify(repo => repo.CreateAsync(
                It.Is<Car>(c => !string.IsNullOrEmpty(c.Id) && c.PricePerDay == 100),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AddCarsAsync_ShouldMapAndCallCreateManyAsync()
        {
            var carsDto = new List<CarDto>
            {
                new CarDto { Id = "1", PricePerDay = 120 },
                new CarDto { Id = "2", PricePerDay = 150 }
            };

            var mappedCars = new List<Car>
            {
                new Car { Id = "1", PricePerDay = 120 },
                new Car { Id = "2", PricePerDay = 150 }
            };

            _mockMapper.Setup(m => m.Map<IEnumerable<Car>>(carsDto)).Returns(mappedCars);
            _mockCarRepository.Setup(repo => repo.CreateManyAsync(It.IsAny<IEnumerable<Car>>(), It.IsAny<CancellationToken>()))
                              .Returns(Task.CompletedTask);
            await _carService.AddCarsAsync(carsDto, CancellationToken.None);

            _mockCarRepository.Verify(repo => repo.CreateManyAsync(
                It.Is<IEnumerable<Car>>(cars => cars.SequenceEqual(mappedCars)),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task GetAllCarsAsync_ShouldReturnFilteredAndOrderedCars()
        {
            var cars = new List<Car>
            {
                new Car { Id = "1", PricePerDay = 200, IsDeleted = false },
                new Car { Id = "2", PricePerDay = 100, IsDeleted = true },
                new Car { Id = "3", PricePerDay = 150, IsDeleted = false },
            };

            _mockCarRepository.Setup(repo => repo.GetManyAsync(
                It.IsAny<Expression<Func<Car, bool>>[]>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(cars.ToArray());

            IEnumerable<Car> filteredOrderedCars = cars
                .Where(c => c.IsDeleted == false)
                .OrderBy(c => c.PricePerDay);

            var expectedDtoList = new List<CarDto>
            {
                new CarDto { Id = "3", PricePerDay = 150 },
                new CarDto { Id = "1", PricePerDay = 200 }
            };

            _mockMapper.Setup(m => m.Map<List<CarDto>>(It.Is<IEnumerable<Car>>(cs => cs.SequenceEqual(filteredOrderedCars))))
                       .Returns(expectedDtoList);

            var result = await _carService.GetAllCarsAsync(CancellationToken.None);

            Assert.Equal(expectedDtoList.Count, result.Count);
            Assert.Equal(expectedDtoList[0].Id, result[0].Id);
            Assert.Equal(expectedDtoList[1].Id, result[1].Id);
        }

        [Fact]
        public async Task GetCarByIdAsync_ShouldReturnDto_WhenCarExists()
        {
            var car = new Car { Id = "1", PricePerDay = 100, IsDeleted = false };
            var carDto = new CarDto { Id = "1", PricePerDay = 100 };

            _mockCarRepository.Setup(repo => repo.GetAsync(
                It.IsAny<List<Expression<Func<Car, bool>>>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(car);

            _mockMapper.Setup(m => m.Map<CarDto>(car)).Returns(carDto);

            var result = await _carService.GetCarByIdAsync("1", CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal("1", result.Id);
        }

        [Fact]
        public async Task GetCarByIdAsync_ShouldReturnNull_WhenCarDoesNotExist()
        {
            _mockCarRepository.Setup(repo => repo.GetAsync(
                It.IsAny<List<Expression<Func<Car, bool>>>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync((Car)null);

            var result = await _carService.GetCarByIdAsync("NonExistingId", CancellationToken.None);

            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateCarAsync_ShouldCallUpdateAsync_WhenCarExists()
        {
            var carDto = new CarDto { Id = "1", PricePerDay = 200 };
            var existingCar = new Car { Id = "1", PricePerDay = 150 };

            _mockCarRepository.Setup(repo => repo.GetAsync(
                It.IsAny<List<Expression<Func<Car, bool>>>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingCar);

            _mockMapper.Setup(m => m.Map(carDto, existingCar))
                       .Callback<CarDto, Car>((src, dest) => dest.PricePerDay = src.PricePerDay);

            _mockCarRepository.Setup(repo => repo.UpdateAsync(existingCar, It.IsAny<CancellationToken>()))
                              .Returns(Task.CompletedTask);

            await _carService.UpdateCarAsync(carDto, CancellationToken.None);

            _mockCarRepository.Verify(repo => repo.UpdateAsync(existingCar, It.IsAny<CancellationToken>()), Times.Once);
            Assert.Equal(200, existingCar.PricePerDay);
        }

        [Fact]
        public async Task UpdateCarAsync_ShouldThrowException_WhenCarDoesNotExist()
        {
            var carDto = new CarDto { Id = "NonExistingId", PricePerDay = 200 };

            _mockCarRepository.Setup(repo => repo.GetAsync(
                It.IsAny<List<Expression<Func<Car, bool>>>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync((Car)null);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _carService.UpdateCarAsync(carDto, CancellationToken.None));

            Assert.Contains("Car with Id", ex.Message);
        }

        [Fact]
        public async Task SoftDeleteCarAsync_ShouldCallSoftDeleteAsync_WhenCarExists()
        {
            var car = new Car { Id = "1" };

            _mockCarRepository.Setup(repo => repo.GetAsync(
                It.IsAny<List<Expression<Func<Car, bool>>>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(car);

            _mockCarRepository.Setup(repo => repo.SoftDeleteAsync(car, It.IsAny<CancellationToken>()))
                              .Returns(Task.CompletedTask);

            await _carService.SoftDeleteCarAsync("1", CancellationToken.None);

            _mockCarRepository.Verify(repo => repo.SoftDeleteAsync(car, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task SoftDeleteCarAsync_ShouldNotCallSoftDeleteAsync_WhenCarDoesNotExist()
        {
            _mockCarRepository.Setup(repo => repo.GetAsync(
                It.IsAny<List<Expression<Func<Car, bool>>>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync((Car)null);

            await _carService.SoftDeleteCarAsync("NonExistingId", CancellationToken.None);

            _mockCarRepository.Verify(repo => repo.SoftDeleteAsync(It.IsAny<Car>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task HardDeleteCarAsync_ShouldCallHardDeleteAsync_WhenCarExists()
        {
            var car = new Car { Id = "1" };

            _mockCarRepository.Setup(repo => repo.GetAsync(
                It.IsAny<List<Expression<Func<Car, bool>>>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(car);

            _mockCarRepository.Setup(repo => repo.HardDeleteAsync(car, It.IsAny<CancellationToken>()))
                              .Returns(Task.CompletedTask);

            await _carService.HardDeleteCarAsync("1", CancellationToken.None);

            _mockCarRepository.Verify(repo => repo.HardDeleteAsync(car, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HardDeleteCarAsync_ShouldNotCallHardDeleteAsync_WhenCarDoesNotExist()
        {
            _mockCarRepository.Setup(repo => repo.GetAsync(
                It.IsAny<List<Expression<Func<Car, bool>>>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync((Car)null);

            await _carService.HardDeleteCarAsync("NonExistingId", CancellationToken.None);

            _mockCarRepository.Verify(repo => repo.HardDeleteAsync(It.IsAny<Car>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
