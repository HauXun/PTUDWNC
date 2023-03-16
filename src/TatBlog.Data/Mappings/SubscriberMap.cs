using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TatBlog.Core.Entities;

namespace TatBlog.Data.Mappings;

public class SubscriberMap : IEntityTypeConfiguration<Subscriber>
{
  public void Configure(EntityTypeBuilder<Subscriber> builder)
  {
    // Table name
    builder.ToTable("Subscribers");

    // Primary key
    builder.HasKey(p => p.Id);

    // Fields
    builder.Property(p => p.SubscribeEmail)
           .IsRequired()
           .HasMaxLength(500);
    builder.Property(p => p.SubDated)
           .IsRequired()
           .HasColumnType("datetime");
    builder.Property(p => p.UnSubDated)
           .HasColumnType("datetime");
    builder.Property(p => p.CancelReason)
           .HasMaxLength(5000);
    builder.Property(p => p.ForceLock)
           .HasDefaultValue(false);
    builder.Property(p => p.UnsubscribeVoluntary)
           .HasDefaultValue(false);
    builder.Property(p => p.AdminNotes)
           .HasMaxLength(5000);
  }
}