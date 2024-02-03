using System.ComponentModel.DataAnnotations.Schema;

namespace School_BookingPortal.Dtos
{
    public class AddRegisterDto
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? MobileNo { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
        public string? Picture { get; set; }
        public string? FitnessCertificate { get; set; }
        public string? Vehicle { get; set; }

      
        public IFormFile? ImageFile { get; set; }
       
        public IFormFile? CertificateFile { get; set; }
    }
}
