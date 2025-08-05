using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace RMERP.DAL.Models;

public partial class RMERPContext : DbContext
{
    public RMERPContext()
    {
    }

    public RMERPContext(DbContextOptions<RMERPContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AdminUser> AdminUsers { get; set; }

    public virtual DbSet<Allowance> Allowances { get; set; }

    public virtual DbSet<Attendance> Attendances { get; set; }

    public virtual DbSet<Attendance_Parameter> Attendance_Parameters { get; set; }

    public virtual DbSet<Attendance_Summary> Attendance_Summaries { get; set; }

    public virtual DbSet<Cities_all> Cities_alls { get; set; }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Client_ActivationHistory> Client_ActivationHistories { get; set; }

    public virtual DbSet<Client_Contact> Client_Contacts { get; set; }

    public virtual DbSet<Client_Requirement> Client_Requirements { get; set; }

    public virtual DbSet<Client_Requirement_Allowance> Client_Requirement_Allowances { get; set; }

    public virtual DbSet<Clients_Employee> Clients_Employees { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<Designation> Designations { get; set; }

    public virtual DbSet<Document_Type> Document_Types { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Employee_Advance> Employee_Advances { get; set; }

    public virtual DbSet<Employee_Document> Employee_Documents { get; set; }

    public virtual DbSet<Firm> Firms { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<Invoice_Concept> Invoice_Concepts { get; set; }

    public virtual DbSet<ProfessionalTaxCalculation> ProfessionalTaxCalculations { get; set; }

    public virtual DbSet<State> States { get; set; }

    public virtual DbSet<Wage_PaySlip> Wage_PaySlips { get; set; }

    public virtual DbSet<Wage_Process> Wage_Processes { get; set; }

    public virtual DbSet<Wage_Process_Client> Wage_Process_Clients { get; set; }

    public virtual DbSet<Wage_Register> Wage_Registers { get; set; }

    public virtual DbSet<Wage_Register_Advance> Wage_Register_Advances { get; set; }

    public virtual DbSet<Wage_Register_Allowance> Wage_Register_Allowances { get; set; }

    public virtual DbSet<Wage_Register_Allowances_1> Wage_Register_Allowances_1s { get; set; }

    public virtual DbSet<Wage_Register_Allowances_10> Wage_Register_Allowances_10s { get; set; }

    public virtual DbSet<Wage_Register_Allowances_2> Wage_Register_Allowances_2s { get; set; }

    public virtual DbSet<Wage_Register_Allowances_3> Wage_Register_Allowances_3s { get; set; }

    public virtual DbSet<Wage_Register_Allowances_4> Wage_Register_Allowances_4s { get; set; }

    public virtual DbSet<Wage_Register_Allowances_5> Wage_Register_Allowances_5s { get; set; }

    public virtual DbSet<Wage_Register_Allowances_6> Wage_Register_Allowances_6s { get; set; }

    public virtual DbSet<Wage_Register_Allowances_7> Wage_Register_Allowances_7s { get; set; }

    public virtual DbSet<Wage_Register_Allowances_8> Wage_Register_Allowances_8s { get; set; }

    public virtual DbSet<Wage_Register_Allowances_9> Wage_Register_Allowances_9s { get; set; }

    public virtual DbSet<Wage_Register_Canteen> Wage_Register_Canteens { get; set; }

    public virtual DbSet<Wage_Register_Outstation> Wage_Register_Outstations { get; set; }

    public virtual DbSet<Wage_Register_Performance> Wage_Register_Performances { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=S148-72-214-32\\SQLEXPRESS;Database=RMERP;User Id=sa;password=Perfect;Trusted_Connection=True;TrustServerCertificate=True;Integrated Security=False;MultipleActiveResultSets=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AdminUser>(entity =>
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

            entity.HasOne(d => d.FRM).WithMany(p => p.AdminUsers)
                .HasForeignKey(d => d.FRM_Id)
                .HasConstraintName("FK_AdminUsers_Firms");
        });

        modelBuilder.Entity<Allowance>(entity =>
        {
            entity.HasKey(e => e.ALL_Id).HasName("PK_Allowances_1");

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

            entity.ToTable("Attendance");

            entity.Property(e => e.ATT_ImportedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ATT_Orignal_Row1)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ATT_Orignal_Row2)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ATT_Shift)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.HasOne(d => d.CLI).WithMany(p => p.Attendances)
                .HasForeignKey(d => d.CLI_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Attendance_Clients");

            entity.HasOne(d => d.DES).WithMany(p => p.Attendances)
                .HasForeignKey(d => d.DES_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Attendance_Designations");

            entity.HasOne(d => d.EMP).WithMany(p => p.Attendances)
                .HasForeignKey(d => d.EMP_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Attendance_Employees");

            entity.HasOne(d => d.WAG).WithMany(p => p.Attendances)
                .HasForeignKey(d => d.WAG_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Attendance_Wage_Process");
        });

        modelBuilder.Entity<Attendance_Parameter>(entity =>
        {
            entity.HasKey(e => e.ATP_Id);

            entity.ToTable("Attendance_Parameter");

            entity.Property(e => e.ATP_Att_MonthReal).HasDefaultValue(true);
            entity.Property(e => e.ATP_RegisteredOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<Attendance_Summary>(entity =>
        {
            entity.HasKey(e => e.ATS_Id);

            entity.ToTable("Attendance_Summary");

            entity.Property(e => e.ATS_ImportedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ATS_TemplateReference)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.CLI).WithMany(p => p.Attendance_Summaries)
                .HasForeignKey(d => d.CLI_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Attendance_Summary_Clients");

            entity.HasOne(d => d.DES).WithMany(p => p.Attendance_Summaries)
                .HasForeignKey(d => d.DES_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Attendance_Summary_Designations");

            entity.HasOne(d => d.EMP).WithMany(p => p.Attendance_Summaries)
                .HasForeignKey(d => d.EMP_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Attendance_Summary_Employees");

            entity.HasOne(d => d.WAG).WithMany(p => p.Attendance_Summaries)
                .HasForeignKey(d => d.WAG_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Attendance_Summary_Wage_Process");
        });

        modelBuilder.Entity<Cities_all>(entity =>
        {
            entity.HasKey(e => e.CITY_Id);

            entity.ToTable("Cities_all");

            entity.Property(e => e.CITY_Name)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.CIT_Id);

            entity.Property(e => e.CIT_Name)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.CLI_Id);

            entity.Property(e => e.CLI_Address)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.CLI_Email)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.CLI_Email_2)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.CLI_Fax)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CLI_GST_Info).IsUnicode(false);
            entity.Property(e => e.CLI_GST_Number)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CLI_HSN_Code)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.CLI_InActivatedOn).HasColumnType("datetime");
            entity.Property(e => e.CLI_International_Domestic).HasComment("1: International; 2: Domestic");
            entity.Property(e => e.CLI_Invoicing_Address1)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.CLI_Invoicing_Address2)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.CLI_Invoicing_City)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CLI_Invoicing_Location)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.CLI_Invoicing_Name)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.CLI_Invoicing_ZipCode)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.CLI_IsActive).HasDefaultValue(true);
            entity.Property(e => e.CLI_Logo)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.CLI_MLWF_Contribution).HasColumnType("decimal(9, 2)");
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
            entity.Property(e => e.CLI_Place_Of_Supply)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.CLI_RegisteredOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CLI_Total_WorkingDays).HasComment("0:Consider_RealDays,1:Excluding_WeeklyOff,2:Reduce_StaticDays");
            entity.Property(e => e.CLI_WorkingHours_In_Day).HasDefaultValue(8);

            entity.HasOne(d => d.CITY).WithMany(p => p.Clients)
                .HasForeignKey(d => d.CITY_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Clients_Cities");

            entity.HasOne(d => d.FRM).WithMany(p => p.Clients)
                .HasForeignKey(d => d.FRM_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Clients_Firms");

            entity.HasOne(d => d.STA).WithMany(p => p.Clients)
                .HasForeignKey(d => d.STA_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Clients_States");
        });

        modelBuilder.Entity<Client_ActivationHistory>(entity =>
        {
            entity.HasKey(e => e.CAH_Id);

            entity.ToTable("Client_ActivationHistory");

            entity.Property(e => e.CAH_ActiveOn).HasColumnType("datetime");
            entity.Property(e => e.CAH_InactiveOn).HasColumnType("datetime");

            entity.HasOne(d => d.CLI).WithMany(p => p.Client_ActivationHistories)
                .HasForeignKey(d => d.CLI_Id)
                .HasConstraintName("FK_Client_ActivationHistory_Clients");
        });

        modelBuilder.Entity<Client_Contact>(entity =>
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
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CON_SurName)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.CLI).WithMany(p => p.Client_Contacts)
                .HasForeignKey(d => d.CLI_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Client_Contacts_Clients");
        });

        modelBuilder.Entity<Client_Requirement>(entity =>
        {
            entity.HasKey(e => e.CRI_Id);

            entity.Property(e => e.CRI_Active).HasDefaultValue(true);
            entity.Property(e => e.CRI_Allowance_Name_1)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CRI_Allowance_Name_10)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CRI_Allowance_Name_2)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CRI_Allowance_Name_3)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CRI_Allowance_Name_4)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CRI_Allowance_Name_5)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CRI_Allowance_Name_6)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CRI_Allowance_Name_7)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CRI_Allowance_Name_8)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CRI_Allowance_Name_9)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CRI_Attendance_Allowance_Rate).HasColumnType("decimal(9, 2)");
            entity.Property(e => e.CRI_Basic).HasColumnType("decimal(9, 2)");
            entity.Property(e => e.CRI_Billing_Amount).HasColumnType("decimal(9, 2)");
            entity.Property(e => e.CRI_Billing_ServiceCharge_Formula).HasMaxLength(200);
            entity.Property(e => e.CRI_DA).HasColumnType("decimal(9, 2)");
            entity.Property(e => e.CRI_ESIC_Area)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.CRI_ESIC_Formula).HasMaxLength(200);
            entity.Property(e => e.CRI_HRA_Fixed).HasColumnType("decimal(9, 2)");
            entity.Property(e => e.CRI_InactivatedOn).HasColumnType("datetime");
            entity.Property(e => e.CRI_MLWF_Employee_Base).HasColumnType("decimal(9, 2)");
            entity.Property(e => e.CRI_MLWF_Employee_GThen).HasColumnType("decimal(9, 2)");
            entity.Property(e => e.CRI_MLWF_Employee_LThen).HasColumnType("decimal(9, 2)");
            entity.Property(e => e.CRI_MLWF_Employer_Base).HasColumnType("decimal(9, 2)");
            entity.Property(e => e.CRI_MLWF_Employer_GThen).HasColumnType("decimal(9, 2)");
            entity.Property(e => e.CRI_MLWF_Employer_LThen).HasColumnType("decimal(9, 2)");
            entity.Property(e => e.CRI_Nightshift_Allowance_Rate).HasColumnType("decimal(9, 2)");
            entity.Property(e => e.CRI_OT_Fixed_PerHour).HasColumnType("decimal(9, 2)");
            entity.Property(e => e.CRI_OT_Formula).HasMaxLength(200);
            entity.Property(e => e.CRI_OT_Rate).HasColumnType("decimal(9, 2)");
            entity.Property(e => e.CRI_OutStation_Allowance_Rate).HasColumnType("decimal(9, 2)");
            entity.Property(e => e.CRI_PF_ApplyMAX).HasColumnType("decimal(9, 2)");
            entity.Property(e => e.CRI_PF_Formula).HasMaxLength(200);
            entity.Property(e => e.CRI_ProffTax_F_Amount_1).HasColumnType("decimal(9, 2)");
            entity.Property(e => e.CRI_ProffTax_F_Amount_2).HasColumnType("decimal(9, 2)");
            entity.Property(e => e.CRI_ProffTax_F_Amount_3)
                .HasDefaultValue(200m)
                .HasColumnType("decimal(9, 2)");
            entity.Property(e => e.CRI_ProffTax_F_From_1).HasColumnType("decimal(9, 2)");
            entity.Property(e => e.CRI_ProffTax_F_From_2)
                .HasDefaultValue(7501m)
                .HasColumnType("decimal(9, 2)");
            entity.Property(e => e.CRI_ProffTax_F_From_3)
                .HasDefaultValue(10001m)
                .HasColumnType("decimal(9, 2)");
            entity.Property(e => e.CRI_ProffTax_F_To_1)
                .HasDefaultValue(7500m)
                .HasColumnType("decimal(9, 2)");
            entity.Property(e => e.CRI_ProffTax_F_To_2)
                .HasDefaultValue(10000m)
                .HasColumnType("decimal(9, 2)");
            entity.Property(e => e.CRI_ProffTax_F_To_3)
                .HasDefaultValue(1000000m)
                .HasColumnType("decimal(9, 2)");
            entity.Property(e => e.CRI_ProffTax_M_Amount_1).HasColumnType("decimal(9, 2)");
            entity.Property(e => e.CRI_ProffTax_M_Amount_2)
                .HasDefaultValue(175m)
                .HasColumnType("decimal(9, 2)");
            entity.Property(e => e.CRI_ProffTax_M_Amount_3)
                .HasDefaultValue(200m)
                .HasColumnType("decimal(9, 2)");
            entity.Property(e => e.CRI_ProffTax_M_From_1).HasColumnType("decimal(9, 2)");
            entity.Property(e => e.CRI_ProffTax_M_From_2)
                .HasDefaultValue(7501m)
                .HasColumnType("decimal(9, 2)");
            entity.Property(e => e.CRI_ProffTax_M_From_3)
                .HasDefaultValue(10001m)
                .HasColumnType("decimal(9, 2)");
            entity.Property(e => e.CRI_ProffTax_M_To_1)
                .HasDefaultValue(7500m)
                .HasColumnType("decimal(9, 2)");
            entity.Property(e => e.CRI_ProffTax_M_To_2)
                .HasDefaultValue(10000m)
                .HasColumnType("decimal(9, 2)");
            entity.Property(e => e.CRI_ProffTax_M_To_3)
                .HasDefaultValue(1000000m)
                .HasColumnType("decimal(9, 2)");
            entity.Property(e => e.CRI_RegisteredOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CRI_Total).HasDefaultValue(1);

            entity.HasOne(d => d.CLI).WithMany(p => p.Client_Requirements)
                .HasForeignKey(d => d.CLI_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Client_Requirements_Clients");

            entity.HasOne(d => d.DES).WithMany(p => p.Client_Requirements)
                .HasForeignKey(d => d.DES_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Client_Requirements_Designations");
        });

        modelBuilder.Entity<Client_Requirement_Allowance>(entity =>
        {
            entity.HasKey(e => e.CRA_Id);

            entity.Property(e => e.CRA_Amount).HasColumnType("decimal(9, 2)");
            entity.Property(e => e.CRA_DayswiseOrFull).HasComment("1: Daywise Calculation; 0: give all amount without calculation");

            entity.HasOne(d => d.ALL).WithMany(p => p.Client_Requirement_Allowances)
                .HasForeignKey(d => d.ALL_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Client_Requirement_Allowances_Allowances");

            entity.HasOne(d => d.CRI).WithMany(p => p.Client_Requirement_Allowances)
                .HasForeignKey(d => d.CRI_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Client_Requirement_Allowances_Client_Requirements");
        });

        modelBuilder.Entity<Clients_Employee>(entity =>
        {
            entity.HasKey(e => e.CLE_Id);

            entity.HasIndex(e => e.CLE_Id, "IX_Clients_Employees");

            entity.Property(e => e.CLE_ReassignedOn).HasColumnType("datetime");
            entity.Property(e => e.CLE_RegisteredOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CLE_UnassignedOn).HasColumnType("datetime");

            entity.HasOne(d => d.CLI).WithMany(p => p.Clients_Employees)
                .HasForeignKey(d => d.CLI_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Clients_Employees_Clients");

            entity.HasOne(d => d.DES).WithMany(p => p.Clients_Employees)
                .HasForeignKey(d => d.DES_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Clients_Employees_Designations");

            entity.HasOne(d => d.EMP).WithMany(p => p.Clients_Employees)
                .HasForeignKey(d => d.EMP_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Clients_Employees_Employees");
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.COU_Id).HasName("PK_countries");

            entity.Property(e => e.COU_Code)
                .IsRequired()
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasDefaultValue("")
                .IsFixedLength();
            entity.Property(e => e.COU_Name)
                .IsRequired()
                .HasMaxLength(45)
                .IsUnicode(false)
                .HasDefaultValue("");
        });

        modelBuilder.Entity<Designation>(entity =>
        {
            entity.HasKey(e => e.DES_Id);

            entity.Property(e => e.DES_Title)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Document_Type>(entity =>
        {
            entity.HasKey(e => e.DOT_Id);

            entity.Property(e => e.DOT_Title)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EMP_Id);

            entity.Property(e => e.EMP_Aadhar_Name)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.EMP_Aadhar_Number)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.EMP_Account_Name)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.EMP_Account_Number)
                .HasMaxLength(25)
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
            entity.Property(e => e.EMP_DOB).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.EMP_Designation)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.EMP_ESIC_Number)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValue("Pending");
            entity.Property(e => e.EMP_ESIC_Remark)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.EMP_FirstName)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.EMP_Gender).HasComment("1:Male;0:Female");
            entity.Property(e => e.EMP_InactivatedOn).HasColumnType("datetime");
            entity.Property(e => e.EMP_IsActive).HasDefaultValue(true);
            entity.Property(e => e.EMP_Is_IDBI_Other).HasComment("0: IDBI to IDBI ;1: IDBI to Other;");
            entity.Property(e => e.EMP_LIN_Number)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.EMP_LIN_Remark)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.EMP_MiddleName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.EMP_Pan_Number)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.EMP_Payment_Type).HasComment("0:BankAccount; 1:Cheque &Cash");
            entity.Property(e => e.EMP_Permanent_Address)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.EMP_RegisteredOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EMP_RejoinOn).HasColumnType("datetime");
            entity.Property(e => e.EMP_SurName)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.EMP_TPC_EmployeeId)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.EMP_Temporary_Address)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.EMP_UAN_Number)
                .HasMaxLength(12)
                .IsUnicode(false)
                .HasDefaultValue("Pending");
            entity.Property(e => e.EMP_UAN_Remark)
                .HasMaxLength(200)
                .IsUnicode(false);

            entity.HasOne(d => d.EMP_CityNavigation).WithMany(p => p.Employees)
                .HasForeignKey(d => d.EMP_City)
                .HasConstraintName("FK_Employees_Cities_all");

            entity.HasOne(d => d.EMP_StateNavigation).WithMany(p => p.Employees)
                .HasForeignKey(d => d.EMP_State)
                .HasConstraintName("FK_Employees_States");

            entity.HasOne(d => d.FRM).WithMany(p => p.Employees)
                .HasForeignKey(d => d.FRM_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Employees_Firms");
        });

        modelBuilder.Entity<Employee_Advance>(entity =>
        {
            entity.HasKey(e => e.ADV_Id);

            entity.ToTable("Employee_Advance");

            entity.Property(e => e.ADV_Amount).HasColumnType("decimal(9, 2)");
            entity.Property(e => e.ADV_RegisteredOn).HasColumnType("datetime");
            entity.Property(e => e.ADV_Status).HasComment("false: Pending True: Done");

            entity.HasOne(d => d.EMP).WithMany(p => p.Employee_Advances)
                .HasForeignKey(d => d.EMP_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Employee_Advance_Employees");

            entity.HasOne(d => d.WAG_Id_Closed_OnNavigation).WithMany(p => p.Employee_Advances)
                .HasForeignKey(d => d.WAG_Id_Closed_On)
                .HasConstraintName("FK_Employee_Advance_Wage_Process");
        });

        modelBuilder.Entity<Employee_Document>(entity =>
        {
            entity.HasKey(e => e.EMD_Id);

            entity.Property(e => e.EMD_Name)
                .IsRequired()
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.EMD_UploadedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.DOT).WithMany(p => p.Employee_Documents)
                .HasForeignKey(d => d.DOT_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Employee_Documents_Document_Types");

            entity.HasOne(d => d.EMP).WithMany(p => p.Employee_Documents)
                .HasForeignKey(d => d.EMP_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Employee_Documents_Employees");
        });

        modelBuilder.Entity<Firm>(entity =>
        {
            entity.HasKey(e => e.FRM_Id);

            entity.Property(e => e.FRM_AccountNumber)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FRM_Address1).IsUnicode(false);
            entity.Property(e => e.FRM_Address2).IsUnicode(false);
            entity.Property(e => e.FRM_BankName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FRM_Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FRM_GST_No)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FRM_IFSC_Code)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.FRM_InvoicingName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FRM_Name)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FRM_ShortName)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.STA).WithMany(p => p.Firms)
                .HasForeignKey(d => d.STA_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Firms_States");
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.INV_Id);

            entity.Property(e => e.INV_CGST_Total).HasColumnType("decimal(9, 2)");
            entity.Property(e => e.INV_ClientOrder_Number)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.INV_CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.INV_HSN)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.INV_IGST_Total).HasColumnType("decimal(9, 2)");
            entity.Property(e => e.INV_Number)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.INV_Remark)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.INV_SGST_Total).HasColumnType("decimal(9, 2)");
            entity.Property(e => e.INV_Total).HasColumnType("decimal(9, 2)");

            entity.HasOne(d => d.CLI).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.CLI_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Invoices_Clients");

            entity.HasOne(d => d.FRM).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.FRM_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Invoices_Firms");
        });

        modelBuilder.Entity<Invoice_Concept>(entity =>
        {
            entity.HasKey(e => e.INC_Id);

            entity.Property(e => e.INC_Description)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.INC_Total).HasColumnType("decimal(9, 2)");

            entity.HasOne(d => d.INV).WithMany(p => p.Invoice_Concepts)
                .HasForeignKey(d => d.INV_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Invoice_Concepts_Invoices");
        });

        modelBuilder.Entity<ProfessionalTaxCalculation>(entity =>
        {
            entity.HasKey(e => e.PTC_Id).HasName("PK_Pro");

            entity.ToTable("ProfessionalTaxCalculation");

            entity.Property(e => e.PTC_Amount).HasColumnType("decimal(9, 2)");
            entity.Property(e => e.PTC_From).HasColumnType("decimal(9, 2)");
            entity.Property(e => e.PTC_MF)
                .IsRequired()
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.PTC_To).HasColumnType("decimal(9, 2)");
        });

        modelBuilder.Entity<State>(entity =>
        {
            entity.HasKey(e => e.STA_Id).HasName("PK_states");

            entity.Property(e => e.STA_GST_Code)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.STA_Name)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.COU).WithMany(p => p.States)
                .HasForeignKey(d => d.COU_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_states_Countries");
        });

        modelBuilder.Entity<Wage_PaySlip>(entity =>
        {
            entity.HasKey(e => e.WPS_Id);

            entity.Property(e => e.WPS_FileName)
                .IsRequired()
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.WPS_GeneratedOn).HasColumnType("datetime");
            entity.Property(e => e.WPS_Status).HasDefaultValue(1);

            entity.HasOne(d => d.EMP).WithMany(p => p.Wage_PaySlips)
                .HasForeignKey(d => d.EMP_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Wage_PaySlips_Employees");

            entity.HasOne(d => d.WAG).WithMany(p => p.Wage_PaySlips)
                .HasForeignKey(d => d.WAG_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Wage_PaySlips_Wage_Process");
        });

        modelBuilder.Entity<Wage_Process>(entity =>
        {
            entity.HasKey(e => e.WAG_Id);

            entity.ToTable("Wage_Process");

            entity.Property(e => e.WAG_RegisteredOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.FRM).WithMany(p => p.Wage_Processes)
                .HasForeignKey(d => d.FRM_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Wage_Process_Firms");
        });

        modelBuilder.Entity<Wage_Process_Client>(entity =>
        {
            entity.HasKey(e => e.WPC_Id);

            entity.Property(e => e.WPC_SavedOn).HasColumnType("datetime");

            entity.HasOne(d => d.CLI).WithMany(p => p.Wage_Process_Clients)
                .HasForeignKey(d => d.CLI_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Wage_Process_Clients_Clients");

            entity.HasOne(d => d.WAG).WithMany(p => p.Wage_Process_Clients)
                .HasForeignKey(d => d.WAG_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Wage_Process_Clients_Wage_Process");
        });

        modelBuilder.Entity<Wage_Register>(entity =>
        {
            entity.HasKey(e => e.WAR_Id);

            entity.ToTable("Wage_Register");

            entity.Property(e => e.WAR_Advance_Amount).HasColumnType("decimal(9, 2)");
            entity.Property(e => e.WAR_Allowance_Calculated_1).HasColumnType("decimal(9, 2)");
            entity.Property(e => e.WAR_Allowance_Calculated_10).HasColumnType("decimal(9, 2)");
            entity.Property(e => e.WAR_Allowance_Calculated_2).HasColumnType("decimal(9, 2)");
            entity.Property(e => e.WAR_Allowance_Calculated_3).HasColumnType("decimal(9, 2)");
            entity.Property(e => e.WAR_Allowance_Calculated_4).HasColumnType("decimal(9, 2)");
            entity.Property(e => e.WAR_Allowance_Calculated_5).HasColumnType("decimal(9, 2)");
            entity.Property(e => e.WAR_Allowance_Calculated_6).HasColumnType("decimal(9, 2)");
            entity.Property(e => e.WAR_Allowance_Calculated_7).HasColumnType("decimal(9, 2)");
            entity.Property(e => e.WAR_Allowance_Calculated_8).HasColumnType("decimal(9, 2)");
            entity.Property(e => e.WAR_Allowance_Calculated_9).HasColumnType("decimal(9, 2)");
            entity.Property(e => e.WAR_Attendance_Allowance_Calculated).HasColumnType("decimal(9, 2)");
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
            entity.Property(e => e.WAR_LWF_Deduction_Employee).HasColumnType("decimal(9, 2)");
            entity.Property(e => e.WAR_LWF_Deduction_Employer).HasColumnType("decimal(9, 2)");
            entity.Property(e => e.WAR_LastModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.WAR_Nightshift_Allowance_Calculated).HasColumnType("decimal(9, 2)");
            entity.Property(e => e.WAR_OutStation_Allowance_Calculated).HasColumnType("decimal(9, 2)");
            entity.Property(e => e.WAR_OverTime_Calculated).HasColumnType("decimal(9, 2)");
            entity.Property(e => e.WAR_OverTime_Formula).HasMaxLength(200);
            entity.Property(e => e.WAR_OverTime_Payment).HasDefaultValue(1);
            entity.Property(e => e.WAR_PF).HasColumnType("decimal(9, 2)");
            entity.Property(e => e.WAR_PF_Calculated).HasColumnType("decimal(9, 2)");
            entity.Property(e => e.WAR_PF_Formula).HasMaxLength(200);
            entity.Property(e => e.WAR_Performance_Allowance_Calculated).HasColumnType("decimal(9, 2)");
            entity.Property(e => e.WAR_ProffesionalTax_Calculated)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.WAR_RevenueDeduction_Calculated)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.WAR_WorkingHrs_In_Day).HasDefaultValue(8);

            entity.HasOne(d => d.CLI).WithMany(p => p.Wage_Registers)
                .HasForeignKey(d => d.CLI_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Wage_Register_Clients");

            entity.HasOne(d => d.CRI).WithMany(p => p.Wage_Registers)
                .HasForeignKey(d => d.CRI_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Wage_Register_Client_Requirements");

            entity.HasOne(d => d.EMP).WithMany(p => p.Wage_Registers)
                .HasForeignKey(d => d.EMP_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Wage_Register_Employees");

            entity.HasOne(d => d.WAG).WithMany(p => p.Wage_Registers)
                .HasForeignKey(d => d.WAG_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Wage_Register_Wage_Process");
        });

        modelBuilder.Entity<Wage_Register_Advance>(entity =>
        {
            entity.HasKey(e => e.WAD_Id);

            entity.Property(e => e.WAD_Amount).HasColumnType("decimal(9, 2)");
            entity.Property(e => e.WAD_ClosedOn).HasColumnType("datetime");

            entity.HasOne(d => d.EMP).WithMany(p => p.Wage_Register_Advances)
                .HasForeignKey(d => d.EMP_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Wage_Register_Advances_Employees");

            entity.HasOne(d => d.WAG).WithMany(p => p.Wage_Register_Advances)
                .HasForeignKey(d => d.WAG_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Wage_Register_Advances_Wage_Process");
        });

        modelBuilder.Entity<Wage_Register_Allowance>(entity =>
        {
            entity.HasKey(e => e.WAA_Id);

            entity.Property(e => e.WAA_Amount).HasColumnType("decimal(9, 2)");
            entity.Property(e => e.WAA_Amount_Calculated).HasColumnType("decimal(9, 2)");

            entity.HasOne(d => d.CRA).WithMany(p => p.Wage_Register_Allowances)
                .HasForeignKey(d => d.CRA_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Wage_Register_Allowances_Client_Requirement_Allowances");

            entity.HasOne(d => d.WAR).WithMany(p => p.Wage_Register_Allowances)
                .HasForeignKey(d => d.WAR_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Wage_Register_Allowances_Wage_Register");
        });

        modelBuilder.Entity<Wage_Register_Allowances_1>(entity =>
        {
            entity.HasKey(e => e.WRA_Id_1);

            entity.ToTable("Wage_Register_Allowances_1");

            entity.Property(e => e.WRA_Amount_1).HasColumnType("decimal(9, 2)");

            entity.HasOne(d => d.CLE).WithMany(p => p.Wage_Register_Allowances_1s)
                .HasForeignKey(d => d.CLE_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Wage_Register_Allowances_1_Clients_Employees");

            entity.HasOne(d => d.WAG).WithMany(p => p.Wage_Register_Allowances_1s)
                .HasForeignKey(d => d.WAG_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Wage_Register_Allowances_1_Wage_Process");
        });

        modelBuilder.Entity<Wage_Register_Allowances_10>(entity =>
        {
            entity.HasKey(e => e.WRA_Id_10);

            entity.ToTable("Wage_Register_Allowances_10");

            entity.Property(e => e.WRA_Amount_10).HasColumnType("decimal(9, 5)");

            entity.HasOne(d => d.CLE).WithMany(p => p.Wage_Register_Allowances_10s)
                .HasForeignKey(d => d.CLE_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Wage_Register_Allowances_10_Clients_Employees");

            entity.HasOne(d => d.WAG).WithMany(p => p.Wage_Register_Allowances_10s)
                .HasForeignKey(d => d.WAG_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Wage_Register_Allowances_10_Wage_Process");
        });

        modelBuilder.Entity<Wage_Register_Allowances_2>(entity =>
        {
            entity.HasKey(e => e.WRA_Id_2);

            entity.ToTable("Wage_Register_Allowances_2");

            entity.Property(e => e.WRA_Amount_2).HasColumnType("decimal(9, 2)");

            entity.HasOne(d => d.CLE).WithMany(p => p.Wage_Register_Allowances_2s)
                .HasForeignKey(d => d.CLE_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Wage_Register_Allowances_2_Clients_Employees");

            entity.HasOne(d => d.WAG).WithMany(p => p.Wage_Register_Allowances_2s)
                .HasForeignKey(d => d.WAG_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Wage_Register_Allowances_2_Wage_Process");
        });

        modelBuilder.Entity<Wage_Register_Allowances_3>(entity =>
        {
            entity.HasKey(e => e.WRA_Id_3);

            entity.ToTable("Wage_Register_Allowances_3");

            entity.Property(e => e.WRA_Amount_3).HasColumnType("decimal(9, 3)");

            entity.HasOne(d => d.CLE).WithMany(p => p.Wage_Register_Allowances_3s)
                .HasForeignKey(d => d.CLE_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Wage_Register_Allowances_3_Clients_Employees");

            entity.HasOne(d => d.WAG).WithMany(p => p.Wage_Register_Allowances_3s)
                .HasForeignKey(d => d.WAG_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Wage_Register_Allowances_3_Wage_Process");
        });

        modelBuilder.Entity<Wage_Register_Allowances_4>(entity =>
        {
            entity.HasKey(e => e.WRA_Id_4);

            entity.ToTable("Wage_Register_Allowances_4");

            entity.Property(e => e.WRA_Amount_4).HasColumnType("decimal(9, 4)");

            entity.HasOne(d => d.CLE).WithMany(p => p.Wage_Register_Allowances_4s)
                .HasForeignKey(d => d.CLE_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Wage_Register_Allowances_4_Clients_Employees");

            entity.HasOne(d => d.WAG).WithMany(p => p.Wage_Register_Allowances_4s)
                .HasForeignKey(d => d.WAG_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Wage_Register_Allowances_4_Wage_Process");
        });

        modelBuilder.Entity<Wage_Register_Allowances_5>(entity =>
        {
            entity.HasKey(e => e.WRA_Id_5);

            entity.ToTable("Wage_Register_Allowances_5");

            entity.Property(e => e.WRA_Amount_5).HasColumnType("decimal(9, 5)");

            entity.HasOne(d => d.CLE).WithMany(p => p.Wage_Register_Allowances_5s)
                .HasForeignKey(d => d.CLE_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Wage_Register_Allowances_5_Clients_Employees");

            entity.HasOne(d => d.WAG).WithMany(p => p.Wage_Register_Allowances_5s)
                .HasForeignKey(d => d.WAG_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Wage_Register_Allowances_5_Wage_Process");
        });

        modelBuilder.Entity<Wage_Register_Allowances_6>(entity =>
        {
            entity.HasKey(e => e.WRA_Id_6);

            entity.ToTable("Wage_Register_Allowances_6");

            entity.Property(e => e.WRA_Amount_6).HasColumnType("decimal(9, 5)");

            entity.HasOne(d => d.CLE).WithMany(p => p.Wage_Register_Allowances_6s)
                .HasForeignKey(d => d.CLE_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Wage_Register_Allowances_6_Clients_Employees");

            entity.HasOne(d => d.WAG).WithMany(p => p.Wage_Register_Allowances_6s)
                .HasForeignKey(d => d.WAG_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Wage_Register_Allowances_6_Wage_Process");
        });

        modelBuilder.Entity<Wage_Register_Allowances_7>(entity =>
        {
            entity.HasKey(e => e.WRA_Id_7);

            entity.ToTable("Wage_Register_Allowances_7");

            entity.Property(e => e.WRA_Amount_7).HasColumnType("decimal(9, 5)");

            entity.HasOne(d => d.CLE).WithMany(p => p.Wage_Register_Allowances_7s)
                .HasForeignKey(d => d.CLE_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Wage_Register_Allowances_7_Clients_Employees");

            entity.HasOne(d => d.WAG).WithMany(p => p.Wage_Register_Allowances_7s)
                .HasForeignKey(d => d.WAG_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Wage_Register_Allowances_7_Wage_Process");
        });

        modelBuilder.Entity<Wage_Register_Allowances_8>(entity =>
        {
            entity.HasKey(e => e.WRA_Id_8);

            entity.ToTable("Wage_Register_Allowances_8");

            entity.Property(e => e.WRA_Amount_8).HasColumnType("decimal(9, 5)");

            entity.HasOne(d => d.CLE).WithMany(p => p.Wage_Register_Allowances_8s)
                .HasForeignKey(d => d.CLE_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Wage_Register_Allowances_8_Clients_Employees");

            entity.HasOne(d => d.WAG).WithMany(p => p.Wage_Register_Allowances_8s)
                .HasForeignKey(d => d.WAG_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Wage_Register_Allowances_8_Wage_Process");
        });

        modelBuilder.Entity<Wage_Register_Allowances_9>(entity =>
        {
            entity.HasKey(e => e.WRA_Id_9);

            entity.ToTable("Wage_Register_Allowances_9");

            entity.Property(e => e.WRA_Amount_9).HasColumnType("decimal(9, 5)");

            entity.HasOne(d => d.CLE).WithMany(p => p.Wage_Register_Allowances_9s)
                .HasForeignKey(d => d.CLE_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Wage_Register_Allowances_9_Clients_Employees");

            entity.HasOne(d => d.WAG).WithMany(p => p.Wage_Register_Allowances_9s)
                .HasForeignKey(d => d.WAG_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Wage_Register_Allowances_9_Wage_Process");
        });

        modelBuilder.Entity<Wage_Register_Canteen>(entity =>
        {
            entity.HasKey(e => e.WRC_Id);

            entity.ToTable("Wage_Register_Canteen");

            entity.Property(e => e.WRC_Amount).HasColumnType("decimal(9, 2)");

            entity.HasOne(d => d.CLE).WithMany(p => p.Wage_Register_Canteens)
                .HasForeignKey(d => d.CLE_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Wage_Register_Canteen_Clients_Employees");

            entity.HasOne(d => d.WAG).WithMany(p => p.Wage_Register_Canteens)
                .HasForeignKey(d => d.WAG_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Wage_Register_Canteen_Wage_Process");
        });

        modelBuilder.Entity<Wage_Register_Outstation>(entity =>
        {
            entity.HasKey(e => e.WRO_Id);

            entity.ToTable("Wage_Register_Outstation");

            entity.HasOne(d => d.CLE).WithMany(p => p.Wage_Register_Outstations)
                .HasForeignKey(d => d.CLE_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Wage_Register_Outstation_Clients_Employees");

            entity.HasOne(d => d.WAG).WithMany(p => p.Wage_Register_Outstations)
                .HasForeignKey(d => d.WAG_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Wage_Register_Outstation_Wage_Process");
        });

        modelBuilder.Entity<Wage_Register_Performance>(entity =>
        {
            entity.HasKey(e => e.WRP_Id);

            entity.ToTable("Wage_Register_Performance");

            entity.Property(e => e.WRP_Amount).HasColumnType("decimal(9, 2)");

            entity.HasOne(d => d.CLE).WithMany(p => p.Wage_Register_Performances)
                .HasForeignKey(d => d.CLE_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Wage_Register_Performance_Clients_Employees");

            entity.HasOne(d => d.WAG).WithMany(p => p.Wage_Register_Performances)
                .HasForeignKey(d => d.WAG_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Wage_Register_Performance_Wage_Process");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
