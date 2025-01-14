using FinalProject.Data;
using FinalProject.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Repository.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly DataContext _context;
        private bool disposedValue;

        // Các repository
        UserTokenRepository _userTokenRepository;
        UserRepository _userRepository;
        RoleRepository _roleRepository;
        AddressRepository _addressRepository;

        public UnitOfWork(DataContext context)
        {
            _context = context;
        }

        public UserTokenRepository userTokenRepository { get { return _userTokenRepository ??= new UserTokenRepository(_context); } }
        public UserRepository userRepository { get { return _userRepository ??= new UserRepository(_context); } }
        public RoleRepository roleRepository { get { return _roleRepository ??= new RoleRepository(_context); } }
        public AddressRepository addressRepository { get { return _addressRepository ??= new AddressRepository(_context); } }

        public async Task SaveChangeAsync()
        {
            await _context.SaveChangesAsync();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
                disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this); // đảm bảo dọn sạch bộ nhớ khi chạy xong và không khởi tại Unit để chiếm bộ nhớ
        }
    }
}
