using Microsoft.EntityFrameworkCore;
using School_BookingPortal.Model;

namespace School_BookingPortal.Data
{
    public class DataContext : DbContext
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<UserRequest> UserRequests => Set<UserRequest>();
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
    }
}
