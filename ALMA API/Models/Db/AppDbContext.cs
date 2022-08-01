using Microsoft.EntityFrameworkCore;

namespace ALMA_API.Models.Db
{
    public partial class AppDbContext : DbContext
    {
        public AppDbContext()
        {
        }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<Farm> Farm { get; set; }
        public virtual DbSet<Cow> Cow { get; set; }
        public virtual DbSet<Production> Production { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var config =
                    new ConfigurationBuilder()
                        .AddEnvironmentVariables()
                        .Build();
                
                var ip = Environment.GetEnvironmentVariable("MYSQL_SERVER_IP");
                var port = Environment.GetEnvironmentVariable("MYSQL_SERVER_PORT");
                var user = Environment.GetEnvironmentVariable("MYSQL_SERVER_USER");
                var password = Environment.GetEnvironmentVariable("MYSQL_SERVER_PASSWORD");
                var database = Environment.GetEnvironmentVariable("MYSQL_SERVER_DATABASE");
                var connectionString = $"server={ip};port={port};user={user};password={password};database={database}";
                var serverVersion = new MySqlServerVersion(new Version(8, 0, 1));
                optionsBuilder.UseMySql(connectionString, serverVersion);
            }
        }
        
    }
}
