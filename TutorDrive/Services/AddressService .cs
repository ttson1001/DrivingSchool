using Microsoft.EntityFrameworkCore;
using System;
using TutorDrive.Database;
using TutorDrive.Dtos.Address;
using TutorDrive.Dtos.Address.TutorDrive.Dtos.Address;
using TutorDrive.Entities;
using TutorDrive.Repositories;
using TutorDrive.Services.IService;

namespace TutorDrive.Services
{
    public class AddressService : IAddressService
    {
        private readonly IRepository<Address> _repository;

        public AddressService(IRepository<Address> repository)
        {
            _repository = repository;
        }

        public async Task<List<UpdateAddressDto>> GetAllAsync()
        {
            return await _repository.Get()
                .Select(a => new UpdateAddressDto
                {
                    Id = a.Id,
                    FullAddress = a.FullAddress,
                    Street = a.Street,
                    WardId = a.WardId,
                    ProvinceId = a.ProvinceId
                })
                .ToListAsync();
        }


        public async Task CreateAddressAsync(AddressDto dto)
        {
            var address = new Address
            {
                FullAddress = dto.FullAddress,
                Street = dto.Street,
                WardId = dto.WardId,
                ProvinceId = dto.ProvinceId
            };

            await _repository.AddAsync(address);
            await _repository.SaveChangesAsync();
        }

        public async Task UpdateAddressAsync(UpdateAddressDto dto)
        {
            var address = await _repository.Get().FirstOrDefaultAsync(x => x.Id == dto.Id);
            if (address == null) throw new Exception("Address không tìm thấy");

            address.FullAddress = dto.FullAddress;
            address.Street = dto.Street;
            address.WardId = dto.WardId;
            address.ProvinceId = dto.ProvinceId;

            await _repository.SaveChangesAsync();
        }
    }

}
