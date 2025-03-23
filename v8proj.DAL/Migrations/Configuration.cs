
using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;

namespace v8proj.DAL.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<v8proj.DAL.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }
    } 
}