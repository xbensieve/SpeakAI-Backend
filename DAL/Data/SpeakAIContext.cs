using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Data
{
    public class SpeakAIContext : DbContext
    {
        public SpeakAIContext(DbContextOptions<SpeakAIContext> options)
            : base(options)
        {
        }

        public DbSet<Level> Levels { get; set; }
        public DbSet<UserLevel> UserLevels { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<EnrolledCourse> EnrolledCourses { get; set; }
        public DbSet<TopicProgress> TopicProgresses { get; set; }
        [NotNull]
        public DbSet<ExerciseProgress> ExerciseProgresses { get; set; }

        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<ChatMessages> ChatMessages { get; set; }

        public DbSet<Voucher> Vouchers { get; set; }

        public DbSet<PaymentHistory> PaymentHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 🔥 Cấu hình COLLATION mặc định cho toàn bộ các bảng và cột
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                // Đặt collation cho tất cả các cột string
                foreach (var property in entityType.GetProperties()
                    .Where(p => p.ClrType == typeof(string) && p.GetColumnType() == null))
                {
                    property.SetCollation("utf8mb4_unicode_ci");
                }

                // Đặt collation cho các cột GUID (CHAR(36))
                foreach (var property in entityType.GetProperties()
                    .Where(p => p.ClrType == typeof(Guid) || p.ClrType == typeof(Guid?)))
                {
                    property.SetColumnType("char(36)");
                    property.SetCollation("utf8mb4_unicode_ci");
                }
            }

            // Cấu hình các bảng và khóa chính
            modelBuilder.Entity<Course>().HasKey(c => c.Id);
            modelBuilder.Entity<User>().HasKey(u => u.Id);
            modelBuilder.Entity<EnrolledCourse>().HasKey(u => u.Id);
            modelBuilder.Entity<ExerciseProgress>().HasKey(u => u.Id);
            modelBuilder.Entity<Exercise>().HasKey(u => u.Id);
            modelBuilder.Entity<Topic>().HasKey(u => u.Id);
            modelBuilder.Entity<UserLevel>().HasKey(u => u.Id);
            modelBuilder.Entity<TopicProgress>().HasKey(u => u.Id);
            modelBuilder.Entity<RefreshToken>().HasKey(u => u.Id);
            modelBuilder.Entity<Transaction>().HasKey(u => u.Id);

            // Cấu hình các cột decimal
            modelBuilder.Entity<Course>()
                .Property(c => c.MaxPoint)
                .HasPrecision(18, 2);

            modelBuilder.Entity<EnrolledCourse>()
                .Property(ec => ec.ProgressPoints)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Exercise>()
                .Property(e => e.MaxPoint)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ExerciseProgress>()
                .Property(ep => ep.ProgressPoints)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Level>()
                .Property(l => l.MaxPoint)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Level>()
                .Property(l => l.MinPoint)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Topic>()
                .Property(t => t.MaxPoint)
                .HasPrecision(18, 2);

            modelBuilder.Entity<TopicProgress>()
                .Property(tp => tp.ProgressPoints)
                .HasPrecision(18, 2);

            modelBuilder.Entity<UserLevel>()
                .Property(ul => ul.Point)
                .HasPrecision(18, 2);

            // Cấu hình quan hệ giữa các bảng
            modelBuilder.Entity<Course>()
                .HasOne(c => c.Level)
                .WithMany(l => l.Courses)
                .HasForeignKey(c => c.LevelId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Topic>()
                .HasOne(t => t.Course)
                .WithMany(c => c.Topics)
                .HasForeignKey(t => t.CourseId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Exercise>()
                .HasOne(e => e.Topic)
                .WithMany(t => t.Exercises)
                .HasForeignKey(e => e.TopicId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<EnrolledCourse>()
                .HasOne(ec => ec.User)
                .WithMany(u => u.EnrolledCourses)
                .HasForeignKey(ec => ec.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<EnrolledCourse>()
                .HasOne(ec => ec.Course)
                .WithMany(c => c.EnrolledCourses)
                .HasForeignKey(ec => ec.CourseId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TopicProgress>()
                .HasOne(tp => tp.User)
                .WithMany(u => u.TopicProgresses)
                .HasForeignKey(tp => tp.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TopicProgress>()
                .HasOne(tp => tp.Topic)
                .WithMany(t => t.TopicProgresses)
                .HasForeignKey(tp => tp.TopicId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TopicProgress>()
                .HasOne(tp => tp.EnrolledCourse)
                .WithMany(ec => ec.TopicProgresses)
                .HasForeignKey(tp => tp.EnrolledCourseId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ExerciseProgress>()
                .HasOne(ep => ep.User)
                .WithMany(u => u.ExerciseProgresses)
                .HasForeignKey(ep => ep.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ExerciseProgress>()
                .HasOne(ep => ep.Exercise)
                .WithMany(e => e.ExerciseProgresses)
                .HasForeignKey(ep => ep.ExerciseId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ExerciseProgress>()
                .HasOne(ep => ep.EnrolledCourse)
                .WithMany()
                .HasForeignKey(ep => ep.EnrolledCourseId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Voucher>()
                .HasKey(v => v.VoucherId);

            modelBuilder.Entity<Voucher>()
                .Property(v => v.VoucherCode)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Voucher>()
                .Property(v => v.Description)
                .HasMaxLength(200);

            modelBuilder.Entity<Voucher>()
                .Property(v => v.DiscountPercentage)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Voucher>()
                .Property(v => v.IsActive)
                .HasDefaultValue(true);

            modelBuilder.Entity<Voucher>()
                .Property(v => v.StartDate)
                .HasColumnType("datetime");

            modelBuilder.Entity<Voucher>()
                .Property(v => v.EndDate)
                .HasColumnType("datetime");

            modelBuilder.Entity<Voucher>()
                .Property(v => v.MinPurchaseAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Voucher>()
                .Property(v => v.VoucherType)
                .HasMaxLength(50);

            modelBuilder.Entity<Voucher>()
                .HasOne(v => v.User)
                .WithMany(u => u.Voucher)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Transaction>()
                .Property(t => t.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.User)
                .WithMany(u => u.Transactions)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Order)
                .WithMany(o => o.Transactions)
                .HasForeignKey(t => t.OrderId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ChatMessages>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                      .HasColumnType("char(36)")
                      .HasCharSet("utf8mb4")
                      .HasCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.UserId)
                      .HasColumnType("char(36)")
                      .HasCharSet("utf8mb4")
                      .HasCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.TopicId)
                      .HasColumnType("int");

                entity.Property(e => e.Message)
                      .HasColumnType("longtext")
                      .HasCharSet("utf8mb4");

                entity.Property(e => e.IsBot)
                      .HasColumnType("tinyint(1)");

                entity.Property(e => e.Timestamp)
                      .HasColumnType("datetime(6)");
            });

            modelBuilder.Entity<Level>().HasData(
                new Level
                {
                    LevelId = 1,
                    LevelName = "A0,A1",
                    MinPoint = 0,
                    MaxPoint = 100,
                },
                new Level
                {
                    LevelId = 2,
                    LevelName = "B1,B2",
                    MinPoint = 101,
                    MaxPoint = 200,
                },
                new Level
                {
                    LevelId = 3,
                    LevelName = "C1,B2",
                    MinPoint = 201,
                    MaxPoint = 300,
                }
            );
        }
    }
}