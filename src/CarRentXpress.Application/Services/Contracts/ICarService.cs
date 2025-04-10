using CarRentXpress.DTOs;

namespace CarRentXpress.Application.Services.Contracts;

public interface ICarService
{
    Task AddCarAsync(CarDto carDto, CancellationToken cancellationToken = default);
    Task AddCarsAsync(IEnumerable<CarDto> carsDto, CancellationToken cancellationToken = default);
}