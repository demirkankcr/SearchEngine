using Core.Domain.ComplexTypes.Enums;
using Core.Domain.Entities;
using Core.Persistence.Configurations.Base;
using Core.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Persistence.Configurations;

public class ContentConfiguration : BaseConfiguration<Content, Guid>
{
    public override void Configure(EntityTypeBuilder<Content> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Title).IsRequired().HasMaxLength(500).HasColumnName("Title");
        builder.Property(x => x.ProviderId).IsRequired().HasMaxLength(100).HasColumnName("ProviderId");
        builder.Property(x => x.Source).IsRequired().HasMaxLength(50).HasColumnName("Source");
        builder.Property(x => x.PublishedDate).IsRequired().HasColumnName("PublishedDate");
        builder.Property(x => x.Tags).HasColumnName("Tags");
        builder.Property(x => x.Score).HasColumnName("Score");
        builder.Property(x => x.BaseScore).HasColumnName("BaseScore");
        builder.Property(x => x.FreshnessScore).HasColumnName("FreshnessScore");
        builder.Property(x => x.InteractionScore).HasColumnName("InteractionScore");

        builder.HasDiscriminator<ContentType>(x => x.ContentType)
            .HasValue<VideoContent>(ContentType.Video)
            .HasValue<TextContent>(ContentType.Text);

        builder.ToTable(TableNameConstants.CONTENT);

        builder.HasIndex(x => x.Score); // sırlama indexi
        builder.HasIndex(x => x.PublishedDate); // güncellik indexi
        builder.HasIndex(x => x.ContentType); // filter
        builder.HasIndex(x => new { x.ProviderId, x.Source }).IsUnique(); // tekrara engelleme
    }
}

public class VideoContentConfiguration : IEntityTypeConfiguration<VideoContent>
{
    public void Configure(EntityTypeBuilder<VideoContent> builder)
    {
        builder.Property(x => x.Views).HasColumnName("Views");
        builder.Property(x => x.Likes).HasColumnName("Likes");
        builder.Property(x => x.Duration).HasMaxLength(20).HasColumnName("Duration");
    }
}

public class TextContentConfiguration : IEntityTypeConfiguration<TextContent>
{
    public void Configure(EntityTypeBuilder<TextContent> builder)
    {
        builder.Property(x => x.ReadingTime).HasColumnName("ReadingTime");
        builder.Property(x => x.Reactions).HasColumnName("Reactions");
        builder.Property(x => x.Comments).HasColumnName("Comments");
    }
}
