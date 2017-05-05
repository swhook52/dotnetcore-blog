using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Domain;

namespace core_blog.domain.Migrations
{
    [DbContext(typeof(BloggingContext))]
    [Migration("20170505064552_AddTagsToPosts")]
    partial class AddTagsToPosts
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.1")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Domain.Comment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateCreated");

                    b.Property<string>("Email")
                        .IsRequired();

                    b.Property<string>("Message")
                        .IsRequired();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int>("PostId");

                    b.HasKey("Id");

                    b.HasIndex("PostId");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("Domain.Post", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Body");

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime?>("DatePublished");

                    b.Property<bool>("IsFeatured");

                    b.Property<bool>("IsStatic");

                    b.Property<string>("Slug")
                        .IsRequired();

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("Domain.PostTag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("PostId");

                    b.Property<string>("TagName");

                    b.HasKey("Id");

                    b.HasIndex("PostId");

                    b.HasIndex("TagName");

                    b.ToTable("PostTags");
                });

            modelBuilder.Entity("Domain.Tag", b =>
                {
                    b.Property<string>("Name")
                        .ValueGeneratedOnAdd();

                    b.HasKey("Name");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("Domain.Comment", b =>
                {
                    b.HasOne("Domain.Post", "Post")
                        .WithMany("Comments")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Domain.PostTag", b =>
                {
                    b.HasOne("Domain.Post", "Post")
                        .WithMany("PostTags")
                        .HasForeignKey("PostId");

                    b.HasOne("Domain.Tag", "Tag")
                        .WithMany("Posts")
                        .HasForeignKey("TagName");
                });
        }
    }
}
