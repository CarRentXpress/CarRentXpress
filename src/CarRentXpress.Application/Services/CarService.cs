using AutoMapper;
using CarRentXpress.Application.Services.Interfaces;
using CarRentXpress.Core.Repositories;
using CarRentXpress.Data.Entities;
using CarRentXpress.DTOs;
using System.Linq.Expressions;

namespace CarRentXpress.Application.Services;

public class CarService : ICarService
{
    private readonly IRepository<Car> _carRepository;
    private readonly IMapper _mapper;

    public CarService(IRepository<Car> carRepository, IMapper mapper)
    {
        _carRepository = carRepository;
        _mapper = mapper;
    }

    public async Task AddCarAsync(CarDto carDto, CancellationToken cancellationToken = default)
    {
        Car car = _mapper.Map<Car>(carDto);
    
        if (string.IsNullOrEmpty(car.Id))
        {
            car.Id = Guid.NewGuid().ToString();
        }
        
        await _carRepository.CreateAsync(car, cancellationToken);
    }
    

    public async Task AddCarsAsync(IEnumerable<CarDto> carsDto, CancellationToken cancellationToken = default)
    {
        var cars = _mapper.Map<IEnumerable<Car>>(carsDto);
        await _carRepository.CreateManyAsync(cars, cancellationToken);
    }

    public async Task<List<CarDto>> GetAllCarsAsync(CancellationToken cancellationToken = default)
    {
        var cars = await _carRepository.GetManyAsync(Array.Empty<Expression<Func<Car, bool>>>(), cancellationToken);
        return _mapper.Map<List<CarDto>>(cars.Where(c => c.IsDeleted == false).OrderBy(c => c.PricePerDay));
    }

    public async Task<CarDto?> GetCarByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var filters = new List<Expression<Func<Car, bool>>>
        {
            car => car.Id == id
        };
        var car = await _carRepository.GetAsync(filters, cancellationToken);
        return car != null ? _mapper.Map<CarDto>(car) : null;
    }

    public async Task UpdateCarAsync(CarDto carDto, CancellationToken cancellationToken = default)
    {
        // First, get the existing entity tracked from DB
        var filters = new List<Expression<Func<Car, bool>>>
        {
            car => car.Id == carDto.Id
        };
    
        var existingCar = await _carRepository.GetAsync(filters, cancellationToken);
    
        if (existingCar == null)
            throw new InvalidOperationException($"Car with Id '{carDto.Id}' not found.");

        // Use AutoMapper to update the existing entity from DTO
        _mapper.Map(carDto, existingCar);

        // Update repository (tracked entity)
        await _carRepository.UpdateAsync(existingCar, cancellationToken);
    }

    public async Task SoftDeleteCarAsync(string id, CancellationToken cancellationToken = default)
    {
        var filters = new List<Expression<Func<Car, bool>>>
        {
            car => car.Id == id
        };
        var car = await _carRepository.GetAsync(filters, cancellationToken);
        if (car != null)
        {
            await _carRepository.SoftDeleteAsync(car, cancellationToken);
        }
    }

    public async Task HardDeleteCarAsync(string id, CancellationToken cancellationToken = default)
    {
        var filters = new List<Expression<Func<Car, bool>>>
        {
            car => car.Id == id
        };
        var car = await _carRepository.GetAsync(filters, cancellationToken);
        if (car != null)
        {
            await _carRepository.HardDeleteAsync(car, cancellationToken);
        }
    }
}
