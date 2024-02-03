namespace School_BookingPortal.Model
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public byte[] PasswordHash { get; set; } = new byte[0];
        public byte[] PasswordSalt { get; set; } = new byte[0];
        public string MobileNo { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Picture { get; set; }
        public string FitnessCertificate { get; set; }
        public string Vehicle { get; set; }
      
    }
}
