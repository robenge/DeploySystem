using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace DeploySystem.Api.Data;

public class DeployDbContextFactory : IDesignTimeDbContextFactory<DeployDbContext>
{
    public DeployDbContext CreateDbContext(string[] args)
    {
        // Constrói a configuração a partir do appsettings.json
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var connectionString = configuration.GetConnectionString("Default");

        var optionsBuilder = new DbContextOptionsBuilder<DeployDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new DeployDbContext(optionsBuilder.Options);
    }
}
