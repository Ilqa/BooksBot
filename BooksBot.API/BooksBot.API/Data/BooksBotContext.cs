using BooksBot.API.Data.Entities;
using BooksBot.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BooksBot.API.Data
{
    public class BooksBotContext  : IdentityDbContext<IdentityUser, IdentityRole, string>
    {
        public BooksBotContext(DbContextOptions<BooksBotContext> options) : base(options)
        {
            
        }

        public DbSet<ShouldCrawlEan> ShouldCrawlEans { get; set; }
        public DbSet<CrawlSource> CrawlSources { get; set; }
        public DbSet<BookData> BookData { get; set; }
        public DbSet<SourceWebsite> SourceWebsite { get; set; }

        //public DbSet<UrlStatus> ClaimDataView { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ShouldCrawlEan>(eb => 
            {
                eb.Property(p => p.Ean).IsRequired();
            });
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
        {
            foreach (var entry in ChangeTracker.Entries<IAuditableEntity>().ToList())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedOn = System.DateTime.UtcNow;
                        break;

                    case EntityState.Modified:
                        entry.Entity.ModifiedOn = System.DateTime.UtcNow;
                        break;
                }
            }

            return await base.SaveChangesAsync();

        }

    }
}
