using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace APIs.Models;

public partial class ApiDbContext : DbContext
{
    public ApiDbContext()
    {
    }

    public ApiDbContext(DbContextOptions<ApiDbContext> options)
        : base(options)
    {
    }

    //public virtual DbSet<BilledEvent> BilledEvents { get; set; }

    //public virtual DbSet<ClinicService> ClinicServices { get; set; }

    //public virtual DbSet<CptCode> CptCodes { get; set; }

    public virtual DbSet<Provider> Providers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=::1;Initial Catalog=CS4710;Integrated Security=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //modelBuilder.Entity<BilledEvent>(entity =>
        //{
        //    entity.HasKey(e => e.EventId).HasName("PK__Billed_E__7944C8701746CD2C");

        //    entity.ToTable("Billed_Events");

        //    entity.Property(e => e.EventId).HasColumnName("EventID");
        //    entity.Property(e => e.BilledAmount).HasColumnType("decimal(10, 2)");
        //    entity.Property(e => e.ProviderId).HasColumnName("ProviderID");
        //    entity.Property(e => e.ServiceId).HasColumnName("ServiceID");

        //    entity.HasOne(d => d.Provider).WithMany(p => p.BilledEvents)
        //        .HasForeignKey(d => d.ProviderId)
        //        .OnDelete(DeleteBehavior.ClientSetNull)
        //        .HasConstraintName("FK__Billed_Ev__Provi__48CFD27E");

        //    entity.HasOne(d => d.Service).WithMany(p => p.BilledEvents)
        //        .HasForeignKey(d => d.ServiceId)
        //        .OnDelete(DeleteBehavior.ClientSetNull)
        //        .HasConstraintName("FK__Billed_Ev__Servi__47DBAE45");
        //});

        //modelBuilder.Entity<ClinicService>(entity =>
        //{
        //    entity.HasKey(e => e.ServiceId).HasName("PK__Clinic_S__C51BB0EAA37D9982");

        //    entity.ToTable("Clinic_Services");

        //    entity.Property(e => e.ServiceId).HasColumnName("ServiceID");
        //    entity.Property(e => e.Cptcode)
        //        .HasMaxLength(10)
        //        .IsUnicode(false)
        //        .HasColumnName("CPTCode");
        //    entity.Property(e => e.Fee).HasColumnType("decimal(10, 2)");
        //    entity.Property(e => e.ServiceName).HasMaxLength(255);

        //    entity.HasOne(d => d.CptcodeNavigation).WithMany(p => p.ClinicServices)
        //        .HasForeignKey(d => d.Cptcode)
        //        .OnDelete(DeleteBehavior.ClientSetNull)
        //        .HasConstraintName("FK__Clinic_Se__CPTCo__44FF419A");
        //});

        //modelBuilder.Entity<CptCode>(entity =>
        //{
        //    entity.HasKey(e => e.Cptcode).HasName("PK__CPT_Code__44DA3F2E42C36988");

        //    entity.ToTable("CPT_Codes");

        //    entity.Property(e => e.Cptcode)
        //        .HasMaxLength(10)
        //        .IsUnicode(false)
        //        .HasColumnName("CPTCode");
        //    entity.Property(e => e.Category).HasMaxLength(100);
        //});

        modelBuilder.Entity<Provider>(entity =>
        {
            entity.HasKey(e => e.ProviderId).HasName("PK__Provider__B54C689D8B8AA7E6");

            entity.Property(e => e.ProviderId).HasColumnName("ProviderID");
            entity.Property(e => e.Addrss).HasMaxLength(150);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.Specialty).HasMaxLength(150);

            //entity.HasMany(d => d.Services).WithMany(p => p.Providers)
            //    .UsingEntity<Dictionary<string, object>>(
            //        "ProviderCapability",
            //        r => r.HasOne<ClinicService>().WithMany()
            //            .HasForeignKey("ServiceId")
            //            .HasConstraintName("FK__Provider___Servi__4CA06362"),
            //        l => l.HasOne<Provider>().WithMany()
            //            .HasForeignKey("ProviderId")
            //            .HasConstraintName("FK__Provider___Provi__4BAC3F29"),
            //        j =>
            //        {
            //            j.HasKey("ProviderId", "ServiceId").HasName("PK__Provider__091DD393FAC270D3");
            //            j.ToTable("Provider_Capabilities");
            //            j.IndexerProperty<int>("ProviderId").HasColumnName("ProviderID");
            //            j.IndexerProperty<int>("ServiceId").HasColumnName("ServiceID");
            //        });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
