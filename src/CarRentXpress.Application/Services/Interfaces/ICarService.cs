using CarRentXpress.DTOs;

namespace CarRentXpress.Application.Services.Interfaces
{
    public interface ICarService
    {
        Task AddCarAsync(CarDto carDto, CancellationToken cancellationToken = default);
        Task AddCarsAsync(IEnumerable<CarDto> carsDto, CancellationToken cancellationToken = default);
        Task<List<CarDto>> GetAllCarsAsync(CancellationToken cancellationToken = default);
        Task<CarDto?> GetCarByIdAsync(string id, CancellationToken cancellationToken = default);
        Task UpdateCarAsync(CarDto carDto, CancellationToken cancellationToken = default);
        Task DeleteCarAsync(string id, CancellationToken cancellationToken = default);
    }
}