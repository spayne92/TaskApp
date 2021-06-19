using BaseCoreAPI.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BaseCoreAPI.Data
{
    public class BaseContext : DbContext
    {
        public BaseContext(DbContextOptions<BaseContext> options) : base(options)
        {

        }

        public DbSet<Room> Rooms { get; set; }
        public DbSet<Task> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Room>()
                .HasData(new Room()
                {
                    Id = 1,
                    Name = "Kitchen"
                });
        }
    }
}
