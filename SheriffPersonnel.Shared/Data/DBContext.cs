using Microsoft.EntityFrameworkCore;
using SheriffPersonnel.Shared.Models;

namespace SheriffPersonnel.Shared.Data
{
    public class DBContext : DbContext
    {
        public DBContext(DbContextOptions<DBContext> options) : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<UserRole> UserRoles => Set<UserRole>();
        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<Status> Statuses => Set<Status>();
        public DbSet<EmployeeChange> EmployeeChanges => Set<EmployeeChange>();
        public DbSet<Program> Programs => Set<Program>();
        public DbSet<Rank> Ranks => Set<Rank>();
        public DbSet<Unit> Units => Set<Unit>();
        public DbSet<EmployeeInventoryAssignment> EmployeeInventoryAssignments => Set<EmployeeInventoryAssignment>();
        public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Location> Locations => Set<Location>();
        public DbSet<Supplier> Suppliers => Set<Supplier>();
        public DbSet<Firearm> Firearms => Set<Firearm>();
        public DbSet<Make> Makes => Set<Make>();
        public DbSet<RangeStatistic> RangeStatistics => Set<RangeStatistic>();
        public DbSet<RangeCourse> RangeCourses => Set<RangeCourse>();
        public DbSet<RangeEvent> RangeEvents => Set<RangeEvent>();
        public DbSet<RangeOfficer> RangeOfficers => Set<RangeOfficer>();
        public DbSet<RangeSkill> RangeSkills => Set<RangeSkill>();
        public DbSet<EmployeeTraining> EmployeeTrainings => Set<EmployeeTraining>();
        public DbSet<TrainingSession> TrainingSessions => Set<TrainingSession>();
        public DbSet<TrainingSchool> TrainingSchools => Set<TrainingSchool>();
        public DbSet<Document> Documents => Set<Document>();
        public DbSet<DocumentType> DocumentTypes => Set<DocumentType>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.UserPrincipalName).IsUnique();
                entity.HasIndex(u => u.EntraObjectID).IsUnique();
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasIndex(r => r.RoleName).IsUnique();
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(ur => new { ur.UserID, ur.RoleID });

