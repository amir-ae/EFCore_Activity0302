using InventoryModels;
using InventoryModels.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EFCore_DBLibrary
{
    public class InventoryDbContext : DbContext
    {
        private static IConfigurationRoot? _configuration;
        private static readonly Guid _systemUserId = Guid.Parse("2fd28110-93d0-427d-9207-d55dbca680fa");

        public DbSet<Item> Items { get; set; } = null!;

        public DbSet<Category> Categories { get; set; } = null!;

        public DbSet<CategoryDetail> CategoryDetails { get; set; } = null!;

        public DbSet<Player> Players { get; set; } = null!;

        public DbSet<Genre> Genres { get; set; } = null!;

        public DbSet<GetItemsForListingDto> ItemsForListing { get; set; } = null!;

        public DbSet<AllItemNamesPipeDelimitedStringDto> AllItemNames { get; set; } = null!;

        public DbSet<GetItemsTotalValueDto> ItemsTotalValue { get; set; } = null!;

        public DbSet<FullItemDetailDto> FullItemDetail { get; set; } = null!;

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
            modelBuilder.Entity<Item>()
                .HasMany(x => x.Players)
                .WithMany(p => p.Items)
                .UsingEntity<Dictionary<string, object>>(
                    "ItemPlayers",
                    ip => ip.HasOne<Player>()
                        .WithMany()
                        .HasForeignKey("PlayerId")
                        .HasConstraintName("FK_ItemPlayer_Player")
                        .OnDelete(DeleteBehavior.Cascade),
                    ip => ip.HasOne<Item>()
                        .WithMany()
                        .HasForeignKey("ItemId")
                        .HasConstraintName("FK_PlayerItem_Item")
                        .OnDelete(DeleteBehavior.ClientCascade)
                );

            modelBuilder.Entity<GetItemsForListingDto>(x =>
            {
                x.HasNoKey();
                x.ToView("ItemsForListing");
            });

            modelBuilder.Entity<AllItemNamesPipeDelimitedStringDto>(x =>
            {
                x.HasNoKey();
                x.ToView("AllItemNames");
            });

            modelBuilder.Entity<GetItemsTotalValueDto>(x =>
            {
                x.HasNoKey();
                x.ToView("ItemsTotalValue");
            });

            var genreCreateDate = new DateTime(2021, 01, 01);
            modelBuilder.Entity<Genre>(x =>
            {
                x.HasData(
                    new Genre() { Id = 1, IsActive = true, IsDeleted = false, Name = "Fantasy", CreatedDate = genreCreateDate, CreatedByUserId = _systemUserId },
                    new Genre() { Id = 2, IsActive = true, IsDeleted = false, Name = "Sci/Fi", CreatedDate = genreCreateDate, CreatedByUserId = _systemUserId },
                    new Genre() { Id = 3, IsActive = true, IsDeleted = false, Name = "Horror", CreatedDate = genreCreateDate, CreatedByUserId = _systemUserId },
                    new Genre() { Id = 4, IsActive = true, IsDeleted = false, Name = "Comedy", CreatedDate = genreCreateDate, CreatedByUserId = _systemUserId },
                    new Genre() { Id = 5, IsActive = true, IsDeleted = false, Name = "Drama", CreatedDate = genreCreateDate, CreatedByUserId = _systemUserId }
                );
            });

            modelBuilder.Entity<FullItemDetailDto>(x =>
            {
                x.HasNoKey();
                x.ToView("FullItemDetail");
            });
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