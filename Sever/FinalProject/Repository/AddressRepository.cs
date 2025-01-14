using FinalProject.Data;
using FinalProject.Models.CustomUser;
using FinalProject.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Repository
{
    public class AddressRepository : IAddressRepository
    {
        DataContext _context;
        public AddressRepository(DataContext context)
        {
            _context = context;
        }
        public async Task AddAddressUserAsync(Address address)
        {
            await _context.Addresses.AddAsync(address);
        }

        public async Task<Address> GetAddressByEmailAsync(string id)
        {
            return await _context.Addresses.FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task UpdateAddressUserAsync(Address address)
        {
            _context.Addresses.Update(address);
        }
    }
}
