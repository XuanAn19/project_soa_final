using FinalProject.Repository.Interface;

namespace FinalProject.Repository.UnitOfWork
{
    public interface IUnitOfWork
    {
        UserTokenRepository userTokenRepository { get; }
        UserRepository userRepository { get; }
        RoleRepository roleRepository { get; }
        AddressRepository addressRepository { get; }
        Task SaveChangeAsync();
    }
}
