using InventoryModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EFCore_DBLibrary
{
    public class InventoryDbContext : DbContext
    {
        private static IConfigurationRoot? _configuration;
        private static readonly Guid _systemUserId = Guid.Parse("2fd28110-93d0-427d-9207-d55dbca680fa");

        public DbSet<Item> Items { get; set; } = null!;

        public InventoryDbContext()
        {

        }

        public InventoryDbContext(DbContextOptions options)
            : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

                _configuration = builder.Build();

                optionsBuilder.UseSqlServer(_configuration.GetConnectionString("InventoryManager"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var items = modelBuilder.Entity<Item>();

            if (items is not null)
            {
                //items.Property(i => i.Name).HasMaxLength(50);

                //items.Property(i => i.Description).HasMaxLength(100);

                //items.Property(i => i.Notes).HasMaxLength(100);

                //items.Property(i => i.PurchasedDate).HasColumnType("datetime");

                //items.Property(i => i.SoldDate).HasColumnType("datetime");

                //items.Property(i => i.PurchasePrice).HasColumnType("decimal (9, 2)");

                //items.Property(i => i.CurrentOrFinalPrice).HasColumnType("decimal (9, 2)");

                //items.Property(i => i.CreatedDate).HasColumnType("datetime");

                //items.Property(i => i.LastModifiedDate).HasColumnType("datetime");
            }
        }

        public override int SaveChanges()
        {
            var tracker = ChangeTracker;

            foreach (var entry in tracker.Entries())
            {
                if (entry.Entity is FullAuditModel)
                {
                    var referenceEntity = entry.Entity as FullAuditModel;
                    if (referenceEntity is null) continue;

                    switch (entry.State)
                    {
                        case EntityState.Added:
                            referenceEntity.CreatedDate = DateTime.UtcNow;
                            if (referenceEntity.CreatedByUserId is null)
                            {
                                referenceEntity.CreatedByUserId = _systemUserId;
                            }
                            break;
                        case EntityState.Deleted:
                        case EntityState.Modified:
                            referenceEntity.LastModifiedDate = DateTime.UtcNow;
                            if (referenceEntity.LastModifiedUserId is null)
                            {
                                referenceEntity.LastModifiedUserId = _systemUserId;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            return base.SaveChanges();
        }
    }
}