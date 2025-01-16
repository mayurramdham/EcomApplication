using App.core.Interface;
using Domain.Entity.Products;
using Domain.Entity.Register;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Database
{
    public class AppDbContext:DbContext,IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<User> User { get; set; }
        public DbSet<State> State { get; set; }
        public DbSet<Country> Country { get; set; }
        public DbSet<UserType> UserType { get; set; }
        public DbSet<Card> Card { get; set; }
        public DbSet<CartDetails> CartDetails { get; set; }
        public DbSet<CartMaster> CartMaster { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<SalesDetails> SalesDetails { get; set; }
        public DbSet<SalesMaster> SalesMaster { get; set; }
        public DbSet<Otp> Otp { get; set; }

        public DbContext DbContext => this;
    }
}
