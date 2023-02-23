using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TatBlog.Core.Entities;

namespace TatBlog.Data.Mappings;

public class AuthorMap : IEntityTypeConfiguration<Author>
{
  public void Configure(EntityTypeBuilder<Author> builder)
  {
    // Table name
    builder.ToTable("Authors");

    // Primary key
    builder.HasKey(a => a.Id);

    // Fields
    builder.Property(a => a.FullName)
           .IsRequired()
           .HasMaxLength(100);
    builder.Property(a => a.UrlSlug)
           .IsRequired()
           .HasMaxLength(100);
    builder.Property(a => a.ImageUrl)
           .HasMaxLength(500);
    builder.Property(a => a.Email)
           .HasMaxLength(150);
    builder.Property(a => a.JoinedDate)
           .HasColumnType("datetime");
    builder.Property(a => a.Notes)
           .HasMaxLength(500);
  }
}