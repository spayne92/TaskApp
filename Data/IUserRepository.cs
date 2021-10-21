using BaseCoreAPI.Data.DTOs;

namespace BaseCoreAPI.Data
{
    public interface IUserRepository
    {
        UserDTO GetUser(string userName, string password);
    }
}
