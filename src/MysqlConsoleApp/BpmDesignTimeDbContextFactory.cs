using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Bpmtk.Engine.Storage;

namespace MysqlConsoleApp
{
    public class BpmDesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public virtual ApplicationDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder();
            // builder.UseLoggerFactory(loggerFactory);
            // builder.UseLazyLoadingProxies(true);
            string connectionString = "server=localhost;uid=root;pwd=mctx123456;database=bpmtk4";
            //builder.UseMySql("server=localhost;uid=root;pwd=123456;database=bpmtk4");
            builder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

            return new ApplicationDbContext(builder.Options);
        }
    }

    //Extended
    public class ApplicationDbContext : BpmDbContext
    {

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

    }
}
