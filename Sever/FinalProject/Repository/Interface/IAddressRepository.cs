using FinalProject.Models.CustomUser;

namespace FinalProject.Repository.Interface
{
    public interface IAddressRepository
    {
        Task<Address> GetAddressByEmailAsync(string id);
        Task UpdateAddressUserAsync(Address address);
        Task AddAddressUserAsync(Address address);
    }
}
