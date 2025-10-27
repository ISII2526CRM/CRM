using AppForSEII2526.API.Models;
using Humanizer.Localisation;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AppForSEII2526.API.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Device> Device { get; set; }

        public DbSet<ApplicationUser> ApplicationUser { get; set; }

        public DbSet<Model> Model { get; set; }

        public DbSet<Purchase> Purchase { get; set; }
        
        public DbSet<PurchaseItem> PurchaseItem { get; set; }

        public DbSet<Rental> Rental { get; set; }

        public DbSet<RentDevice> RentDevice{ get; set; }

        public DbSet<Review> Review{ get; set; }

        public DbSet<ReviewItem> ReviewItem{ get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

    }

}
