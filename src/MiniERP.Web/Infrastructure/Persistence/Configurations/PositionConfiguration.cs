using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniERP.Web.Domain.Entities;

namespace MiniERP.Web.Infrastructure.Persistence.Configurations;

internal sealed class PositionConfiguration : IEntityTypeConfiguration<Position>
{
    public void Configure(EntityTypeBuilder<Position> builder)
    {
        builder.ToTable("Position", "dbo");
        builder.HasKey(position => position.PositionId).HasName("PK_Position");
        builder.Property(position => position.PositionId).HasColumnName("PositionID");
        builder.Property(position => position.PositionName).HasMaxLength(30).IsRequired();
        builder.Property(position => position.DeletedAt).HasPrecision(3);
        builder.HasIndex(position => position.PositionName)
            .IsUnique()
            .HasDatabaseName("UX_Position_Name_Active")
            .HasFilter("[DeletedAt] IS NULL");
        builder.HasQueryFilter(position => position.DeletedAt == null);
    }
}