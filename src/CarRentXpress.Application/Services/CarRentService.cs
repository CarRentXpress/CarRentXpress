using System.Linq.Expressions;
using AutoMapper;
using CarRentXpress.Application.Entities;
using CarRentXpress.Core.Repositories;
using CarRentXpress.Data;
using CarRentXpress.Data.Entities;

namespace CarRentXpress.Application.Services
{
    public class CarRentService : ICarRentService
    {
        private readonly IRepository<CarRent> _carRentRepository;
        private readonly IRepository<Car> _carRepository;
        private readonly IMapper _mapper;
        

        public CarRentService(IRepository<CarRent> carRentRepository, IRepository<Car> carRepository, IMapper mapper)
        {
            _carRepository = carRepository;
            _carRentRepository = carRentRepository;
            _mapper = mapper;
        }

        public async Task RentCarAsync(CarRentDto carRentDto, CancellationToken cancellationToken = default)
        {
            var carRent = _mapper.Map<CarRent>(carRentDto);
            await _carRentRepository.CreateAsync(carRent, cancellationToken);
        }

        public async Task UpdateCarRentAsync(CarRentDto carRentDto, CancellationToken cancellationToken = default)
        {
            var filters = new List<Expression<Func<CarRent, bool>>>
            {
                carRent => carRent.Id == carRentDto.Id
            };

            var existingCarRent = await _carRentRepository.GetAsync(filters, cancellationToken);

            if (existingCarRent == null)
                throw new InvalidOperationException($"CarRent with Id '{carRentDto.Id}' not found.");

            _mapper.Map(carRentDto, existingCarRent);

            await _carRentRepository.UpdateAsync(existingCarRent, cancellationToken);
        }

        public async Task<CarRentDto?> GetCarRentByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            var filters = new List<Expression<Func<CarRent, bool>>>
            {
                carRent => carRent.Id == id
            };

            var carRent = await _carRentRepository.GetAsync(filters, cancellationToken);

            return carRent != null ? _mapper.Map<CarRentDto>(carRent) : null;
        }

        public async Task<List<CarRentDto>> GetAllCarRentsAsync(CancellationToken cancellationToken = default)
        {
            var carRents = await _carRentRepository.GetManyAsync(Array.Empty<Expression<Func<CarRent, bool>>>(), cancellationToken);
            foreach (var carRent in carRents)
            {
                var car = await _carRepository.GetAsync(
                    [c => c.Id == carRent.CarId],
                    cancellationToken
                );

                carRent.Car = car;
            }
            return _mapper.Map<List<CarRentDto>>(carRents);
        }

        public async Task DeleteCarRentAsync(string id, CancellationToken cancellationToken = default)
        {
            var filters = new List<Expression<Func<CarRent, bool>>>
            {
                carRent => carRent.Id == id
            };

            var carRent = await _carRentRepository.GetAsync(filters, cancellationToken);

            if (carRent != null)
            {
                await _carRentRepository.HardDeleteAsync(carRent, cancellationToken);
            }
        }

        public async Task<bool> IsCarAvailableAsync(string carId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            Expression<Func<CarRent, bool>> carFilter = r => r.Car.Id == carId;
    
            Expression<Func<CarRent, bool>> dateOverlapFilter = r =>
                (startDate >= r.StartDate && startDate <= r.EndDate) ||
                (endDate >= r.StartDate && endDate <= r.EndDate) ||
                (startDate <= r.StartDate && endDate >= r.EndDate);

            var filters = new List<Expression<Func<CarRent, bool>>>
            {
                carFilter,
                dateOverlapFilter
            };

            var overlappingRents = await _carRentRepository.GetManyAsync(filters, cancellationToken);

            // If there are no overlapping rentals, the car is available
            return !overlappingRents.Any();
        }



    }
}
