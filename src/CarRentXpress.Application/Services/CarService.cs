using CarRentXpress.Application.Services.Contracts;
using CarRentXpress.Core.Repositories;
using CarRentXpress.Data.Entities;
using CarRentXpress.DTOs;

namespace CarRentXpress.Application.Services;

public class CarService : ICarService
{
    private readonly IRepository<Car> _carRepository;

    public CarService(IRepository<Car> carRepository)
    {
        this._carRepository = carRepository;
    }

    public async Task AddCarAsync(CarDto carDto, CancellationToken cancellationToken = default)
    {
        var car = MapToEntity(carDto);

        await this._carRepository.CreateAsync(car, cancellationToken);
    }

    public async Task AddCarsAsync(IEnumerable<CarDto> carsDto, CancellationToken cancellationToken = default)
    {
        var cars = carsDto.Select(MapToEntity);

        await this._carRepository.CreateManyAsync(cars, cancellationToken);
    }

    private Car MapToEntity(CarDto dto)
    {
        return new Car
        {
            Id = dto.Id,
            Brand = dto.Brand,
            Model = dto.Model,
            PricePerDay = dto.PricePerDay,
            ImgUrl = dto.ImgUrl,
        };
    }
}
