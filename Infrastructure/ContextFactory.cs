using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    
        public class ContextFactory : IDesignTimeDbContextFactory<StudentDataContext>
        {
            public StudentDataContext CreateDbContext(string[] args)
            {
                var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
            var connectionStr = "Server=DESKTOP-61JNP5Q\\SQL2019;Database=Students;Trusted_Connection=True;MultipleActiveResultSets=true;";//config["ConnectionStrings:FormDb"];
                var optionsBuilder = new DbContextOptionsBuilder<StudentDataContext>();
                optionsBuilder.UseSqlServer(connectionStr,
                    optionsBuilder =>
                    optionsBuilder.MigrationsAssembly("Infrastructure")
                    );
                return new StudentDataContext(optionsBuilder.Options);
            }
        }
}
