namespace School_BookingPortal.Dtos
{
    public class AuthenticatedUserDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Username { get; set; }
        public string? MobileNo { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
        public string? Picture { get; set; }
        public string? FitnessCertificate { get; set; }
        public string? Vehicle { get; set; }
        public string? Token { get; set; }
    }
}
