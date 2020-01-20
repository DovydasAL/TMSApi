
using Microsoft.EntityFrameworkCore;

namespace TMSEntities
{
    internal class TMSDAL : DbContext
    {
        public DbSet<Listing> Listings { get; set; }
        public DbSet<Term> Terms { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"server=localhost;database=TMSApi;trusted_connection=true;");
        }
    }
}
