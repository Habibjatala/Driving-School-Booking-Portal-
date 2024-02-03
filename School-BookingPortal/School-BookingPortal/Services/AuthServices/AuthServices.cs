using Azure.Core;
using MailKit.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MimeKit.Text;
using MimeKit;
using Org.BouncyCastle.Tls;
using School_BookingPortal.Data;
using School_BookingPortal.Dtos;
using School_BookingPortal.Model;
using System.CodeDom.Compiler;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static Org.BouncyCastle.Math.EC.ECCurve;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.SignalR;
using Org.BouncyCastle.Asn1.Ocsp;
using Microsoft.AspNetCore.Identity;

namespace School_BookingPortal.Services.AuthServices
{
    public class AuthServices : IAuthService
    {
        private readonly DataContext _Db;
        private readonly IConfiguration _configuration;

        public AuthServices(DataContext db, IConfiguration configuration)
        {
            _Db = db;
            _configuration = configuration;
        }


        public async Task<List<UserRequest>> GetAllUsers()
        {
            // Assuming you have a DbSet<User> in your DataContext named Users
            return await _Db.UserRequests.ToListAsync();
        }

        public async Task<ServiceResponse<int>> DeleteUser(int userId)
        {
            var response = new ServiceResponse<int>();

            try
            {
                var user = await _Db.UserRequests.FindAsync(userId);

                if (user == null)
                {
                    response.Message = "User not found";
                    return response;
                }

                _Db.UserRequests.Remove(user);
                await _Db.SaveChangesAsync();

                var subject = "Rejected Request"; 
                var body = " Your Request for the Registrtion Rejected due to some Reasons";

                await SendEmail(user.Email, subject, body); 
                

                response.Message = "User deleted successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error: {ex.Message}";
            }

            return response;
        }
        private async Task SendEmail(string Email, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_configuration.GetSection("EmailUsername").Value));
            email.To.Add(MailboxAddress.Parse(Email));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = body };

            using var smtp = new SmtpClient();
            smtp.Connect(_configuration.GetSection("EmailHost").Value, 587, SecureSocketOptions.StartTls);
            smtp.Authenticate(_configuration.GetSection("EmailUsername").Value, _configuration.GetSection("EmailPassword").Value);
            smtp.Send(email);
            smtp.Disconnect(true);
        }


        public async Task<ServiceResponse<int>> CreateRequest(AddRegisterDto request)
        {
            var response = new ServiceResponse<int>();

            var newUserRequest = new UserRequest
            {
                FirstName = request.FirstName,
                LastName= request.LastName,
                Username= request.Username,
                Email = request.Email,
                MobileNo = request.MobileNo,
                Address = request.Address,
                Picture = request.Picture,
                FitnessCertificate= request.FitnessCertificate,
                Vehicle= request.Vehicle,


            };
            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
            newUserRequest.PasswordHash = passwordHash;
            newUserRequest.PasswordSalt = passwordSalt;

            _Db.UserRequests.Add(newUserRequest);
             await _Db.SaveChangesAsync();
           

            response.Data = newUserRequest.Id;
            response.Message = " Successfully Create Request for Registration ";
            return response;
        }

  public async Task<ServiceResponse<int>> ApproveRequest(AddRegisterDto request)
        {
            var response = new ServiceResponse<int>();
            var user = await _Db.UserRequests.FindAsync(request.Id);

            var newUser = new User
            {
                FirstName = request.FirstName,
                LastName= request.LastName,
                Username= request.Username,
                Email = request.Email,
                MobileNo = request.MobileNo,
                Address = request.Address,
                Picture = request.Picture,
                FitnessCertificate= request.FitnessCertificate,
                Vehicle= request.Vehicle,
                


            };
           if(user != null ) {
                newUser.PasswordHash = user.PasswordHash;
                newUser.PasswordSalt = user.PasswordSalt;
            }
       

            _Db.Users.Add(newUser);
             await _Db.SaveChangesAsync();



            _Db.UserRequests.Remove(user);
            await _Db.SaveChangesAsync();

            response.Data = newUser.Id;
            response.Message = " Successfully Registered ";
            var subject = "Approve Request"; // Correct variable name
            var body = " Your Request for the Registrtion Approve now you can Login ";

            await SendEmail(request.Email, subject, body); // Correct method call


            return response;
        }



        public async Task<ServiceResponse<AuthenticatedUserDto>> Login(string email, string password)
        {
            var response = new ServiceResponse<AuthenticatedUserDto>();

            var user = await _Db.Users.FirstOrDefaultAsync(u => u.Email.ToLower().Equals(email.ToLower()));

            if (user == null)
            {
                response.Success = false;
                response.Message = "User Does not Exist";
                return response;
            }
            else if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                response.Success = false;
                response.Message = "Invalid Password";
                return response;
            }
            else
            {
                var authenticatedUser = new AuthenticatedUserDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Username = user.Username,
                    Email = user.Email,
                    MobileNo = user.MobileNo,
                    Address = user.Address,
                    Picture = user.Picture,
                    FitnessCertificate = user.FitnessCertificate,
                    Vehicle = user.Vehicle,
                    Token = CreateToken(user)
                };
                response.Data = authenticatedUser;
                response.Success = true;
                response.Message = "Successfully Logged In";
            }

            return response;
        }

         //Verify Password for login 
        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))

            {

                var ComputeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return ComputeHash.SequenceEqual(passwordHash);
            }
        }



        //check user 
        public async Task<bool> UserExists(string email)
        {
            if (await _Db.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower()))
            {
                return true;
            }

            return false;
        }


        //HashPassword
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }


        //Send email
       

        //Generate Token
        private string CreateToken(User user)
        {



            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTToken:Key"])); ///abcabcabcabcbabbabababababbabababababbaba
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
    new Claim(JwtRegisteredClaimNames.Email, user.Email),
    new Claim("DateOfJoing", "31-04-0000"),
    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
};

            // Set the token expiration to one month from the current date
            var expirationDate = DateTime.Now.AddMonths(1);

            var token = new JwtSecurityToken(
                _configuration["JWTToken:Issuer"],
                _configuration["JWTToken:Issuer"],
                claims,
                expires: expirationDate, // Set the expiration time
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);

        }

    }
}
