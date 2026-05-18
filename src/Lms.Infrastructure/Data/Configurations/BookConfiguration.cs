using Lms.Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lms.Infrastructure.Data.Configurations
{
    public class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.ToTable("books");
            builder.HasKey(entity => entity.Id);
            builder.HasQueryFilter(entity => !entity.IsDeleted);
            builder.Property(entity => entity.CreatedAt)
                .HasColumnName("created_at");
            builder.Property(entity => entity.IsDeleted)
                .HasColumnName("is_deleted");
            builder.Property(entity => entity.Id)
                .HasColumnName("id");
            builder.Property(entity => entity.Isbn)
                .HasColumnName("isbn");
            builder.Property(entity => entity.Issn)
                .HasColumnName("issn");
            builder.Property(entity => entity.Title)
                .HasColumnName("title");
            builder.Property(entity => entity.Description)
                .HasColumnName("description");
            builder.Property(entity => entity.Language)
                .HasColumnName("language");
            builder.Property(entity => entity.Edition)
                .HasColumnName("edition");
            builder.Property(entity => entity.PageCount)
                .HasColumnName("page_count");
            builder.Property(entity => entity.PublisherId)
                .HasColumnName("publisher_id");
            builder.Property(entity => entity.PublishingDate)
                .HasColumnName("publishing_date");
            builder.Property(entity => entity.BorrowPricePerDay)
                .HasColumnName("borrow_price_per_day");
            builder.Property(entity => entity.FinePerDay)
                .HasColumnName("fine_per_day");
            builder.Property(entity => entity.LostFee)
                .HasColumnName("lost_fee");
            builder.Property(entity => entity.DamageFee)
                .HasColumnName("damage_fee");

            builder.HasMany(entity => entity.BookCopies)
                .WithOne(copy => copy.Book)
                .HasForeignKey(copy => copy.BookId);
            builder.Navigation(entity => entity.BookCopies)
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.HasMany(entity => entity.BookCategories)
                .WithOne(bookCategory => bookCategory.Book)
                .HasForeignKey(bookCategory => bookCategory.BookId);
            builder.Navigation(entity => entity.BookCategories)
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.HasMany(entity => entity.BookKeywords)
                .WithOne(bookKeyword => bookKeyword.Book)
                .HasForeignKey(bookKeyword => bookKeyword.BookId);
            builder.Navigation(entity => entity.BookKeywords)
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.HasMany(entity => entity.BookThemes)
                .WithOne(bookTheme => bookTheme.Book)
                .HasForeignKey(bookTheme => bookTheme.BookId);
            builder.Navigation(entity => entity.BookThemes)
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.HasMany(entity => entity.BookGenres)
                .WithOne(bookGenre => bookGenre.Book)
                .HasForeignKey(bookGenre => bookGenre.BookId);
            builder.Navigation(entity => entity.BookGenres)
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.HasMany(entity => entity.BookAudiences)
                .WithOne(bookAudience => bookAudience.Book)
                .HasForeignKey(bookAudience => bookAudience.BookId);
            builder.Navigation(entity => entity.BookAudiences)
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.HasMany(entity => entity.BookAuthors)
                .WithOne(bookAuthor => bookAuthor.Book)
                .HasForeignKey(bookAuthor => bookAuthor.BookId);
            builder.Navigation(entity => entity.BookAuthors)
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.HasOne(entity => entity.Publisher)
                .WithMany()
                .HasForeignKey(entity => entity.PublisherId);
        }
    }
}
