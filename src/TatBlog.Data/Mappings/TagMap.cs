using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TatBlog.Core.Entities;

namespace TatBlog.Data.Mappings;

public class TagMap : IEntityTypeConfiguration<Tag>
{
  public void Configure(EntityTypeBuilder<Tag> builder)
  {
    // Table name
    builder.ToTable("Tags");

    // Primary key
    builder.HasKey(t => t.Id);

    // Fields
    builder.Property(t => t.Name)
           .IsRequired()
           .HasMaxLength(50);
    builder.Property(t => t.Description)
           .HasMaxLength(500);
    builder.Property(t => t.UrlSlug)
           .IsRequired()
           .HasMaxLength(50);
  }
}