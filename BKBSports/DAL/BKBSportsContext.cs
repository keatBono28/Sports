using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using BKBSports.Models;

namespace BKBSports.DAL
{
    public class BKBSportsContext : DbContext
    {
        public BKBSportsContext() : base("name=defaultConnection")
        {
            // Migration Point
        }
        public DbSet<ArticleModel> Articles { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
    }
}