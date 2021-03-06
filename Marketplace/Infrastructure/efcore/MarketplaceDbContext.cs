﻿using Marketplace.Domain.ClassifiedAd;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Marketplace.Infrastructure.efcore
{
    public class MarketPlaceDbContext : DbContext
    {
        private readonly ILoggerFactory _loggerFactory;

        public MarketPlaceDbContext(DbContextOptions<MarketPlaceDbContext> options, ILoggerFactory loggerFactory)
            : base(options) => _loggerFactory = loggerFactory;
        
        public DbSet<Domain.ClassifiedAd.ClassifiedAd> ClassifiedAds { get; set; }
        public DbSet<Domain.UserProfile.UserProfile> UserProfiles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLoggerFactory(_loggerFactory);
            optionsBuilder.EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ClassifiedAdEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PictureEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new UserProfileEntityTypeConfiguration());
        }
    }

    public class ClassifiedAdEntityTypeConfiguration : IEntityTypeConfiguration<Domain.ClassifiedAd.ClassifiedAd>
    {
        public void Configure(EntityTypeBuilder<Domain.ClassifiedAd.ClassifiedAd> builder)
        {
            builder.HasKey(x => x.ClassifiedAdId);
            builder.OwnsOne(x => x.Id);
            builder.OwnsOne(x => x.Price, p => p.OwnsOne(c => c.Currency));
            builder.OwnsOne(x => x.Text);
            builder.OwnsOne(x => x.Title);
            builder.OwnsOne(x => x.ApprovedBy);
            builder.OwnsOne(x => x.OwnerId);
        }
    }

    public class PictureEntityTypeConfiguration : IEntityTypeConfiguration<Picture>
    {
        public void Configure(EntityTypeBuilder<Picture> builder)
        {
            builder.HasKey(x => x.PictureId);
            builder.OwnsOne(x => x.Id);
            builder.OwnsOne(x => x.ParentId);
            builder.OwnsOne(x => x.Size);
        }
    }
    
    public class UserProfileEntityTypeConfiguration : IEntityTypeConfiguration<Domain.UserProfile.UserProfile>
    {
        public void Configure(EntityTypeBuilder<Domain.UserProfile.UserProfile> builder)
        {
            builder.HasKey(x => x.UserProfileId);
            builder.OwnsOne(x => x.Id);
            builder.OwnsOne(x => x.DisplayName);
            builder.OwnsOne(x => x.FullName);
        }
    }

    public static class AppBuilderDatabaseExtensions
    {
        public static void EnsureDatabase(this IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();
            var context = serviceScope.ServiceProvider.GetService<MarketPlaceDbContext>();

            if(!context.Database.EnsureCreated())
                context.Database.Migrate();
        }
        
        private static void EnsureContextIsMigrated(DbContext context)
        {
            if (!context.Database.EnsureCreated())
                context.Database.Migrate();
        }
        
        public static IServiceCollection AddPostgresDbContext<T>(this IServiceCollection services, 
            string connectionString) where T : DbContext
        {
            services.AddDbContext<T>(options => options.UseNpgsql(connectionString));
            return services;
        }
    }
}