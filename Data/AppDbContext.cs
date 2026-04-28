using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WeatherApp.Models;

namespace WeatherApp.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<WeatherRecord> WeatherRecords { get; set; }
    public DbSet<WeatherAudit> WeatherAudits { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<WeatherRecord>()
            .HasOne(w => w.User)
            .WithMany()
            .HasForeignKey(w => w.UserId);

        builder.Entity<WeatherAudit>()
            .HasOne(a => a.WeatherRecord)
            .WithMany()
            .HasForeignKey(a => a.WeatherRecordId);
        
        builder.Entity<WeatherAudit>()
            .HasOne(a => a.ChangedByUser)
            .WithMany()
            .HasForeignKey(a => a.ChangedByUserId)
            .OnDelete(DeleteBehavior.NoAction);;
        
        builder.Entity<WeatherRecord>()
            .Property(x => x.TempMin)
            .HasPrecision(5, 2);

        builder.Entity<WeatherRecord>()
            .Property(x => x.TempMax)
            .HasPrecision(5, 2);
    }
}