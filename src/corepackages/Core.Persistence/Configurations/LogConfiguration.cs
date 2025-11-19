using Core.Domain.Entities;
using Core.Persistence.Configurations.Base;
using Core.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Persistence.Configurations;

public class LogConfiguration : BaseConfiguration<Log, Guid>
{
    public override void Configure(EntityTypeBuilder<Log> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.EventId).IsRequired(false).HasColumnName("EventId").HasMaxLength(36);
        builder.Property(x => x.LogDomain).IsRequired(false).HasColumnName("LogDomain").HasMaxLength(150);
        builder.Property(x => x.UserId).IsRequired(false).HasColumnName("UserId").HasMaxLength(20);
        builder.Property(x => x.LogDate).HasColumnName("LogDate");
        builder.Property(x => x.Host).IsRequired(false).HasColumnName("Host").HasMaxLength(150);
        builder.Property(x => x.Path).IsRequired(false).HasColumnName("Path").HasMaxLength(150);
        builder.Property(x => x.Scheme).IsRequired(false).HasColumnName("Scheme").HasMaxLength(50);
        builder.Property(x => x.QueryString).IsRequired(false).HasColumnName("QueryString").HasMaxLength(255);
        builder.Property(x => x.RemoteIp).IsRequired(false).HasColumnName("RemoteIp").HasMaxLength(50);
        builder.Property(x => x.Headers).IsRequired(false).HasColumnName("Headers").HasColumnType(LengthConstraints.MAX);
        builder.Property(x => x.ResponseHeaders).IsRequired(false).HasColumnName("ResponseHeaders").HasColumnType(LengthConstraints.MAX);
        builder.Property(x => x.RequestMethod).IsRequired(false).HasColumnName("RequestMethod").HasColumnType(LengthConstraints.MAX);
        builder.Property(x => x.UserAgent).IsRequired(false).HasColumnName("UserAgent").HasColumnType(LengthConstraints.MAX);
        builder.Property(x => x.RequestBody).IsRequired(false).HasColumnName("RequestBody").HasColumnType(LengthConstraints.MAX);
        builder.Property(x => x.ResponseBody).IsRequired(false).HasColumnName("ResponseBody").HasColumnType(LengthConstraints.MAX);
        builder.Property(x => x.Exception).IsRequired(false).HasColumnName("Exception").HasColumnType(LengthConstraints.MAX);
        builder.Property(x => x.ExceptionMessage).IsRequired(false).HasColumnName("ExceptionMessage").HasColumnType(LengthConstraints.MAX);
        builder.Property(x => x.InnerException).IsRequired(false).HasColumnName("InnerException").HasColumnType(LengthConstraints.MAX);
        builder.Property(x => x.InnerExceptionMessage).IsRequired(false).HasColumnName("InnerExceptionMessage").HasColumnType(LengthConstraints.MAX);
        builder.Property(x => x.StatusCode).IsRequired(false).HasColumnName("StatusCode");
        builder.Property(x => x.ResponseTime).IsRequired(false).HasColumnName("ResponseTime");
        builder.Property(x => x.GetLog).IsRequired(false).HasColumnName("GetLog").HasColumnType(LengthConstraints.MAX);
        builder.Property(x => x.GetErrorLog).IsRequired(false).HasColumnName("GetErrorLog").HasColumnType(LengthConstraints.MAX);

        builder.ToTable(TableNameConstants.LOG);
    }
}

