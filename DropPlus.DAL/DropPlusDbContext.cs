using DropPlus.DAL.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DropPlus.DAL
{
    public class DropPlusDbContext : IdentityDbContext<AppUser>
    {
        public virtual DbSet<AppUserResort> AppUserResorts { get; set; }
        public virtual DbSet<Resort> Resorts { get; set; }
        public virtual DbSet<ResortTag> ResortTags { get; set; }
        public virtual DbSet<Review> Reviews { get; set; }
        public virtual DbSet<Tag> Tags { get; set; }

        public DropPlusDbContext(DbContextOptions<DropPlusDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Resort>()
                .HasKey(resort => resort.Id);

            modelBuilder.Entity<Review>()
                .HasKey(review => review.Id);

            modelBuilder.Entity<Tag>()
                .HasKey(tag => tag.Id);

            modelBuilder.Entity<Review>()
                .HasOne(review => review.AppUser)
                .WithMany(appUser => appUser.Reviews)
                .HasForeignKey(review => review.AppUserId);

            modelBuilder.Entity<Review>()
                .HasOne(review => review.Resort)
                .WithMany(resort => resort.Reviews)
                .HasForeignKey(review => review.ResortId);

            // many to many
            // AppUserResorts
            modelBuilder.Entity<AppUserResort>()
                .HasKey(appUserResort => new { appUserResort.AppUserId, appUserResort.ResortId });

            modelBuilder.Entity<AppUserResort>()
                .HasOne(appUserResort => appUserResort.AppUser)
                .WithMany(appUser => appUser.AppUserResorts)
                .HasForeignKey(appUserResort => appUserResort.AppUserId);

            modelBuilder.Entity<AppUserResort>()
                .HasOne(appUserResort => appUserResort.Resort)
                .WithMany(resort => resort.AppUserResorts)
                .HasForeignKey(appUserResort => appUserResort.ResortId);

            // ResortTags
            modelBuilder.Entity<ResortTag>()
                .HasKey(resortTag => new { resortTag.ResortId, resortTag.TagId });

            modelBuilder.Entity<ResortTag>()
                .HasOne(resortTag => resortTag.Resort)
                .WithMany(resort => resort.ResortTags)
                .HasForeignKey(resortTag => resortTag.TagId);

            modelBuilder.Entity<ResortTag>()
                .HasOne(resortTag => resortTag.Tag)
                .WithMany(tag => tag.ResortTags)
                .HasForeignKey(resortTag => resortTag.ResortId);

            base.OnModelCreating(modelBuilder);
        }
    }
}