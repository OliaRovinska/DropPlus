using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DropPlus.DAL
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<DropPlusDbContext>
    {
        public DropPlusDbContext CreateDbContext(string[] args)
        {
            var conf = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json").Build();

            var builder = new DbContextOptionsBuilder<DropPlusDbContext>();
            builder.UseSqlServer(conf["Data:ConnectionString"]);
            return new DropPlusDbContext(builder.Options);
        }
    }
}