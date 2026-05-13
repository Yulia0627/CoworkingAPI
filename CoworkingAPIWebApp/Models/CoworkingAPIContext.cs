using Microsoft.EntityFrameworkCore;

namespace CoworkingAPIWebApp.Models
{
    public class CoworkingAPIContext : DbContext
    {
        public CoworkingAPIContext(DbContextOptions<CoworkingAPIContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Booking> Bookings { get; set; }
        public virtual DbSet<Equipment> Equipment { get; set; }
        public virtual DbSet<Place> Places { get; set; }
        public virtual DbSet<Service> Services { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<ZoneCategory> ZoneCategories { get; set; }
        public virtual DbSet<BookingStatus> BookingStatuses { get; set; }
        public virtual DbSet<EquipCategory> EquipCategories { get; set; }
    }
}

    

