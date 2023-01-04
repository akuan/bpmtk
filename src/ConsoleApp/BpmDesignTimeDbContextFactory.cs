using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Bpmtk.Engine.Storage;

namespace ConsoleApp
{
    public class BpmDesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public virtual ApplicationDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder();
            // builder.UseLoggerFactory(loggerFactory);
            // builder.UseLazyLoadingProxies(true);
            var conn = "server=localhost;uid=root;pwd=123456;database=bpmtk3";
            builder.UseMySql(conn, ServerVersion.AutoDetect(conn));

            return new ApplicationDbContext(builder.Options);
        }
    }

    public class ApplicationDbContext : BpmDbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}
