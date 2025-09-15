using BookReviewApp.Models.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookReviewApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ReviewVote> ReviewVotes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Book>(entity =>
            {
                entity.HasKey(b => b.Id);
                entity.Property(b => b.Title).IsRequired().HasMaxLength(200);
                entity.Property(b => b.Author).IsRequired().HasMaxLength(100);
                entity.Property(b => b.Genre).IsRequired().HasMaxLength(50);
                entity.HasIndex(b => b.Genre);
                entity.HasIndex(b => b.PublishedYear);
            });

            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.Property(r => r.Content).IsRequired().HasMaxLength(2000);
                entity.Property(r => r.Rating).IsRequired();

                entity.HasOne(r => r.Book)
                    .WithMany(b => b.Reviews)
                    .HasForeignKey(r => r.BookId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(r => r.User)
                    .WithMany(u => u.Reviews)
                    .HasForeignKey(r => r.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(r => r.BookId);
                entity.HasIndex(r => r.UserId);
                entity.HasIndex(r => r.DateCreated);
            });

            modelBuilder.Entity<ReviewVote>(entity =>
            {
                entity.HasKey(rv => rv.Id);

                entity.HasOne(rv => rv.Review)
                    .WithMany(r => r.ReviewVotes)
                    .HasForeignKey(rv => rv.ReviewId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(rv => rv.User)
                    .WithMany(u => u.ReviewVotes)
                    .HasForeignKey(rv => rv.UserId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasIndex(rv => new { rv.ReviewId, rv.UserId }).IsUnique();
            });
        }
    }
}