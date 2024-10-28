using Identity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Configurations;

public class UserFlowStpeConfiguration : IEntityTypeConfiguration<RequestFlowStpe>
{
    public void Configure(EntityTypeBuilder<RequestFlowStpe> builder)
    {
        builder.HasKey(x => x.Id);

        builder
            .HasOne(x => x.Request)
            .WithMany(x => x.RequestFlowStpes)
            .HasForeignKey(x => x.RequestId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}