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
        public virtual DbSet<Designations> Designations { get; set; }
        public virtual DbSet<Document_Types> Document_Types { get; set; }
        public virtual DbSet<Employee_Advance> Employee_Advance { get; set; }
        public virtual DbSet<Employee_Documents> Employee_Documents { get; set; }
        public virtual DbSet<Employees> Employees { get; set; }
        public virtual DbSet<Firms> Firms { get; set; }
        public virtual DbSet<ProfessionalTaxCalculation> ProfessionalTaxCalculation { get; set; }
        public virtual DbSet<Wage_Process> Wage_Process { get; set; }
        public virtual DbSet<Wage_Process_Clients> Wage_Process_Clients { get; set; }
        public virtual DbSet<Wage_Register> Wage_Register { get; set; }
        public virtual DbSet<Wage_Register_Advances> Wage_Register_Advances { get; set; }
        public virtual DbSet<Wage_Register_Allowances> Wage_Register_Allowances { get; set; }
        public virtual DbSet<Wage_Register_Canteen> Wage_Register_Canteen { get; set; }

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

                entity.Property(e => e.ALL_Shortform)
                    .HasMaxLength(30)
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

                entity.Property(e => e.ATT_Orignal_Row1)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.ATT_Orignal_Row2)
                    .HasMaxLength(20)
                    .IsUnicode(false);

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
                    .HasConstraintName("FK_Client_Requirement_Allowances_Allowances1");

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

                entity.Property(e => e.CRI_DA).HasColumnType("decimal(9, 2)");

                entity.Property(e => e.CRI_ESIC_Area)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.CRI_ESIC_Formula).HasMaxLength(200);

                entity.Property(e => e.CRI_HRA_Fixed).HasColumnType("decimal(9, 2)");

                entity.Property(e => e.CRI_InactivatedOn).HasColumnType("datetime");

                entity.Property(e => e.CRI_OT_Formula).HasMaxLength(200);

                entity.Property(e => e.CRI_OT_Rate).HasColumnType("decimal(9, 2)");

                entity.Property(e => e.CRI_PF_Formula).HasMaxLength(200);

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

                entity.Property(e => e.CLI_WorkingHours_In_Day).HasDefaultValueSql("((8))");

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

            modelBuilder.Entity<Designations>(entity =>
            {
                entity.HasKey(e => e.DES_Id);

                entity.Property(e => e.DES_Title)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Document_Types>(entity =>
            {
                entity.HasKey(e => e.DOT_Id);

                entity.Property(e => e.DOT_Title)
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

            modelBuilder.Entity<Employee_Documents>(entity =>
            {
                entity.HasKey(e => e.EMD_Id);

                entity.Property(e => e.EMD_Name)
                    .IsRequired()
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.EMD_UploadedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.DOT_)
                    .WithMany(p => p.Employee_Documents)
                    .HasForeignKey(d => d.DOT_Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Employee_Documents_Document_Types");

                entity.HasOne(d => d.EMP_)
                    .WithMany(p => p.Employee_Documents)
                    .HasForeignKey(d => d.EMP_Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Employee_Documents_Employees");
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

                entity.Property(e => e.EMP_Account_Name)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.EMP_Account_Number)
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.Property(e => e.EMP_Address)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.EMP_Bank)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.EMP_Bank_IFSC)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.EMP_Branch)
                    .HasMaxLength(100)
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
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('Pending')");

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
                    .IsRequired()
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('Pending')");
            });

            modelBuilder.Entity<Firms>(entity =>
            {
                entity.HasKey(e => e.FRM_Id);

                entity.Property(e => e.FRM_Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ProfessionalTaxCalculation>(entity =>
            {
                entity.HasKey(e => e.PTC_Id);

                entity.Property(e => e.PTC_Amount).HasColumnType("decimal(9, 2)");

                entity.Property(e => e.PTC_From).HasColumnType("decimal(9, 2)");

                entity.Property(e => e.PTC_MF)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.PTC_To).HasColumnType("decimal(9, 2)");
            });

            modelBuilder.Entity<Wage_Process>(entity =>
            {
                entity.HasKey(e => e.WAG_Id);

                entity.Property(e => e.WAG_Month).HasColumnType("date");

                entity.Property(e => e.WAG_RegisteredOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<Wage_Process_Clients>(entity =>
            {
                entity.HasKey(e => e.WPC_Id);

                entity.Property(e => e.WPC_SavedOn).HasColumnType("datetime");

                entity.HasOne(d => d.CLI_)
                    .WithMany(p => p.Wage_Process_Clients)
                    .HasForeignKey(d => d.CLI_Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Wage_Process_Clients_Clients");

                entity.HasOne(d => d.WAG_)
                    .WithMany(p => p.Wage_Process_Clients)
                    .HasForeignKey(d => d.WAG_Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Wage_Process_Clients_Wage_Process");
            });

            modelBuilder.Entity<Wage_Register>(entity =>
            {
                entity.HasKey(e => e.WAR_Id);

                entity.Property(e => e.WAR_Advance_Amount).HasColumnType("decimal(9, 2)");

                entity.Property(e => e.WAR_Basic).HasColumnType("decimal(9, 2)");

                entity.Property(e => e.WAR_Basic_Calculated).HasColumnType("decimal(9, 2)");

                entity.Property(e => e.WAR_CanteenFacility_Calculation)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.WAR_DA).HasColumnType("decimal(9, 2)");

                entity.Property(e => e.WAR_DA_Calculated).HasColumnType("decimal(9, 2)");

                entity.Property(e => e.WAR_ESIC).HasColumnType("decimal(9, 2)");

                entity.Property(e => e.WAR_ESIC_Calculated).HasColumnType("decimal(9, 2)");

                entity.Property(e => e.WAR_ESIC_Formula).HasMaxLength(200);

                entity.Property(e => e.WAR_FinalTotal).HasColumnType("decimal(9, 2)");

                entity.Property(e => e.WAR_GrossTotal).HasColumnType("decimal(9, 2)");

                entity.Property(e => e.WAR_HRA).HasColumnType("decimal(9, 2)");

                entity.Property(e => e.WAR_HRA_Calculated).HasColumnType("decimal(9, 2)");

                entity.Property(e => e.WAR_LastModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.WAR_OverTime_Calculated).HasColumnType("decimal(9, 2)");

                entity.Property(e => e.WAR_OverTime_Formula).HasMaxLength(200);

                entity.Property(e => e.WAR_OverTime_Payment).HasDefaultValueSql("((1))");

                entity.Property(e => e.WAR_PF).HasColumnType("decimal(9, 2)");

                entity.Property(e => e.WAR_PF_Calculated).HasColumnType("decimal(9, 2)");

                entity.Property(e => e.WAR_PF_Formula).HasMaxLength(200);

                entity.Property(e => e.WAR_ProffesionalTax_Calculated)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.WAR_RevenueDeduction_Calculated)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.WAR_WorkingHrs_In_Day).HasDefaultValueSql("((8))");

                entity.HasOne(d => d.CLI_)
                    .WithMany(p => p.Wage_Register)
                    .HasForeignKey(d => d.CLI_Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Wage_Register_Clients");

                entity.HasOne(d => d.CRI_)
                    .WithMany(p => p.Wage_Register)
                    .HasForeignKey(d => d.CRI_Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Wage_Register_Client_Requirements");

                entity.HasOne(d => d.EMP_)
                    .WithMany(p => p.Wage_Register)
                    .HasForeignKey(d => d.EMP_Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Wage_Register_Employees");

                entity.HasOne(d => d.WAG_)
                    .WithMany(p => p.Wage_Register)
                    .HasForeignKey(d => d.WAG_Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Wage_Register_Wage_Process");
            });

            modelBuilder.Entity<Wage_Register_Advances>(entity =>
            {
                entity.HasKey(e => e.WAD_Id);

                entity.Property(e => e.WAD_Amount).HasColumnType("decimal(9, 2)");

                entity.Property(e => e.WAD_ClosedOn).HasColumnType("datetime");

                entity.HasOne(d => d.EMP_)
                    .WithMany(p => p.Wage_Register_Advances)
                    .HasForeignKey(d => d.EMP_Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Wage_Register_Advances_Employees");

                entity.HasOne(d => d.WAG_)
                    .WithMany(p => p.Wage_Register_Advances)
                    .HasForeignKey(d => d.WAG_Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Wage_Register_Advances_Wage_Process");
            });

            modelBuilder.Entity<Wage_Register_Allowances>(entity =>
            {
                entity.HasKey(e => e.WAA_Id);

                entity.Property(e => e.WAA_Amount).HasColumnType("decimal(9, 2)");

                entity.Property(e => e.WAA_Amount_Calculated).HasColumnType("decimal(9, 2)");

                entity.HasOne(d => d.CRA_)
                    .WithMany(p => p.Wage_Register_Allowances)
                    .HasForeignKey(d => d.CRA_Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Wage_Register_Allowances_Client_Requirement_Allowances");

                entity.HasOne(d => d.WAR_)
                    .WithMany(p => p.Wage_Register_Allowances)
                    .HasForeignKey(d => d.WAR_Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Wage_Register_Allowances_Wage_Register");
            });

            modelBuilder.Entity<Wage_Register_Canteen>(entity =>
            {
                entity.HasKey(e => e.WRC_Id);

                entity.Property(e => e.WRC_Amount).HasColumnType("decimal(9, 2)");

                entity.HasOne(d => d.CLE_)
                    .WithMany(p => p.Wage_Register_Canteen)
                    .HasForeignKey(d => d.CLE_Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Wage_Register_Canteen_Clients_Employees");

                entity.HasOne(d => d.WAG_)
                    .WithMany(p => p.Wage_Register_Canteen)
                    .HasForeignKey(d => d.WAG_Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Wage_Register_Canteen_Wage_Process");
            });
        }
    }
}
