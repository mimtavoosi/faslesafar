using FasleSafar.Models;
using Microsoft.EntityFrameworkCore;

namespace FasleSafar.Data
{
    public class FasleSafarContext:DbContext
    {
        public FasleSafarContext(DbContextOptions<FasleSafarContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Destination> Destinations { get; set; }
        public DbSet<Tour> Tours { get; set; }
        public DbSet<ReqTrip> ReqTrips { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Attraction> Attractions { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Content> Contents { get; set; }
        public DbSet<HotelStaring> HotelStarings { get; set; }
        public DbSet<RatingHistory> RatingHistories { get; set; }
		public DbSet<Passenger> Passengers { get; set; }
		public DbSet<Token> Tokens { get; set; }
		protected override void OnModelCreating(ModelBuilder modelBuilder) //run just once
        {

            base.OnModelCreating(modelBuilder);

            //Next Step: Call Add-Migration and Update-Database tools for update database
        }
    }
}
