using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniERP.Web.Domain.Entities;

namespace MiniERP.Web.Infrastructure.Persistence.Configurations;

internal sealed class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("Department", "dbo");
        builder.HasKey(department => department.DepartmentId).HasName("PK_Department");
        builder.Property(department => department.DepartmentId).HasColumnName("DepartmentID");
        builder.Property(department => department.DepartmentName).HasMaxLength(30).IsRequired();
        builder.Property(department => department.DeletedAt).HasPrecision(3);
        builder.HasIndex(department => department.DepartmentName)
            .IsUnique()
            .HasDatabaseName("UX_Department_Name_Active")
            .HasFilter("[DeletedAt] IS NULL");
        builder.HasQueryFilter(department => department.DeletedAt == null);
    }
}
