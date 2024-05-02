﻿// <auto-generated />
using System;
using GymInfrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace GymInfrastructure.Migrations
{
    [DbContext(typeof(DbgymContext))]
    [Migration("20240412232305_RemoveAvaliabilityField12")]
    partial class RemoveAvaliabilityField12
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("GymDomain.Model.GroupClass", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("GymId")
                        .HasColumnType("int")
                        .HasColumnName("GymID");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(500)
                        .IsUnicode(false)
                        .HasColumnType("varchar(500)");

                    b.Property<int>("Price")
                        .HasColumnType("int");

                    b.Property<int>("Room")
                        .HasColumnType("int");

                    b.Property<string>("Schedule")
                        .IsRequired()
                        .HasMaxLength(500)
                        .IsUnicode(false)
                        .HasColumnType("varchar(500)");

                    b.HasKey("Id");

                    b.HasIndex("GymId");

                    b.ToTable("GroupClasses");
                });

            modelBuilder.Entity("GymDomain.Model.Gym", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Adress")
                        .IsRequired()
                        .HasMaxLength(500)
                        .IsUnicode(false)
                        .HasColumnType("varchar(500)");

                    b.Property<string>("Equipment")
                        .IsRequired()
                        .HasMaxLength(1500)
                        .IsUnicode(false)
                        .HasColumnType("varchar(1500)");

                    b.Property<string>("Schedule")
                        .IsRequired()
                        .HasMaxLength(500)
                        .IsUnicode(false)
                        .HasColumnType("varchar(500)");

                    b.HasKey("Id");

                    b.ToTable("Gyms");
                });

            modelBuilder.Entity("GymDomain.Model.Member", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)");

                    b.Property<int>("GymId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(500)
                        .IsUnicode(false)
                        .HasColumnType("varchar(500)");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)");

                    b.Property<int?>("SubscriptionId")
                        .IsRequired()
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("GymId");

                    b.HasIndex("SubscriptionId");

                    b.ToTable("Members");
                });

            modelBuilder.Entity("GymDomain.Model.Subscription", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(1500)
                        .IsUnicode(false)
                        .HasColumnType("varchar(1500)");

                    b.Property<int>("Duration")
                        .HasColumnType("int");

                    b.Property<int>("GymId")
                        .HasColumnType("int");

                    b.Property<int>("Price")
                        .HasColumnType("int");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(150)
                        .IsUnicode(false)
                        .HasColumnType("varchar(150)");

                    b.HasKey("Id");

                    b.HasIndex("GymId");

                    b.ToTable("Subscriptions");
                });

            modelBuilder.Entity("GymDomain.Model.Trainer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .IsUnicode(false)
                        .HasColumnType("varchar(1000)");

                    b.Property<int>("GymId")
                        .HasColumnType("int")
                        .HasColumnName("GymID");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .IsUnicode(false)
                        .HasColumnType("varchar(1000)");

                    b.Property<string>("TrainerName")
                        .IsRequired()
                        .HasMaxLength(500)
                        .IsUnicode(false)
                        .HasColumnType("varchar(500)");

                    b.HasKey("Id");

                    b.HasIndex("GymId");

                    b.ToTable("Trainers");
                });

            modelBuilder.Entity("MembersClass", b =>
                {
                    b.Property<int>("MemberId")
                        .HasColumnType("int");

                    b.Property<int>("ClassId")
                        .HasColumnType("int");

                    b.HasKey("MemberId", "ClassId");

                    b.HasIndex("ClassId");

                    b.ToTable("MembersClasses", (string)null);
                });

            modelBuilder.Entity("TrainersClass", b =>
                {
                    b.Property<int>("TrainerId")
                        .HasColumnType("int");

                    b.Property<int>("ClassId")
                        .HasColumnType("int");

                    b.HasKey("TrainerId", "ClassId");

                    b.HasIndex("ClassId");

                    b.ToTable("TrainersClasses", (string)null);
                });

            modelBuilder.Entity("GymDomain.Model.GroupClass", b =>
                {
                    b.HasOne("GymDomain.Model.Gym", "Gym")
                        .WithMany("GroupClasses")
                        .HasForeignKey("GymId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_Classes_Gyms");

                    b.Navigation("Gym");
                });

            modelBuilder.Entity("GymDomain.Model.Member", b =>
                {
                    b.HasOne("GymDomain.Model.Gym", "Gym")
                        .WithMany("Members")
                        .HasForeignKey("GymId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_Members_Gyms");

                    b.HasOne("GymDomain.Model.Subscription", "Subscription")
                        .WithMany("Members")
                        .HasForeignKey("SubscriptionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_Members_Subscriptions");

                    b.Navigation("Gym");

                    b.Navigation("Subscription");
                });

            modelBuilder.Entity("GymDomain.Model.Subscription", b =>
                {
                    b.HasOne("GymDomain.Model.Gym", "Gym")
                        .WithMany("Subscriptions")
                        .HasForeignKey("GymId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_Subscriptions_Gyms");

                    b.Navigation("Gym");
                });

            modelBuilder.Entity("GymDomain.Model.Trainer", b =>
                {
                    b.HasOne("GymDomain.Model.Gym", "Gym")
                        .WithMany("Trainers")
                        .HasForeignKey("GymId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_Trainers_Gyms");

                    b.Navigation("Gym");
                });

            modelBuilder.Entity("MembersClass", b =>
                {
                    b.HasOne("GymDomain.Model.GroupClass", null)
                        .WithMany()
                        .HasForeignKey("ClassId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_MembersClasses_Classes");

                    b.HasOne("GymDomain.Model.Member", null)
                        .WithMany()
                        .HasForeignKey("MemberId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_MembersClasses_Members");
                });

            modelBuilder.Entity("TrainersClass", b =>
                {
                    b.HasOne("GymDomain.Model.GroupClass", null)
                        .WithMany()
                        .HasForeignKey("ClassId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_TrainersClasses_Classes");

                    b.HasOne("GymDomain.Model.Trainer", null)
                        .WithMany()
                        .HasForeignKey("TrainerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_TrainersClasses_Trainers");
                });

            modelBuilder.Entity("GymDomain.Model.Gym", b =>
                {
                    b.Navigation("GroupClasses");

                    b.Navigation("Members");

                    b.Navigation("Subscriptions");

                    b.Navigation("Trainers");
                });

            modelBuilder.Entity("GymDomain.Model.Subscription", b =>
                {
                    b.Navigation("Members");
                });
#pragma warning restore 612, 618
        }
    }
}
