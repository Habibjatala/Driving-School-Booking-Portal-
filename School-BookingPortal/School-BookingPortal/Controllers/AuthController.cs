using MailKit.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MimeKit.Text;
using MimeKit;
using School_BookingPortal.Dtos;
using School_BookingPortal.Model;
using School_BookingPortal.Services.AuthServices;
using School_BookingPortal.Services.FilesServices;
using MailKit.Net.Smtp;

namespace School_BookingPortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IFileService _fileService;
        private readonly IConfiguration _configuration;

        public AuthController(IFileService fileService, IAuthService authService, IConfiguration configuration)
        {
            _fileService = fileService;
            _authService = authService;
            _configuration = configuration;
        }

        [HttpPost("CreateRequest")]
        public async Task<ActionResult<ServiceResponse<int>>> CreateRequest(AddRegisterDto request)
        {
          
            
            if(request.ImageFile != null)
            {
                var fileResult = _fileService.SaveImage(request.ImageFile);
                if (fileResult.Item1==1)
                {
                    request.Picture = fileResult.Item2;

                }
               

            }
            if(request.CertificateFile != null)
            {
               var fileResult= _fileService.Savefile(request.CertificateFile);
                if(fileResult.Item1==1)
                {
                    request.FitnessCertificate = fileResult.Item2;
                }
            }
            var response = await _authService.CreateRequest(request);
            if (!response.Success)
            {
                return BadRequest(response);
            }
           
            return Ok(response);
        }
        
         [HttpPost("ApproveRequest")]
        public async Task<ActionResult<ServiceResponse<int>>> ApproveRequest(AddRegisterDto request)
        {
          
            
            
            
            var response = await _authService.ApproveRequest(request);
            if (!response.Success)
            {
                return BadRequest(response);
            }
           
            return Ok(response);
        }
        
        
        [HttpGet("GetAllUsersRequests")]
        public async Task<ActionResult<List<User>>> GetAllUsers()
        {
            var users = await _authService.GetAllUsers();
            return Ok(users);
        }

        [HttpDelete("delete/{id}")]
        public async Task<ActionResult<ServiceResponse<int>>> DeleteUser(int id)
        {
            // Perform deletion logic here (e.g., using Entity Framework)
            var response = await _authService.DeleteUser(id);

            if (!response.Success)
            {
                return BadRequest(response);
            }
            
            return Ok(response);
        }

        [HttpPost("Login")]
        public async Task<ActionResult<ServiceResponse<AuthenticatedUserDto>>> Login(LoginDto request)
        {
            var response = await _authService.Login(request.Email, request.Password);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);

        }
    }
}
