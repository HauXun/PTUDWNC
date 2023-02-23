using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TatBlog.Core.Entities;

namespace TatBlog.Data.Mappings;

public class CategoryMap : IEntityTypeConfiguration<Category>
{
  public void Configure(EntityTypeBuilder<Category> builder)
  {
    // Table name
    builder.ToTable("Categories");

    // Primary key
    builder.HasKey(c => c.Id);

    // Fields
    builder.Property(c => c.Name)
           .IsRequired()
           .HasMaxLength(50);
    builder.Property(c => c.Description)
           .HasMaxLength(500);
    builder.Property(c => c.UrlSlug)
           .IsRequired()
           .HasMaxLength(50);
    builder.Property(c => c.ShowOnMenu)
           .HasMaxLength(150)
           .HasDefaultValue(false);
  }
}