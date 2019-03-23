using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace RMERP.DAL.Models
{
    public partial class RMERPContext : DbContext
    {
        public RMERPContext()
        {
        }

        public RMERPContext(DbContextOptions<RMERPContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AdminUsers> AdminUsers { get; set; }
        public virtual DbSet<Allowances> Allowances { get; set; }
        public virtual DbSet<Attendance> Attendance { get; set; }
        public virtual DbSet<Cities> Cities { get; set; }
        public virtual DbSet<Client_Contacts> Client_Contacts { get; set; }
        public virtual DbSet<Client_Requirement_Allowances> Client_Requirement_Allowances { get; set; }
        public virtual DbSet<Client_Requirements> Client_Requirements { get; set; }
        public virtual DbSet<Clients> Clients { get; set; }
        public virtual DbSet<Clients_Employees> Clients_Employees { get; set; }
        public virtual DbSet<Departments> Departments { get; set; }
        public virtual DbSet<Designations> Designations { get; set; }
        public virtual DbSet<Employee_Advance> Employee_Advance { get; set; }
        public virtual DbSet<Employees> Employees { get; set; }
        public virtual DbSet<Firms> Firms { get; set; }
        public virtual DbSet<Wage_Process> Wage_Process { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=PC_41;Database=RMERP;User Id=sa;password=Perfect;Trusted_Connection=False;MultipleActiveResultSets=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AdminUsers>(entity =>
            {
                entity.HasKey(e => e.ADM_Id);

                entity.Property(e => e.ADM_EmailId)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ADM_FirstName)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.ADM_LastName)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.ADM_MiddleName)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.ADM_Mobile)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ADM_Password)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.FRM_)
                    .WithMany(p => p.AdminUsers)
                    .HasForeignKey(d => d.FRM_Id)
                    .HasConstraintName("FK_AdminUsers_Firms");
            });

            modelBuilder.Entity<Allowances>(entity =>
            {
                entity.HasKey(e => e.ALL_Id);

                entity.Property(e => e.ALL_Alias)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ALL_Title)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Attendance>(entity =>
            {
                entity.HasKey(e => e.ATT_Id);

                entity.Property(e => e.ATT_Date).HasColumnType("date");

                entity.Property(e => e.ATT_ImportedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ATT_Shift)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.HasOne(d => d.CLI_)
                    .WithMany(p => p.Attendance)
                    .HasForeignKey(d => d.CLI_Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Attendance_Clients");

                entity.HasOne(d => d.DES_)
                    .WithMany(p => p.Attendance)
                    .HasForeignKey(d => d.DES_Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Attendance_Designations");

                entity.HasOne(d => d.EMP_)
                    .WithMany(p => p.Attendance)
                    .HasForeignKey(d => d.EMP_Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Attendance_Employees");

                entity.HasOne(d => d.WAG_)
                    .WithMany(p => p.Attendance)
                    .HasForeignKey(d => d.WAG_Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Attendance_Wage_Process");
            });

            modelBuilder.Entity<Cities>(entity =>
            {
                entity.HasKey(e => e.CITY_Id);

                entity.Property(e => e.CITY_Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Client_Contacts>(entity =>
            {
                entity.HasKey(e => e.CON_Id);

                entity.Property(e => e.CON_Designation)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.CON_Email)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.CON_FirstName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.CON_Mobile)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CON_RegisteredOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CON_SurName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.CLI_)
                    .WithMany(p => p.Client_Contacts)
                    .HasForeignKey(d => d.CLI_Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Client_Contacts_Clients");
            });

            modelBuilder.Entity<Client_Requirement_Allowances>(entity =>
            {
                entity.HasKey(e => e.CRA_Id);

                entity.Property(e => e.CRA_Amount).HasColumnType("decimal(9, 2)");

                entity.HasOne(d => d.ALL_)
                    .WithMany(p => p.Client_Requirement_Allowances)
                    .HasForeignKey(d => d.ALL_Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Client_Requirement_Allowances_Allowances");

                entity.HasOne(d => d.CRI_)
                    .WithMany(p => p.Client_Requirement_Allowances)
                    .HasForeignKey(d => d.CRI_Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Client_Requirement_Allowances_Client_Requirements");
            });

            modelBuilder.Entity<Client_Requirements>(entity =>
            {
                entity.HasKey(e => e.CRI_Id);

                entity.Property(e => e.CRI_Active)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.CRI_Basic).HasColumnType("decimal(9, 2)");

                entity.Property(e => e.CRI_ESIC_Area)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.CRI_HRA_Fixed).HasColumnType("decimal(9, 2)");

                entity.Property(e => e.CRI_InactivatedOn).HasColumnType("datetime");

                entity.Property(e => e.CRI_OT_Rate).HasColumnType("decimal(9, 2)");

                entity.Property(e => e.CRI_RegisteredOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CRI_Total).HasDefaultValueSql("((1))");

                entity.HasOne(d => d.CLI_)
                    .WithMany(p => p.Client_Requirements)
                    .HasForeignKey(d => d.CLI_Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Client_Requirements_Clients");

                entity.HasOne(d => d.DES_)
                    .WithMany(p => p.Client_Requirements)
                    .HasForeignKey(d => d.DES_Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Client_Requirements_Designations");
            });

            modelBuilder.Entity<Clients>(entity =>
            {
                entity.HasKey(e => e.CLI_Id);

                entity.Property(e => e.CLI_Address)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.CLI_Att_MonthReal)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.CLI_Email)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.CLI_Email_2)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.CLI_Fax)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.CLI_GST_Number)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.CLI_HSN_Code)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.CLI_InActivatedOn).HasColumnType("datetime");

                entity.Property(e => e.CLI_IsActive)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.CLI_Logo)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.CLI_Name)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.CLI_Phone)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.CLI_Pincode)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.CLI_RegisteredOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.CITY_)
                    .WithMany(p => p.Clients)
                    .HasForeignKey(d => d.CITY_Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Clients_Cities");

                entity.HasOne(d => d.FRM_)
                    .WithMany(p => p.Clients)
                    .HasForeignKey(d => d.FRM_Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Clients_Firms");
            });

            modelBuilder.Entity<Clients_Employees>(entity =>
            {
                entity.HasKey(e => e.CLE_Id);

                entity.HasIndex(e => e.CLE_Id)
                    .HasName("IX_Clients_Employees");

                entity.Property(e => e.CLE_RegisteredOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.CLI_)
                    .WithMany(p => p.Clients_Employees)
                    .HasForeignKey(d => d.CLI_Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Clients_Employees_Clients");

                entity.HasOne(d => d.DES_)
                    .WithMany(p => p.Clients_Employees)
                    .HasForeignKey(d => d.DES_Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Clients_Employees_Designations");

                entity.HasOne(d => d.EMP_)
                    .WithMany(p => p.Clients_Employees)
                    .HasForeignKey(d => d.EMP_Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Clients_Employees_Employees");
            });

            modelBuilder.Entity<Departments>(entity =>
            {
                entity.HasKey(e => e.DEPT_Id);

                entity.Property(e => e.DEPT_Title)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Designations>(entity =>
            {
                entity.HasKey(e => e.DES_Id);

                entity.Property(e => e.DES_Title)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Employee_Advance>(entity =>
            {
                entity.HasKey(e => e.ADV_Id);

                entity.Property(e => e.ADV_Amount).HasColumnType("decimal(9, 2)");

                entity.Property(e => e.ADV_RegisteredOn).HasColumnType("datetime");

                entity.HasOne(d => d.EMP_)
                    .WithMany(p => p.Employee_Advance)
                    .HasForeignKey(d => d.EMP_Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Employee_Advance_Employees");
            });

            modelBuilder.Entity<Employees>(entity =>
            {
                entity.HasKey(e => e.EMP_Id);

                entity.Property(e => e.EMP_Aadhar_Name)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.EMP_Aadhar_Number)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.EMP_Address)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.EMP_Contact_Primary)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.EMP_Contact_Secondry)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.EMP_DOB)
                    .HasColumnType("date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.EMP_DateOfJoining).HasColumnType("date");

                entity.Property(e => e.EMP_Designation)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.EMP_ESIC_Number)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.EMP_FirstName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.EMP_InactivatedOn).HasColumnType("datetime");

                entity.Property(e => e.EMP_IsActive)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.EMP_MiddleName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.EMP_Pan_Number)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.EMP_RegisteredOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.EMP_SurName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.EMP_TPC_EmployeeId)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.EMP_UAN_Number)
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.HasOne(d => d.DEPT_)
                    .WithMany(p => p.Employees)
                    .HasForeignKey(d => d.DEPT_Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Employees_Departments");
            });

            modelBuilder.Entity<Firms>(entity =>
            {
                entity.HasKey(e => e.FRM_Id);

                entity.Property(e => e.FRM_Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Wage_Process>(entity =>
            {
                entity.HasKey(e => e.WAG_Id);

                entity.Property(e => e.WAG_Month).HasColumnType("date");

                entity.Property(e => e.WAG_RegisteredOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });
        }
    }
}
