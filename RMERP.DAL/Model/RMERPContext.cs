using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace RMERP.DAL.Model
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
        public virtual DbSet<Attendance> Attendance { get; set; }
        public virtual DbSet<Cities> Cities { get; set; }
        public virtual DbSet<ClientContacts> ClientContacts { get; set; }
        public virtual DbSet<ClientRequirements> ClientRequirements { get; set; }
        public virtual DbSet<Clients> Clients { get; set; }
        public virtual DbSet<ClientsEmployees> ClientsEmployees { get; set; }
        public virtual DbSet<Departments> Departments { get; set; }
        public virtual DbSet<Designations> Designations { get; set; }
        public virtual DbSet<Employees> Employees { get; set; }
        public virtual DbSet<Firms> Firms { get; set; }
        public virtual DbSet<WageProcess> WageProcess { get; set; }

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
                entity.HasKey(e => e.AdmId);

                entity.Property(e => e.AdmId).HasColumnName("ADM_Id");

                entity.Property(e => e.AdmEmailId)
                    .IsRequired()
                    .HasColumnName("ADM_EmailId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.AdmFirstName)
                    .IsRequired()
                    .HasColumnName("ADM_FirstName")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.AdmLastName)
                    .IsRequired()
                    .HasColumnName("ADM_LastName")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.AdmMiddleName)
                    .IsRequired()
                    .HasColumnName("ADM_MiddleName")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.AdmMobile)
                    .IsRequired()
                    .HasColumnName("ADM_Mobile")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.AdmPassword)
                    .IsRequired()
                    .HasColumnName("ADM_Password")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FrmId).HasColumnName("FRM_Id");

                entity.HasOne(d => d.Frm)
                    .WithMany(p => p.AdminUsers)
                    .HasForeignKey(d => d.FrmId)
                    .HasConstraintName("FK_AdminUsers_Firms");
            });

            modelBuilder.Entity<Attendance>(entity =>
            {
                entity.HasKey(e => e.AttId);

                entity.Property(e => e.AttId).HasColumnName("ATT_Id");

                entity.Property(e => e.AdmIdImportedBy).HasColumnName("ADM_Id_ImportedBy");

                entity.Property(e => e.AttDate)
                    .HasColumnName("ATT_Date")
                    .HasColumnType("date");

                entity.Property(e => e.AttExtraHoursWorked).HasColumnName("ATT_ExtraHoursWorked");

                entity.Property(e => e.AttImportedOn)
                    .HasColumnName("ATT_ImportedOn")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.AttIsEarnLeave).HasColumnName("ATT_IsEarnLeave");

                entity.Property(e => e.AttIsPaidHoliday).HasColumnName("ATT_IsPaidHoliday");

                entity.Property(e => e.AttIsPresent).HasColumnName("ATT_IsPresent");

                entity.Property(e => e.AttIsWeeklyOff).HasColumnName("ATT_IsWeeklyOff");

                entity.Property(e => e.AttShift)
                    .HasColumnName("ATT_Shift")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.CliId).HasColumnName("CLI_Id");

                entity.Property(e => e.CriId).HasColumnName("CRI_Id");

                entity.Property(e => e.EmpId).HasColumnName("EMP_Id");

                entity.Property(e => e.WagId).HasColumnName("WAG_Id");

                entity.HasOne(d => d.Cli)
                    .WithMany(p => p.Attendance)
                    .HasForeignKey(d => d.CliId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Attendance_Clients");

                entity.HasOne(d => d.Cri)
                    .WithMany(p => p.Attendance)
                    .HasForeignKey(d => d.CriId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Attendance_Client_Requirements");

                entity.HasOne(d => d.Emp)
                    .WithMany(p => p.Attendance)
                    .HasForeignKey(d => d.EmpId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Attendance_Employees");

                entity.HasOne(d => d.Wag)
                    .WithMany(p => p.Attendance)
                    .HasForeignKey(d => d.WagId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Attendance_Wage_Process");
            });

            modelBuilder.Entity<Cities>(entity =>
            {
                entity.HasKey(e => e.CityId);

                entity.Property(e => e.CityId).HasColumnName("CITY_Id");

                entity.Property(e => e.CityName)
                    .IsRequired()
                    .HasColumnName("CITY_Name")
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ClientContacts>(entity =>
            {
                entity.HasKey(e => e.ConId);

                entity.ToTable("Client_Contacts");

                entity.Property(e => e.ConId).HasColumnName("CON_Id");

                entity.Property(e => e.AdmIdRegisteredBy).HasColumnName("ADM_Id_RegisteredBy");

                entity.Property(e => e.CliId).HasColumnName("CLI_Id");

                entity.Property(e => e.ConDesignation)
                    .HasColumnName("CON_Designation")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ConEmail)
                    .IsRequired()
                    .HasColumnName("CON_Email")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.ConFirstName)
                    .IsRequired()
                    .HasColumnName("CON_FirstName")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ConIsPrimary).HasColumnName("CON_isPrimary");

                entity.Property(e => e.ConMobile)
                    .IsRequired()
                    .HasColumnName("CON_Mobile")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ConRegisteredOn)
                    .HasColumnName("CON_RegisteredOn")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ConSurName)
                    .HasColumnName("CON_SurName")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.Cli)
                    .WithMany(p => p.ClientContacts)
                    .HasForeignKey(d => d.CliId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Client_Contacts_Clients");
            });

            modelBuilder.Entity<ClientRequirements>(entity =>
            {
                entity.HasKey(e => e.CriId);

                entity.ToTable("Client_Requirements");

                entity.Property(e => e.CriId).HasColumnName("CRI_Id");

                entity.Property(e => e.AdmIdInactivatedBy).HasColumnName("ADM_Id_InactivatedBy");

                entity.Property(e => e.CliId).HasColumnName("CLI_Id");

                entity.Property(e => e.CriActive)
                    .IsRequired()
                    .HasColumnName("CRI_Active")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.CriAllowanceAttention)
                    .HasColumnName("CRI_Allowance_Attention")
                    .HasColumnType("decimal(9, 2)");

                entity.Property(e => e.CriAllowanceConveyance)
                    .HasColumnName("CRI_Allowance_Conveyance")
                    .HasColumnType("decimal(9, 2)");

                entity.Property(e => e.CriAllowanceGrade)
                    .HasColumnName("CRI_Allowance_Grade")
                    .HasColumnType("decimal(9, 2)");

                entity.Property(e => e.CriAllowanceUpKeep)
                    .HasColumnName("CRI_Allowance_UpKeep")
                    .HasColumnType("decimal(9, 2)");

                entity.Property(e => e.CriBasic)
                    .HasColumnName("CRI_Basic")
                    .HasColumnType("decimal(9, 2)");

                entity.Property(e => e.CriBasicDa)
                    .HasColumnName("CRI_BasicDA")
                    .HasColumnType("decimal(9, 2)");

                entity.Property(e => e.CriDa).HasColumnName("CRI_DA");

                entity.Property(e => e.CriEsicArea)
                    .HasColumnName("CRI_ESIC_Area")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.CriEsicPercentage).HasColumnName("CRI_ESIC_Percentage");

                entity.Property(e => e.CriHraFixed)
                    .HasColumnName("CRI_HRA_Fixed")
                    .HasColumnType("decimal(9, 2)");

                entity.Property(e => e.CriHraPercentage).HasColumnName("CRI_HRA_Percentage");

                entity.Property(e => e.CriInactivatedOn)
                    .HasColumnName("CRI_InactivatedOn")
                    .HasColumnType("datetime");

                entity.Property(e => e.CriOtMultipleTimes).HasColumnName("CRI_OT_MultipleTimes");

                entity.Property(e => e.CriOtRate)
                    .HasColumnName("CRI_OT_Rate")
                    .HasColumnType("decimal(9, 2)");

                entity.Property(e => e.CriPfPercentage).HasColumnName("CRI_PF_Percentage");

                entity.Property(e => e.CriRegisteredOn)
                    .HasColumnName("CRI_RegisteredOn")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CriWageCalculationOnWeeklyOffPlus).HasColumnName("CRI_WageCalculationOnWeeklyOffPlus");

                entity.Property(e => e.DesId).HasColumnName("DES_Id");

                entity.HasOne(d => d.Cli)
                    .WithMany(p => p.ClientRequirements)
                    .HasForeignKey(d => d.CliId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Client_Requirements_Clients");

                entity.HasOne(d => d.Des)
                    .WithMany(p => p.ClientRequirements)
                    .HasForeignKey(d => d.DesId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Client_Requirements_Designations");
            });

            modelBuilder.Entity<Clients>(entity =>
            {
                entity.HasKey(e => e.CliId);

                entity.Property(e => e.CliId).HasColumnName("CLI_Id");

                entity.Property(e => e.AdmIdInactivatedBy).HasColumnName("ADM_Id_InactivatedBy");

                entity.Property(e => e.AdmIdRegisterBy).HasColumnName("ADM_Id_RegisterBy");

                entity.Property(e => e.CityId).HasColumnName("CITY_Id");

                entity.Property(e => e.CliAddress)
                    .HasColumnName("CLI_Address")
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.CliAttMonthEnd).HasColumnName("CLI_Att_Month_End");

                entity.Property(e => e.CliAttMonthReal)
                    .IsRequired()
                    .HasColumnName("CLI_Att_MonthReal")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.CliAttMonthStart).HasColumnName("CLI_Att_Month_Start");

                entity.Property(e => e.CliEmail)
                    .HasColumnName("CLI_Email")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.CliEmail2)
                    .HasColumnName("CLI_Email_2")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.CliFax)
                    .HasColumnName("CLI_Fax")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.CliGstNumber)
                    .HasColumnName("CLI_GST_Number")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.CliGstRate).HasColumnName("CLI_GST_Rate");

                entity.Property(e => e.CliHsnCode)
                    .HasColumnName("CLI_HSN_Code")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.CliInActivatedOn)
                    .HasColumnName("CLI_InActivatedOn")
                    .HasColumnType("datetime");

                entity.Property(e => e.CliInternationalDomestic).HasColumnName("CLI_International_Domestic");

                entity.Property(e => e.CliIsActive)
                    .IsRequired()
                    .HasColumnName("CLI_IsActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.CliLogo)
                    .HasColumnName("CLI_Logo")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.CliName)
                    .IsRequired()
                    .HasColumnName("CLI_Name")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.CliPhone)
                    .HasColumnName("CLI_Phone")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.CliPincode)
                    .IsRequired()
                    .HasColumnName("CLI_Pincode")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.CliRegisteredOn)
                    .HasColumnName("CLI_RegisteredOn")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CliTdsRate).HasColumnName("CLI_TDS_Rate");

                entity.Property(e => e.FrmId).HasColumnName("FRM_Id");

                entity.HasOne(d => d.City)
                    .WithMany(p => p.Clients)
                    .HasForeignKey(d => d.CityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Clients_Cities");

                entity.HasOne(d => d.Frm)
                    .WithMany(p => p.Clients)
                    .HasForeignKey(d => d.FrmId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Clients_Firms");
            });

            modelBuilder.Entity<ClientsEmployees>(entity =>
            {
                entity.HasKey(e => e.CleId);

                entity.ToTable("Clients_Employees");

                entity.Property(e => e.CleId).HasColumnName("CLE_Id");

                entity.Property(e => e.AdmIdRegisteredBy).HasColumnName("ADM_Id_RegisteredBy");

                entity.Property(e => e.CleRegisteredOn)
                    .HasColumnName("CLE_RegisteredOn")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CliId).HasColumnName("CLI_Id");

                entity.Property(e => e.DesId).HasColumnName("DES_Id");

                entity.Property(e => e.EmpId).HasColumnName("EMP_Id");

                entity.HasOne(d => d.Cli)
                    .WithMany(p => p.ClientsEmployees)
                    .HasForeignKey(d => d.CliId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Clients_Employees_Clients");

                entity.HasOne(d => d.Des)
                    .WithMany(p => p.ClientsEmployees)
                    .HasForeignKey(d => d.DesId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Clients_Employees_Designations");

                entity.HasOne(d => d.Emp)
                    .WithMany(p => p.ClientsEmployees)
                    .HasForeignKey(d => d.EmpId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Clients_Employees_Employees");
            });

            modelBuilder.Entity<Departments>(entity =>
            {
                entity.HasKey(e => e.DeptId);

                entity.Property(e => e.DeptId).HasColumnName("DEPT_Id");

                entity.Property(e => e.DeptTitle)
                    .IsRequired()
                    .HasColumnName("DEPT_Title")
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Designations>(entity =>
            {
                entity.HasKey(e => e.DesId);

                entity.Property(e => e.DesId).HasColumnName("DES_Id");

                entity.Property(e => e.DesTitle)
                    .IsRequired()
                    .HasColumnName("DES_Title")
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Employees>(entity =>
            {
                entity.HasKey(e => e.EmpId);

                entity.Property(e => e.EmpId).HasColumnName("EMP_Id");

                entity.Property(e => e.AdmIdInactivatedBy).HasColumnName("ADM_Id_InactivatedBy");

                entity.Property(e => e.AdmIdRegisteredBy).HasColumnName("ADM_Id_RegisteredBy");

                entity.Property(e => e.DeptId).HasColumnName("DEPT_Id");

                entity.Property(e => e.EmpAadharName)
                    .HasColumnName("EMP_Aadhar_Name")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.EmpAadharNumber)
                    .HasColumnName("EMP_Aadhar_Number")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.EmpAddress)
                    .HasColumnName("EMP_Address")
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.EmpContactPrimary)
                    .HasColumnName("EMP_Contact_Primary")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.EmpContactSecondry)
                    .HasColumnName("EMP_Contact_Secondry")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.EmpDateOfJoining)
                    .HasColumnName("EMP_DateOfJoining")
                    .HasColumnType("date");

                entity.Property(e => e.EmpDesignation)
                    .HasColumnName("EMP_Designation")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.EmpDob)
                    .HasColumnName("EMP_DOB")
                    .HasColumnType("date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.EmpEmployeeNumberOffice).HasColumnName("EMP_EmployeeNumber_Office");

                entity.Property(e => e.EmpEsicNumber)
                    .HasColumnName("EMP_ESIC_Number")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.EmpFirstName)
                    .IsRequired()
                    .HasColumnName("EMP_FirstName")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.EmpGender).HasColumnName("EMP_Gender");

                entity.Property(e => e.EmpInactivatedOn)
                    .HasColumnName("EMP_InactivatedOn")
                    .HasColumnType("datetime");

                entity.Property(e => e.EmpIsActive)
                    .IsRequired()
                    .HasColumnName("EMP_IsActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.EmpMarried).HasColumnName("EMP_Married");

                entity.Property(e => e.EmpMiddleName)
                    .HasColumnName("EMP_MiddleName")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.EmpPanNumber)
                    .HasColumnName("EMP_Pan_Number")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.EmpRegisteredOn)
                    .HasColumnName("EMP_RegisteredOn")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.EmpSurName)
                    .IsRequired()
                    .HasColumnName("EMP_SurName")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.EmpTpcEmployeeId)
                    .HasColumnName("EMP_TPC_EmployeeId")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.EmpUanNumber)
                    .HasColumnName("EMP_UAN_Number")
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.HasOne(d => d.Dept)
                    .WithMany(p => p.Employees)
                    .HasForeignKey(d => d.DeptId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Employees_Departments");
            });

            modelBuilder.Entity<Firms>(entity =>
            {
                entity.HasKey(e => e.FrmId);

                entity.Property(e => e.FrmId).HasColumnName("FRM_Id");

                entity.Property(e => e.FrmName)
                    .IsRequired()
                    .HasColumnName("FRM_Name")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<WageProcess>(entity =>
            {
                entity.HasKey(e => e.WagId);

                entity.ToTable("Wage_Process");

                entity.Property(e => e.WagId).HasColumnName("WAG_Id");

                entity.Property(e => e.AdmIdRegisteredBy).HasColumnName("ADM_Id_RegisteredBy");

                entity.Property(e => e.WagMonth)
                    .HasColumnName("WAG_Month")
                    .HasColumnType("date");

                entity.Property(e => e.WagRegisteredOn)
                    .HasColumnName("WAG_RegisteredOn")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });
        }
    }
}
