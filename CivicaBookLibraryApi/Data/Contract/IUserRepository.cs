using CivicaBookLibraryApi.Models;
namespace CivicaBookLibraryApi.Data.Contract
{
    public interface IUserRepository
    {
        User GetUserById(int id);
        User? ValidateUser(string username);
        bool RegisterUser(User user);
        bool UserExists(string loginId, string email);
        int UserCount();
        bool UpdateUser(User user);
        IEnumerable<User> GetAllUsers();
        IEnumerable<User> GetPaginatedUsers(int page, int pageSize, string? search, string sortOrder);
        bool DeleteUser(int id);
        int TotalUsers(string? search);
    }
}
