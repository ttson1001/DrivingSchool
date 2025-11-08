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
        private readonly IRepository<Account> _accoutnRepository;
        private readonly IRepository<StudentProfile> _studentRepo;
        private readonly IRepository<Ward> _wardRepo;
        private readonly IRepository<Province> _provinceRepo;

        public AddressService(IRepository<Address> repository, IRepository<Account> accoutnRepository, IRepository<StudentProfile> studentRepo, IRepository<Ward> wardRepo, IRepository<Province> provinceRepo)
        {
            _repository = repository;
            _accoutnRepository = accoutnRepository;
            _studentRepo = studentRepo;
            _wardRepo = wardRepo;
            _provinceRepo = provinceRepo;
        }

        public async Task<List<UpdateAddressDto>> GetAllAsync()
        {
            return await _repository.Get().Include(x => x.Ward).Include(x => x.Province)
                .Select(a => new UpdateAddressDto
                {
                    Id = a.Id,
                    FullAddress = a.FullAddress,
                    Street = a.Street,
                    WardCode = a.Ward.Code,
                    ProvinceCode = a.Province.Code
                })
                .ToListAsync();
        }

        public async Task CreateAddressAsync(AddressDto dto)
        {
            if (dto.AccountId == null)
                throw new Exception("AccountId là bắt buộc để tạo địa chỉ");

            var student = await _studentRepo.Get()
                .Include(s => s.Address)
                .FirstOrDefaultAsync(s => s.AccountId == dto.AccountId);

            if (student == null)
                throw new Exception("Không tìm thấy hồ sơ học viên tương ứng với tài khoản này");

            if (student.AddressId.HasValue && student.Address != null)
            {
                _repository.Delete(student.Address);
                await _repository.SaveChangesAsync();
            }

            var newAddress = new Address
            {
                FullAddress = dto.FullAddress ?? "",
                Street = dto.Street ?? "",
                WardId = dto.WardId ?? 0,
                ProvinceId = dto.ProvinceId ?? 0
            };

            await _repository.AddAsync(newAddress);
            await _repository.SaveChangesAsync();

            student.AddressId = newAddress.Id;
            await _repository.SaveChangesAsync();
        }


        public async Task UpdateAddressAsync(UpdateAddressDto dto)
        {
            var address = await _repository.Get().FirstOrDefaultAsync(x => x.Id == dto.Id);
            if (address == null) throw new Exception("Address không tìm thấy");
            var ward = await _wardRepo.Get().FirstOrDefaultAsync(x => x.Code == dto.WardCode);
            var province = await _provinceRepo.Get().FirstOrDefaultAsync(x => x.Code == dto.ProvinceCode);
            address.FullAddress = dto.FullAddress;
            address.Street = dto.Street;
            address.WardId = ward.Id;
            address.ProvinceId = province.Id;

            await _repository.SaveChangesAsync();
        }
    }

}
