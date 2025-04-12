using CarRentXpress.Application.Entities;

namespace CarRentXpress.Application.Services;

public interface ICarRentService
{
    Task RentCarAsync(CarRentDto carRentDto, CancellationToken cancellationToken = default);
    Task UpdateCarRentAsync(CarRentDto carRentDto, CancellationToken cancellationToken = default);
    Task<CarRentDto?> GetCarRentByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<List<CarRentDto>> GetAllCarRentsAsync(CancellationToken cancellationToken = default);
    Task DeleteCarRentAsync(string id, CancellationToken cancellationToken = default);

    Task<bool> IsCarAvailableAsync(string carId, DateTime startDate, DateTime endDate,
        CancellationToken cancellationToken = default);
}