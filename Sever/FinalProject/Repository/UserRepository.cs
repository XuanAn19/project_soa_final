using FinalProject.Data;
using FinalProject.Models;
using FinalProject.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Repository
{
    public class UserRepository : IUserRepository
    {
        DataContext _dataContext;

        public UserRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task Delete(User user)
        {
             _dataContext.Users.Remove(user);
        }

        public async Task<List<User>> GetAllUserAsync()
        {
            return await _dataContext.Users.AsQueryable().ToListAsync();
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _dataContext.Users.FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<User> GetUserByIdlAsync(string Id)
        {
            return await _dataContext.Users.Include(r => r.role).Include(a => a.Addresss).FirstOrDefaultAsync(x => x.Id == Id);
        }

        public async Task Insert(User user)
        {
            await _dataContext.Users.AddAsync(user);
        }

        public async Task Update(User user)
        {
            _dataContext.Users.Update(user);
        }
    }
}
