using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

using GymDomain.Model;

namespace GymInfrastructure;

public partial class DbgymContext : DbContext
{
    public DbgymContext()
    {
    }

    public DbgymContext(DbContextOptions<DbgymContext> options)
        : base(options)
    {
    }

    public virtual DbSet<GroupClass> GroupClasses { get; set; }

    public virtual DbSet<Gym> Gyms { get; set; }

    public virtual DbSet<Member> Members { get; set; }

    public virtual DbSet<Subscription> Subscriptions { get; set; }

    public virtual DbSet<Trainer> Trainers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=YehorLaptop; Database=DBGym; Trusted_Connection=True;  Trusted_Connection=True;TrustServerCertificate=True; MultipleActiveResultSets=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GroupClass>(entity =>
        {
            entity.Property(e => e.GymId).HasColumnName("GymID");
            entity.Property(e => e.Schedule)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(500)
                .IsUnicode(false);

            entity.HasOne(d => d.Gym).WithMany(p => p.GroupClasses)
                .HasForeignKey(d => d.GymId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Classes_Gyms");
        });

        modelBuilder.Entity<Gym>(entity =>
        {
            entity.Property(e => e.Adress)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Equipment)
                .HasMaxLength(1500)
                .IsUnicode(false);
            entity.Property(e => e.Schedule)
                .HasMaxLength(500)
                .IsUnicode(false);
            
        });

        modelBuilder.Entity<Member>(entity =>
        {
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Phone)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Subscription).WithMany(p => p.Members)
                .HasForeignKey(d => d.SubscriptionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Members_Subscriptions");
            entity.HasOne(d => d.Gym).WithMany(p => p.Members)
                .HasForeignKey(d => d.GymId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Members_Gyms");

            entity.HasMany(d => d.GroupClasses).WithMany(p => p.Members)
                .UsingEntity<Dictionary<string, object>>(
                    "MembersClass",
                    r => r.HasOne<GroupClass>().WithMany()
                        .HasForeignKey("ClassId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("FK_MembersClasses_Classes"),
                    l => l.HasOne<Member>().WithMany()
                        .HasForeignKey("MemberId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("FK_MembersClasses_Members"),
                    j =>
                    {
                        j.HasKey("MemberId", "ClassId");
                        j.ToTable("MembersClasses");
                    });
        });

        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.Property(e => e.Type)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Description)
                .HasMaxLength(1500)
                .IsUnicode(false);

            entity.HasOne(d => d.Gym).WithMany(p => p.Subscriptions)
                .HasForeignKey(d => d.GymId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Subscriptions_Gyms");
        });

        modelBuilder.Entity<Trainer>(entity =>
        {
            entity.Property(e => e.Email)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.GymId).HasColumnName("GymID");
            entity.Property(e => e.Phone)
                .HasMaxLength(1000)
                .IsUnicode(false);
            
            entity.Property(e => e.TrainerName)
                .HasMaxLength(500)
                .IsUnicode(false);

            entity.HasOne(d => d.Gym).WithMany(p => p.Trainers)
                .HasForeignKey(d => d.GymId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Trainers_Gyms");

            entity.HasMany(d => d.GroupClasses).WithMany(p => p.Trainers)
                .UsingEntity<Dictionary<string, object>>(
                    "TrainersClass",
                    r => r.HasOne<GroupClass>().WithMany()
                        .HasForeignKey("ClassId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("FK_TrainersClasses_Classes"),
                    l => l.HasOne<Trainer>().WithMany()
                        .HasForeignKey("TrainerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("FK_TrainersClasses_Trainers"),
                    j =>
                    {
                        j.HasKey("TrainerId", "ClassId");
                        j.ToTable("TrainersClasses");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
