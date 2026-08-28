using System.Threading.Tasks.Dataflow;
using LibraryApi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Book> Books => Set<Book>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<BookReview> BookReviews => Set<BookReview>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<RefreshToken>()
            .HasOne(rt => rt.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<BookReview>()
            .HasOne(br => br.Book)
            .WithMany()
            .HasForeignKey(br => br.BookId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<BookReview>()
            .HasOne(br => br.User)
            .WithMany()
            .HasForeignKey(br => br.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<BookReview>()
            .HasIndex(br => new { br.BookId, br.UserId })
            .IsUnique();
    }
}