                entity.HasOne(ur => ur.User)
                      .WithMany(u => u.UserRoles)
                      .HasForeignKey(ur => ur.UserID)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ur => ur.Role)
                      .WithMany(r => r.UserRoles)
                      .HasForeignKey(ur => ur.RoleID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Employee entity configuration and foreign keys
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.ToTable("Employees", tb => tb.HasTrigger("trg_Employees"));

                entity.HasKey(e => e.EmployeeID);

                // Map C# EmployeeFlag property to SQL column IsSheriffEmployee
                entity.Property(e => e.EmployeeFlag).HasColumnName("IsSheriffEmployee");
                entity.Ignore(e => e.IsSheriffEmployee);

                // Foreign key relationship for LastUser
                entity.HasOne(e => e.LastUser)
                      .WithMany()
                      .HasForeignKey(e => e.LastUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                // Foreign key relationship for Status
                entity.HasOne(e => e.Status)
                      .WithMany(s => s.Employees)
                      .HasForeignKey(e => e.StatusID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // EmployeeChange entity configuration and foreign keys
            modelBuilder.Entity<EmployeeChange>(entity =>
            {
                entity.ToTable("EmployeeChanges");
                entity.HasKey(ec => ec.HistoryID);

                entity.HasOne(ec => ec.Employee)
                      .WithMany(e => e.EmployeeChanges)
                      .HasForeignKey(ec => ec.EmployeeID)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ec => ec.Program)
                      .WithMany(p => p.EmployeeChanges)
                      .HasForeignKey(ec => ec.ProgramID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(ec => ec.Rank)
                      .WithMany(r => r.EmployeeChanges)
                      .HasForeignKey(ec => ec.RankID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(ec => ec.Unit)
                      .WithMany(u => u.EmployeeChanges)
                      .HasForeignKey(ec => ec.UnitID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(ec => ec.LastUser)
                      .WithMany()
                      .HasForeignKey(ec => ec.LastUserID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Program>(entity =>
            {
                entity.ToTable("Programs");
                entity.HasKey(p => p.ProgramID);
            });

            modelBuilder.Entity<Rank>(entity =>
            {
                entity.ToTable("Ranks");
                entity.HasKey(r => r.RankID);
            });

            modelBuilder.Entity<Unit>(entity =>
            {
                entity.ToTable("Units");
                entity.HasKey(u => u.UnitID);
            });

            modelBuilder.Entity<EmployeeInventoryAssignment>(entity =>
            {
                entity.ToTable("EmployeeInventoryAssignments");
                entity.HasKey(e => e.EmpInventoryID);

                entity.HasOne(e => e.Employee)
                      .WithMany(emp => emp.EmployeeInventoryAssignments)
                      .HasForeignKey(e => e.EmployeeID)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.InventoryItem)
                      .WithMany(inv => inv.EmployeeInventoryAssignments)
                      .HasForeignKey(e => e.InventoryID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.LastUser)
                      .WithMany()
                      .HasForeignKey(e => e.LastUserID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<InventoryItem>(entity =>
            {
                entity.ToTable("InventoryItems");
                entity.HasKey(i => i.InventoryID);

                entity.HasOne(i => i.Category)
                      .WithMany(c => c.InventoryItems)
                      .HasForeignKey(i => i.CategoryID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(i => i.Supplier)
                      .WithMany(s => s.InventoryItems)
                      .HasForeignKey(i => i.SupplierID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(i => i.Location)
                      .WithMany(l => l.InventoryItems)
                      .HasForeignKey(i => i.LocationID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(i => i.LastUser)
                      .WithMany()
                      .HasForeignKey(i => i.LastUserID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Categories");
                entity.HasKey(c => c.CategoryID);
            });

            modelBuilder.Entity<Location>(entity =>
            {
                entity.ToTable("Locations");
                entity.HasKey(l => l.LocationID);
            });

            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.ToTable("Suppliers");
                entity.HasKey(s => s.SupplierID);
            });

            modelBuilder.Entity<Firearm>(entity =>
            {
                entity.ToTable("Firearms");
                entity.HasKey(f => f.FirearmID);

                entity.HasOne(f => f.Employee)
                      .WithMany(e => e.Firearms)
                      .HasForeignKey(f => f.EmployeeID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(f => f.Make)
                      .WithMany(m => m.Firearms)
                      .HasForeignKey(f => f.MakeID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(f => f.Status)
                      .WithMany(s => s.Firearms)
                      .HasForeignKey(f => f.StatusID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(f => f.LastUser)
                      .WithMany()
                      .HasForeignKey(f => f.LastUserID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Make>(entity =>
            {
                entity.ToTable("Makes");
                entity.HasKey(m => m.MakeID);
            });

            modelBuilder.Entity<RangeStatistic>(entity =>
            {
                entity.ToTable("RangeStatistics");
                entity.HasKey(rs => rs.RangeID);

                entity.HasOne(rs => rs.Employee)
                      .WithMany(e => e.RangeStatistics)
                      .HasForeignKey(rs => rs.EmployeeID)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(rs => rs.Firearm)
                      .WithMany(f => f.RangeStatistics)
                      .HasForeignKey(rs => rs.FirearmID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(rs => rs.Course)
                      .WithMany(c => c.RangeStatistics)
                      .HasForeignKey(rs => rs.CourseID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(rs => rs.RangeEvent)
                      .WithMany(re => re.RangeStatistics)
                      .HasForeignKey(rs => rs.RangeEventID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(rs => rs.LastUser)
                      .WithMany()
                      .HasForeignKey(rs => rs.LastUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(rs => rs.RangeSkills)
                      .WithMany()
                      .UsingEntity(
                          "RangeStatisticSkills",
                          l => l.HasOne(typeof(RangeSkill)).WithMany().HasForeignKey("SkillID"),
                          r => r.HasOne(typeof(RangeStatistic)).WithMany().HasForeignKey("RangeID"),
                          j =>
                          {
                              j.ToTable("RangeStatisticSkills");
                              j.HasKey("RangeID", "SkillID");
                          });
            });

            modelBuilder.Entity<RangeCourse>(entity =>
            {
                entity.ToTable("RangeCourses");
                entity.HasKey(rc => rc.CourseID);
            });

            modelBuilder.Entity<RangeEvent>(entity =>
            {
                entity.ToTable("RangeEvents");
                entity.HasKey(re => re.RangeEventID);

                entity.HasOne(re => re.RangeOfficer)
                      .WithMany()
                      .HasForeignKey(re => re.RangeOfficerID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(re => re.LastUser)
                      .WithMany()
                      .HasForeignKey(re => re.LastUserID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<RangeOfficer>(entity =>
            {
                entity.ToTable("RangeOfficers");
                entity.HasKey(ro => ro.RangeOfficerID);
            });

            modelBuilder.Entity<RangeSkill>(entity =>
            {
                entity.ToTable("RangeSkills");
                entity.HasKey(rs => rs.RangeSkillID);
                entity.Property(rs => rs.RangeSkillID).HasColumnName("SkillID");
                entity.Property(rs => rs.SkillName).HasColumnName("RangeSkill");
            });

            modelBuilder.Entity<EmployeeTraining>(entity =>
            {
                entity.ToTable("EmployeeTrainings");
                entity.HasKey(et => et.EmployeeTrainingID);

                entity.HasOne(et => et.Employee)
                      .WithMany(e => e.EmployeeTrainings)
                      .HasForeignKey(et => et.EmployeeID)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(et => et.TrainingSession)
                      .WithMany(s => s.EmployeeTrainings)
                      .HasForeignKey(et => et.TrainingSessionID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(et => et.LastUser)
                      .WithMany()
                      .HasForeignKey(et => et.LastUserID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<TrainingSession>(entity =>
            {
                entity.ToTable("TrainingSessions");
                entity.HasKey(ts => ts.TrainingSessionID);

                entity.HasOne(ts => ts.TrainingSchool)
                      .WithMany(s => s.TrainingSessions)
                      .HasForeignKey(ts => ts.TrainingSchoolID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<TrainingSchool>(entity =>
            {
                entity.ToTable("TrainingSchools");
                entity.HasKey(ts => ts.TrainingSchoolID);
            });

            modelBuilder.Entity<DocumentType>(entity =>
            {
                entity.ToTable("DocumentTypes");
                entity.HasKey(dt => dt.DocumentTypeID);
                entity.Property(dt => dt.DocumentTypeName).HasColumnName("DocumentType");
            });

            modelBuilder.Entity<Document>(entity =>
            {
                entity.ToTable("Documents");
                entity.HasKey(d => d.DocumentID);

                entity.HasOne(d => d.Employee)
                      .WithMany(e => e.Documents)
                      .HasForeignKey(d => d.EmployeeID)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.DocumentType)
                      .WithMany(dt => dt.Documents)
                      .HasForeignKey(d => d.DocumentTypeID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.UploadedByUser)
                      .WithMany()
                      .HasForeignKey(d => d.UploadedBy)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}

