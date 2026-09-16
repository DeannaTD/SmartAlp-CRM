using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NewGenBridge.Models;

namespace NewGenBridge.Data
{
    public class ApplicationDbContext : IdentityDbContext<NPUser, IdentityRole, string>
    {
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Division> Divisions { get; set; }
        public DbSet<NPUser> NPUsers { get; set; }
        public DbSet<PaymentPeriod> PaymentPeriods { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<UserLesson> UserLessons { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Level> Levels { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<UserAccess> UserAccesses { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<Price> Prices { get; set; }
        public DbSet<MentorLevel> MentorLevels { get; set; }
        public DbSet<HubFile> HubFiles { get; set; }
        public DbSet<HubFileRole> HubFileRoles { get; set; }
        public DbSet<Translation> Translations { get; set; }
        public DbSet<Lead> Leads { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<KitRequest> KitRequests { get; set; }
        public DbSet<Agreement> Agreements { get; set; }
        public DbSet<Royalty> Royalties { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            //Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<HubFileRole>()
                .HasKey(hfr => new { hfr.FileId, hfr.RoleName });
            builder.Entity<HubFileRole>()
                .HasOne(hfr => hfr.HubFile)
                .WithMany(hf => hf.HubFileRoles)
                .HasForeignKey(hfr => hfr.FileId);
        }
    }
}
