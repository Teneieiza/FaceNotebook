using Microsoft.EntityFrameworkCore;
using FaceNoteBook.Models;

namespace FaceNoteBook.Data;

public class UserDataContext : DbContext
{
    public UserDataContext(DbContextOptions<UserDataContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<User>(entity =>
    {
        entity.ToTable("users");
        
        entity.HasKey(e => e.Id);
        
        entity.Property(e => e.Id)
            .HasColumnName("id");
        
        entity.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnName("name");
        
        entity.Property(e => e.Email)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnName("email");
        
        entity.HasIndex(e => e.Email)
            .IsUnique();
        
        entity.Property(e => e.Password)
            .IsRequired()
            .HasMaxLength(150)
            .HasColumnName("password");
        
        entity.Property(e => e.CreatedAt)
            .HasColumnName("createdat")
            .HasColumnType("timestamp");
        
        entity.Property(e => e.UpdatedAt)
            .HasColumnName("updatedat")
            .HasColumnType("timestamp");
    });
    }
    
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            if (entry.Entity is User user)
            {
                user.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}