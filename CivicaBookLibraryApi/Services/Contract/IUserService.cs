using CivicaBookLibraryApi.Dtos;

namespace CivicaBookLibraryApi.Services.Contract
{
    public interface IUserService
    {

        ServiceResponse<UserDto> GetUserById(int userId);
        ServiceResponse<string> LoginUserService(LoginDto login);
        ServiceResponse<string> RegisterUserService(RegisterDto register);
        ServiceResponse<string> ChangePassword(ChangePasswordDto changePasswordDto);
        ServiceResponse<string> ResetPassword(ResetPasswordDto resetPasswordDto);
        ServiceResponse<IEnumerable<UserDto>> GetAllUsers();
        ServiceResponse<IEnumerable<UserDto>> GetPaginatedUsers(int page, int pageSize, string? search, string sortOrder);
        ServiceResponse<string> RemoveUser(int id);
        ServiceResponse<int> TotalUsers(string? search);
    }
}
