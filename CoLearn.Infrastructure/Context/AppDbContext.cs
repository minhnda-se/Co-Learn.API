using System;
using System.Collections.Generic;
using CoLearn.Domain.Models;
using CoLearn.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace CoLearn.Infrastructure.Context;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Assignment> Assignments { get; set; }

    public virtual DbSet<AuditTrail> AuditTrails { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<BookingStatus> BookingStatuses { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<CourseCategory> CourseCategories { get; set; }

    public virtual DbSet<CourseMaterial> CourseMaterials { get; set; }

    public virtual DbSet<Enrollment> Enrollments { get; set; }

    public virtual DbSet<ErrorType> ErrorTypes { get; set; }

    public virtual DbSet<FileUpload> FileUploads { get; set; }

    public virtual DbSet<Lesson> Lessons { get; set; }

    public virtual DbSet<MaterialType> MaterialTypes { get; set; }

    public virtual DbSet<Parent> Parents { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Schedule> Schedules { get; set; }

    public virtual DbSet<ScheduleStatus> ScheduleStatuses { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<StudentProgress> StudentProgresses { get; set; }

    public virtual DbSet<Submission> Submissions { get; set; }

    public virtual DbSet<SystemLog> SystemLogs { get; set; }

    public virtual DbSet<Teacher> Teachers { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<TransactionStatus> TransactionStatuses { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserProfile> UserProfiles { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.;Database=CoLearnDB;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Assignment>(entity =>
        {
            entity.HasKey(e => e.AssignmentId).HasName("PK__Assignme__32499E57CE0C502C");

            entity.Property(e => e.AssignmentId).HasColumnName("AssignmentID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.LessonId).HasColumnName("LessonID");
            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasOne(d => d.Lesson).WithMany(p => p.Assignments)
                .HasForeignKey(d => d.LessonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Assignments_Lesson");
        });

        modelBuilder.Entity<AuditTrail>(entity =>
        {
            entity.HasKey(e => e.AuditId).HasName("PK__AuditTra__A17F23B8CDB7E63B");

            entity.ToTable("AuditTrail");

            entity.Property(e => e.AuditId).HasColumnName("AuditID");
            entity.Property(e => e.Action).HasMaxLength(50);
            entity.Property(e => e.PerformedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Pkvalue)
                .HasMaxLength(200)
                .HasColumnName("PKValue");
            entity.Property(e => e.TableName).HasMaxLength(200);

            entity.HasOne(d => d.PerformedByNavigation).WithMany(p => p.AuditTrails)
                .HasForeignKey(d => d.PerformedBy)
                .HasConstraintName("FK_Audit_PerformedBy");
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__Bookings__73951ACD96D2A993");

            entity.HasIndex(e => e.IsDeleted, "IX_Bookings_IsDeleted");

            entity.HasIndex(e => e.ScheduleId, "IX_Bookings_ScheduleID").HasFilter("([IsDeleted]=(0))");

            entity.HasIndex(e => e.StudentId, "IX_Bookings_StudentID").HasFilter("([IsDeleted]=(0))");

            entity.Property(e => e.BookingId).HasColumnName("BookingID");
            entity.Property(e => e.BookingStatusId).HasColumnName("BookingStatusID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Notes).HasMaxLength(1000);
            entity.Property(e => e.ScheduleId).HasColumnName("ScheduleID");
            entity.Property(e => e.StudentId).HasColumnName("StudentID");

            entity.HasOne(d => d.BookingStatus).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.BookingStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Bookings_Status");

            entity.HasOne(d => d.Schedule).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.ScheduleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Bookings_Schedule");

            entity.HasOne(d => d.Student).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Bookings_Student");
        });

        modelBuilder.Entity<BookingStatus>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__BookingS__C8EE204389F48E71");

            entity.ToTable("BookingStatus");

            entity.HasIndex(e => e.StatusName, "UQ__BookingS__05E7698A05F4ADC5").IsUnique();

            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.StatusName).HasMaxLength(50);
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.CourseId).HasName("PK__Courses__C92D718704B22F01");

            entity.HasIndex(e => e.IsDeleted, "IX_Courses_IsDeleted");

            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.ImageUrl).HasMaxLength(512);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Level).HasMaxLength(50);
            entity.Property(e => e.PricePerSession).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.ShortDescription).HasMaxLength(1024);
            entity.Property(e => e.TeacherId).HasColumnName("TeacherID");
            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasOne(d => d.Category).WithMany(p => p.Courses)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK_Courses_Category");

            entity.HasOne(d => d.Teacher).WithMany(p => p.Courses)
                .HasForeignKey(d => d.TeacherId)
                .HasConstraintName("FK_Courses_Teacher");
        });

        modelBuilder.Entity<CourseCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__CourseCa__19093A2B032EF677");

            entity.HasIndex(e => e.IsDeleted, "IX_CourseCategories_IsDeleted");

            entity.HasIndex(e => e.Name, "UQ__CourseCa__737584F6346E87AA").IsUnique();

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.Description).HasMaxLength(512);
            entity.Property(e => e.Name).HasMaxLength(150);
        });

        modelBuilder.Entity<CourseMaterial>(entity =>
        {
            entity.HasKey(e => e.MaterialId).HasName("PK__CourseMa__C5061317AC4E99C0");

            entity.HasIndex(e => e.IsDeleted, "IX_CourseMaterials_IsDeleted");

            entity.Property(e => e.MaterialId).HasColumnName("MaterialID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.LessonId).HasColumnName("LessonID");
            entity.Property(e => e.MaterialType).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.Url).HasMaxLength(1024);

            entity.HasOne(d => d.Course).WithMany(p => p.CourseMaterials)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK_Materials_Course");

            entity.HasOne(d => d.Lesson).WithMany(p => p.CourseMaterials)
                .HasForeignKey(d => d.LessonId)
                .HasConstraintName("FK_Materials_Lesson");
        });

        modelBuilder.Entity<Enrollment>(entity =>
        {
            entity.HasKey(e => e.EnrollmentId).HasName("PK__Enrollme__7F6877FBAD37DD61");

            entity.HasIndex(e => e.CourseId, "IX_Enrollments_CourseID").HasFilter("([IsDeleted]=(0))");

            entity.HasIndex(e => e.IsDeleted, "IX_Enrollments_IsDeleted");

            entity.HasIndex(e => e.StudentId, "IX_Enrollments_StudentID").HasFilter("([IsDeleted]=(0))");

            entity.HasIndex(e => new { e.StudentId, e.CourseId }, "UX_Enrollments_Student_Course").IsUnique();

            entity.Property(e => e.EnrollmentId).HasColumnName("EnrollmentID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.EnrolledAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.ProgressPercent).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Active");
            entity.Property(e => e.StudentId).HasColumnName("StudentID");

            entity.HasOne(d => d.Course).WithMany(p => p.Enrollments)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Enrollments_Course");

            entity.HasOne(d => d.Student).WithMany(p => p.Enrollments)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Enrollments_Student");
        });

        modelBuilder.Entity<ErrorType>(entity =>
        {
            entity.HasKey(e => e.ErrorTypeId).HasName("PK__ErrorTyp__9BB6BEEB60DDBF59");

            entity.HasIndex(e => e.ErrorName, "UQ__ErrorTyp__DB3189627CAD0829").IsUnique();

            entity.Property(e => e.ErrorTypeId).HasColumnName("ErrorTypeID");
            entity.Property(e => e.ErrorName).HasMaxLength(100);
        });

        modelBuilder.Entity<FileUpload>(entity =>
        {
            entity.HasKey(e => e.FileId).HasName("PK__FileUplo__6F0F989FDFBB14E2");

            entity.Property(e => e.FileId).HasColumnName("FileID");
            entity.Property(e => e.ContentType).HasMaxLength(200);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.FileName).HasMaxLength(512);
            entity.Property(e => e.FileUrl).HasMaxLength(1024);
            entity.Property(e => e.OwnerUserId).HasColumnName("OwnerUserID");
            entity.Property(e => e.RelatedId).HasColumnName("RelatedID");
            entity.Property(e => e.RelatedType).HasMaxLength(50);

            entity.HasOne(d => d.OwnerUser).WithMany(p => p.FileUploads)
                .HasForeignKey(d => d.OwnerUserId)
                .HasConstraintName("FK_FileUploads_User");
        });

        modelBuilder.Entity<Lesson>(entity =>
        {
            entity.HasKey(e => e.LessonId).HasName("PK__Lessons__B084ACB0743BA4B3");

            entity.HasIndex(e => e.IsDeleted, "IX_Lessons_IsDeleted");

            entity.Property(e => e.LessonId).HasColumnName("LessonID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.VideoUrl).HasMaxLength(512);

            entity.HasOne(d => d.Course).WithMany(p => p.Lessons)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Lessons_Course");
        });

        modelBuilder.Entity<MaterialType>(entity =>
        {
            entity.HasKey(e => e.MaterialTypeId).HasName("PK__Material__1621BBFFF558C474");

            entity.HasIndex(e => e.TypeName, "UQ__Material__D4E7DFA8558D3B4A").IsUnique();

            entity.Property(e => e.MaterialTypeId).HasColumnName("MaterialTypeID");
            entity.Property(e => e.TypeName).HasMaxLength(50);
        });

        modelBuilder.Entity<Parent>(entity =>
        {
            entity.HasKey(e => e.ParentId).HasName("PK__Parents__D339510FEB1385FC");

            entity.HasIndex(e => e.UserId, "UQ__Parents__1788CCADCA0A0EBE").IsUnique();

            entity.Property(e => e.ParentId).HasColumnName("ParentID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Relationship).HasMaxLength(50);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithOne(p => p.Parent)
                .HasForeignKey<Parent>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Parents_User");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payments__9B556A587961D29A");

            entity.HasIndex(e => e.BookingId, "IX_Payments_BookingID").HasFilter("([IsDeleted]=(0))");

            entity.HasIndex(e => e.EnrollmentId, "IX_Payments_EnrollmentID").HasFilter("([IsDeleted]=(0))");

            entity.HasIndex(e => e.IsDeleted, "IX_Payments_IsDeleted");

            entity.Property(e => e.PaymentId).HasColumnName("PaymentID");
            entity.Property(e => e.Amount).HasColumnType("decimal(14, 2)");
            entity.Property(e => e.BookingId).HasColumnName("BookingID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.EnrollmentId).HasColumnName("EnrollmentID");
            entity.Property(e => e.MethodId).HasColumnName("MethodID");
            entity.Property(e => e.PayerUserId).HasColumnName("PayerUserID");
            entity.Property(e => e.StatusId)
                .HasDefaultValue(1)
                .HasColumnName("StatusID");

            entity.HasOne(d => d.Booking).WithMany(p => p.Payments)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK_Payments_Booking");

            entity.HasOne(d => d.Enrollment).WithMany(p => p.Payments)
                .HasForeignKey(d => d.EnrollmentId)
                .HasConstraintName("FK_Payments_Enrollment");

            entity.HasOne(d => d.Method).WithMany(p => p.Payments)
                .HasForeignKey(d => d.MethodId)
                .HasConstraintName("FK_Payments_Method");

            entity.HasOne(d => d.Status).WithMany(p => p.Payments)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Payments_Status");
        });

        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.HasKey(e => e.MethodId).HasName("PK__PaymentM__FC681FB19CEA00C2");

            entity.HasIndex(e => e.MethodName, "UQ__PaymentM__218CFB178E4DF5B4").IsUnique();

            entity.Property(e => e.MethodId).HasColumnName("MethodID");
            entity.Property(e => e.MethodName).HasMaxLength(50);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE3AB08C8B66");

            entity.HasIndex(e => e.RoleName, "UQ__Roles__8A2B6160D7D3ABA3").IsUnique();

            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.RoleName).HasMaxLength(50);
            entity.Property(e => e.RoleType).HasMaxLength(20);
        });

        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.HasKey(e => e.ScheduleId).HasName("PK__Schedule__9C8A5B698C182157");

            entity.HasIndex(e => e.CourseId, "IX_Schedules_CourseID").HasFilter("([IsDeleted]=(0))");

            entity.HasIndex(e => e.TeacherId, "IX_Schedules_TeacherID").HasFilter("([IsDeleted]=(0))");

            entity.Property(e => e.ScheduleId).HasColumnName("ScheduleID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.MaxStudents).HasDefaultValue((byte)1);
            entity.Property(e => e.RecurrenceRule).HasMaxLength(200);
            entity.Property(e => e.ScheduleStatusId)
                .HasDefaultValue(1)
                .HasColumnName("ScheduleStatusID");
            entity.Property(e => e.TeacherId).HasColumnName("TeacherID");

            entity.HasOne(d => d.Course).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK_Schedules_Course");

            entity.HasOne(d => d.ScheduleStatus).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.ScheduleStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Schedules_Status");

            entity.HasOne(d => d.Teacher).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Schedules_Teacher");
        });

        modelBuilder.Entity<ScheduleStatus>(entity =>
        {
            entity.HasKey(e => e.ScheduleStatusId).HasName("PK__Schedule__3A1C0AC5416FB9A1");

            entity.ToTable("ScheduleStatus");

            entity.HasIndex(e => e.StatusName, "UQ__Schedule__05E7698A9734D58D").IsUnique();

            entity.Property(e => e.ScheduleStatusId).HasColumnName("ScheduleStatusID");
            entity.Property(e => e.StatusName).HasMaxLength(50);
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("PK__Students__32C52A790941E7FA");

            entity.HasIndex(e => e.IsDeleted, "IX_Students_IsDeleted");

            entity.HasIndex(e => e.ParentId, "IX_Students_Parent");

            entity.HasIndex(e => e.UserId, "UQ__Students__1788CCADE005F6F6").IsUnique();

            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.GradeLevel).HasMaxLength(50);
            entity.Property(e => e.ParentId).HasColumnName("ParentID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Parent).WithMany(p => p.Students)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("FK_Students_Parent");

            entity.HasOne(d => d.User).WithOne(p => p.Student)
                .HasForeignKey<Student>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Students_User");
        });

        modelBuilder.Entity<StudentProgress>(entity =>
        {
            entity.HasKey(e => e.ProgressId).HasName("PK__StudentP__BAE29C85B69C791B");

            entity.ToTable("StudentProgress");

            entity.HasIndex(e => e.EnrollmentId, "IX_StudentProgress_Enrollment");

            entity.Property(e => e.ProgressId).HasColumnName("ProgressID");
            entity.Property(e => e.EnrollmentId).HasColumnName("EnrollmentID");
            entity.Property(e => e.LessonId).HasColumnName("LessonID");
            entity.Property(e => e.Notes).HasMaxLength(1000);
            entity.Property(e => e.Score).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Enrollment).WithMany(p => p.StudentProgresses)
                .HasForeignKey(d => d.EnrollmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StudentProgress_Enrollment");

            entity.HasOne(d => d.Lesson).WithMany(p => p.StudentProgresses)
                .HasForeignKey(d => d.LessonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StudentProgress_Lesson");
        });

        modelBuilder.Entity<Submission>(entity =>
        {
            entity.HasKey(e => e.SubmissionId).HasName("PK__Submissi__449EE105F864A30F");

            entity.HasIndex(e => e.AssignmentId, "IX_Submissions_AssignmentID");

            entity.HasIndex(e => e.StudentId, "IX_Submissions_StudentID");

            entity.Property(e => e.SubmissionId).HasColumnName("SubmissionID");
            entity.Property(e => e.AssignmentId).HasColumnName("AssignmentID");
            entity.Property(e => e.FilePath).HasMaxLength(512);
            entity.Property(e => e.Grade).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.SubmittedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Assignment).WithMany(p => p.Submissions)
                .HasForeignKey(d => d.AssignmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Submissions_Assignment");

            entity.HasOne(d => d.Student).WithMany(p => p.Submissions)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Submissions_Student");
        });

        modelBuilder.Entity<SystemLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK__SystemLo__5E5499A8F873DF37");

            entity.Property(e => e.LogId).HasColumnName("LogID");
            entity.Property(e => e.Action).HasMaxLength(200);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.ErrorTypeId).HasColumnName("ErrorTypeID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.ErrorType).WithMany(p => p.SystemLogs)
                .HasForeignKey(d => d.ErrorTypeId)
                .HasConstraintName("FK_SystemLogs_ErrorType");

            entity.HasOne(d => d.User).WithMany(p => p.SystemLogs)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_SystemLogs_User");
        });

        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.HasKey(e => e.TeacherId).HasName("PK__Teachers__EDF25944D99F4D48");

            entity.HasIndex(e => e.IsDeleted, "IX_Teachers_IsDeleted");

            entity.HasIndex(e => e.VerificationStatus, "IX_Teachers_Verification");

            entity.HasIndex(e => e.UserId, "UQ__Teachers__1788CCAD498C8D07").IsUnique();

            entity.Property(e => e.TeacherId).HasColumnName("TeacherID");
            entity.Property(e => e.AvgRating).HasColumnType("decimal(3, 2)");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.HourlyRate).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.Qualification).HasMaxLength(512);
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.VerificationStatus)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");

            entity.HasOne(d => d.User).WithOne(p => p.Teacher)
                .HasForeignKey<Teacher>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Teachers_User");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("PK__Transact__55433A4B4EB77133");

            entity.Property(e => e.TransactionId).HasColumnName("TransactionID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.GatewayName).HasMaxLength(100);
            entity.Property(e => e.GatewayTransactionCode).HasMaxLength(200);
            entity.Property(e => e.PaymentId).HasColumnName("PaymentID");
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.Payment).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.PaymentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transactions_Payment");
        });

        modelBuilder.Entity<TransactionStatus>(entity =>
        {
            entity.HasKey(e => e.TransactionStatusId).HasName("PK__Transact__57B5E1A3EA0B6B38");

            entity.ToTable("TransactionStatus");

            entity.HasIndex(e => e.StatusName, "UQ__Transact__05E7698A74153817").IsUnique();

            entity.Property(e => e.TransactionStatusId).HasColumnName("TransactionStatusID");
            entity.Property(e => e.StatusName).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC2BEF87C2");

            entity.HasIndex(e => e.Email, "IX_Users_Email");

            entity.HasIndex(e => e.IsDeleted, "IX_Users_IsDeleted");

            entity.HasIndex(e => e.PrimaryRoleId, "IX_Users_PrimaryRole");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D105345FD3A507").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FullName).HasMaxLength(200);
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.PrimaryRoleId).HasColumnName("PrimaryRoleID");

            entity.HasOne(d => d.PrimaryRole).WithMany(p => p.Users)
                .HasForeignKey(d => d.PrimaryRoleId)
                .HasConstraintName("FK_Users_PrimaryRole");
        });

        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.HasKey(e => e.ProfileId).HasName("PK__UserProf__290C88843D6AEA8F");

            entity.HasIndex(e => e.UserId, "UQ__UserProf__1788CCADFF042AF8").IsUnique();

            entity.Property(e => e.ProfileId).HasColumnName("ProfileID");
            entity.Property(e => e.Address).HasMaxLength(512);
            entity.Property(e => e.AvatarUrl).HasMaxLength(1024);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithOne(p => p.UserProfile)
                .HasForeignKey<UserProfile>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserProfiles_User");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.RoleId }).HasName("PK__UserRole__AF27604FD9CB7DC3");

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.AssignedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserRoles_Role");

            entity.HasOne(d => d.User).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserRoles_User");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
