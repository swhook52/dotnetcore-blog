using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Domain;

namespace core_blog.domain.Migrations
{
    [DbContext(typeof(BloggingContext))]
    [Migration("20170104190752_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

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
        }
    }
}
