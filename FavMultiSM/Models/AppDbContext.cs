using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FavMultiSM.Models
{
    public class AppDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new ConfigurationBuilder()
           .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("passes.json", optional: true, reloadOnChange: true);
            var configuration = builder.Build();
            var conString = configuration.GetSection("db").GetSection("connectionString").Value;
            optionsBuilder.UseMySql(conString, ServerVersion.AutoDetect(conString));
        }
        public DbSet<Challenge> Challenges { get; set; }
        public DbSet<UserAccountData> UserAccountDatas { get; set; }
        public DbSet<UserPicture> UserPictures { get; set; }
        public DbSet<UserText> UserTexts { get; set; }
    }
}
