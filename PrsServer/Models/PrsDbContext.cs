using Microsoft.EntityFrameworkCore;

namespace PrsServer.Models {
        public class PrsDbContext : DbContext {

            public DbSet<User> Users { get; set; }
            public DbSet<Vendor> Vendors { get; set; }
            public DbSet<Product> Products { get; set; }
            public DbSet<Request> Requests { get; set; }
            public DbSet<Requestline> Requestlines { get; set; }

            public PrsDbContext(DbContextOptions<PrsDbContext> options) : base(options) { }

            protected override void OnModelCreating(ModelBuilder modelBuilder) { }

        }
    }
