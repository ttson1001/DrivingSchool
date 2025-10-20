using TutorDrive.Dtos.Address;
using TutorDrive.Dtos.Address.TutorDrive.Dtos.Address;
using TutorDrive.Entities;

namespace TutorDrive.Services.IService
{
    public interface IAddressService
    {
        Task CreateAddressAsync(AddressDto dto);
        Task UpdateAddressAsync(UpdateAddressDto dto);
        Task<List<UpdateAddressDto>> GetAllAsync();
    }

}
