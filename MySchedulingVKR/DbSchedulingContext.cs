using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using MySchedulingVKR.Models;

namespace MySchedulingVKR;

public partial class DbSchedulingContext : DbContext
{
    public DbSchedulingContext()
    {
    }

    public DbSchedulingContext(DbContextOptions<DbSchedulingContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Lesson> Lessons { get; set; }

    public virtual DbSet<Organization> Organizations { get; set; }

    public virtual DbSet<OrganizationSubject> OrganizationSubjects { get; set; }

    public virtual DbSet<Schedule> Schedules { get; set; }

    public virtual DbSet<ScheduleStudent> ScheduleStudents { get; set; }

    public virtual DbSet<ScheduleTeacher> ScheduleTeachers { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<Subject> Subjects { get; set; }

    public virtual DbSet<Teacher> Teachers { get; set; }

    public virtual DbSet<TeacherAccess> TeacherAccesses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-SD0RFQA;Database=db_Scheduling;TrustServerCertificate=True;Trusted_Connection=True;MultipleActiveResultSets=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Lesson>(entity =>
        {
            entity.ToTable("Lesson");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.DayOfTheWeek)
                .HasMaxLength(50)
                .HasColumnName("Day_of_the_week");
            entity.Property(e => e.Time).HasMaxLength(50);
        });

        modelBuilder.Entity<Organization>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_admins");

            entity.ToTable("Organization");

            entity.HasIndex(e => e.Login, "UQ__Organiza__5E55825BD91BEE29").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Login).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(50);
            entity.Property(e => e.Specialization).HasMaxLength(50);
        });

        modelBuilder.Entity<OrganizationSubject>(entity =>
        {
            entity
                .HasKey(e => new { e.SubjectId, e.OrganizationId }).HasName("PK_Organization_Subject");
                
            entity.ToTable("Organization_Subject");

            entity.Property(e => e.OrganizationId).HasColumnName("OrganizationID");
            entity.Property(e => e.SubjectId).HasColumnName("SubjectID");
            
            entity.HasOne(e => e.Organization)
                  .WithMany(o =>  o.OrganizationSubjects)
                  .HasForeignKey(e => e.OrganizationId)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_Organization_Subject_Organization");

            entity.HasOne(d => d.Subject)
                  .WithMany(s => s.OrganizationSubjects)
                  .HasForeignKey(d => d.SubjectId)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_Organization_Subject_Subject");
        });

        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.HasKey(e => e.LessonId).HasName("PK_schedule");

            entity.ToTable("Schedule");

            entity.Property(e => e.LessonId).HasColumnName("LessonID");

            entity.HasOne(d => d.Lesson).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.LessonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Schedule_Lesson");
        });

        modelBuilder.Entity<ScheduleStudent>(entity =>
        {
            entity
                .HasKey(e => new { e.ScheduleId, e.StudentId }).HasName("PK_Schedule_Student");

            entity
                .ToTable("Schedule_Student");

            entity.Property(e => e.ScheduleId).HasColumnName("ScheduleID");
            entity.Property(e => e.StudentId).HasColumnName("StudentID");

            entity.HasOne(d => d.Schedule)
                .WithMany(s => s.ScheduleStudents)
                .HasForeignKey(d => d.ScheduleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Schedule_Student_Schedule");

            entity.HasOne(d => d.Student)
                .WithMany(s => s.ScheduleStudents)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Schedule_Student_Student");
        });

        modelBuilder.Entity<ScheduleTeacher>(entity =>
        {
            entity
                .HasKey(e => new { e.ScheduleId, e.TeacherId }).HasName("PK_Schedule_Teacher");

            entity
                .ToTable("Schedule_Teacher");

            entity.Property(e => e.ScheduleId).HasColumnName("ScheduleID");
            entity.Property(e => e.TeacherId).HasColumnName("TeacherID");

            entity.HasOne(d => d.Schedule)
                .WithMany(s => s.ScheduleTeachers)
                .HasForeignKey(d => d.ScheduleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Schedule_Teacher_Schedule");

            entity.HasOne(d => d.Teacher)
                .WithMany(t => t.ScheduleTeachers)
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Schedule_Teacher_Teacher");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_students");

            entity.ToTable("Student");

            entity.HasIndex(e => e.Login, "UQ__Student__5E55825BF3F86D30").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Login).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(50);
            entity.Property(e => e.Surname).HasMaxLength(50);
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_subjects");

            entity.ToTable("Subject");

            entity.HasIndex(e => e.Name, "UQ__Subject__737584F6BA62E295").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.ToTable("Teacher");

            entity.HasIndex(e => e.Login, "UQ__Teacher__5E55825B02D95DD5").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Login).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.OrganizationId).HasColumnName("OrganizationID");
            entity.Property(e => e.Password).HasMaxLength(50);
            entity.Property(e => e.SubjectId).HasColumnName("SubjectID");
            entity.Property(e => e.Surname).HasMaxLength(50);

            entity.HasOne(d => d.Organization).WithMany(p => p.Teachers)
                .HasForeignKey(d => d.OrganizationId)
                .HasConstraintName("FK_teachers_organization");

            entity.HasOne(d => d.Subject).WithMany(p => p.Teachers)
                .HasForeignKey(d => d.SubjectId)
                .HasConstraintName("FK_teachers_subjects");
        });

        modelBuilder.Entity<TeacherAccess>(entity =>
        {
            entity
               .HasKey(e => new { e.TeacherId, e.LessonId }).HasName("PK_Teacher_access");

            entity.ToTable("Teacher_access");

            entity.Property(e => e.TeacherId).HasColumnName("TeacherID");
            entity.Property(e => e.LessonId).HasColumnName("LessonID");

            entity.HasOne(d => d.Teacher)
                .WithMany(t => t.TeacherAccesses)
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Teacher_access_Teacher");

            entity.HasOne(d => d.Lesson)
                .WithMany(t => t.TeacherAccesses)
                .HasForeignKey(d => d.LessonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Teacher_access_Lesson");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
