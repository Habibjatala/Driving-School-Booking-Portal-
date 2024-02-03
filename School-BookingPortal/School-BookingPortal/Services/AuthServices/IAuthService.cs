using School_BookingPortal.Dtos;
using School_BookingPortal.Model;

namespace School_BookingPortal.Services.AuthServices
{
    public interface IAuthService
    {
        Task<ServiceResponse<int>> CreateRequest(AddRegisterDto request);
        Task<ServiceResponse<int>> ApproveRequest(AddRegisterDto request);
        Task<ServiceResponse<AuthenticatedUserDto>> Login(String username, string password);
        Task<List<UserRequest>> GetAllUsers();
        Task<ServiceResponse<int>> DeleteUser(int userId);
        Task<bool> UserExists(string username);
    }
}
