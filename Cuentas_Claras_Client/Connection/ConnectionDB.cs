using Cuentas_Claras_Client.Models;
using Microsoft.EntityFrameworkCore;

namespace Cuentas_Claras_Client.Connection
{
    public class ConnectionDB : DbContext
    {
        public ConnectionDB(DbContextOptions<ConnectionDB> options) : base(options) { }

        public DbSet<Users> users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Users>(entity =>
            {
                entity.Property(u => u.Password).HasColumnName("UserPassword");
            });
        }
    }
}
