
using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace TMSEntities
{
    public class TMSContext : DbContext
    {
        public TMSContext(DbContextOptions<TMSContext> options) : base(options) { }
        public DbSet<Listing> Listings { get; set; }
        public DbSet<Term> Terms { get; set; }

    }
}
