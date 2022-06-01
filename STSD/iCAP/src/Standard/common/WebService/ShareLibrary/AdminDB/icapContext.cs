using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ShareLibrary.AdminDB
{
    public partial class icapContext: DbContext
    {
        public virtual DbSet<Branch> Branch { get; set; }
        public virtual DbSet<Branchdashboard> Branchdashboard { get; set; }
        public virtual DbSet<Branchdashboardelement> Branchdashboardelement { get; set; }
        public virtual DbSet<Branchdashboardlist> Branchdashboardlist { get; set; }
        public virtual DbSet<Branchroles> Branchroles { get; set; }
        public virtual DbSet<Company> Company { get; set; }
        public virtual DbSet<Companydashboard> Companydashboard { get; set; }
        public virtual DbSet<Companydashboardelement> Companydashboardelement { get; set; }
        public virtual DbSet<Data> Data { get; set; }
        public virtual DbSet<Device> Device { get; set; }
        public virtual DbSet<Devicecertificate> Devicecertificate { get; set; }
        public virtual DbSet<Deviceclass> Deviceclass { get; set; }
        public virtual DbSet<Devicedatalist> Devicedatalist { get; set; }
        public virtual DbSet<Employee> Employee { get; set; }
        public virtual DbSet<Widget> Widget { get; set; }
        public virtual DbSet<LicenseList> LicenseList { get; set; }
        public virtual DbSet<DataGroup> DataGroup { get; set; }
        public virtual DbSet<Email> Email { get; set; }
        public virtual DbSet<Chart> Chart { get; set; }
        public virtual DbSet<DataChartList> DataChartList { get; set; }
        public virtual DbSet<WidgetBranchList> WidgetBranchList { get; set; }
        public virtual DbSet<BranchDeviceList> BranchDeviceList { get; set; }
        public virtual DbSet<Threshold> Threshold { get; set; }
        public virtual DbSet<ThresholdBranchList> ThresholdBranchList { get; set; }
        public virtual DbSet<ThresholdEmployeeList> ThresholdEmployeeList { get; set; }
        public virtual DbSet<ThresholdExternalRecipientList> ThresholdExternalRecipientList { get; set; }
        public virtual DbSet<Permission> Permission { get; set; }
        public virtual DbSet<ExternalRecipient> ExternalRecipient { get; set; }
        public virtual DbSet<ThresholdPermissionList> ThresholdPermissionList { get; set; }
        public virtual DbSet<Marker> Marker { get; set; }
        public virtual DbSet<MarkerDevicelist> MarkerDevicelist { get; set; }
        public virtual DbSet<DeviceRawData> DeviceRawData { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
            //optionsBuilder.UseMySql(@"server=172.30.0.3;userid=root;pwd=admin;port=3306;database=icap;persistsecurityinfo=True");
            //optionsBuilder.UseMySql(@"server=172.16.50.59;userid=root;pwd=admin;port=3389;database=icap;persistsecurityinfo=True");
            string ConnectionString;

            if (File.Exists("DBSetting.json"))
            {
                using (StreamReader sr = new StreamReader(new FileStream("DBSetting.json", FileMode.Open)))
                {
                    dynamic str = JsonConvert.DeserializeObject(sr.ReadLine());
                    ConnectionString = str.AdminDBConnectionString;
                }
            }
            else
            {
                ConnectionString = "server=172.30.0.3;userid=root;pwd=admin;port=3306;database=icap;persistsecurityinfo=True";
                var json = new
                {
                    AdminDBConnectionString = ConnectionString,
                    DataDBConnectionString = "mongodb://172.30.0.2:27017",
                    RedisConnectionString = "172.30.0.5"
                };

                using (StreamWriter sw = new StreamWriter(new FileStream("DBSetting.json", FileMode.CreateNew)))
                {
                    sw.WriteLine(JsonConvert.SerializeObject(json));
                }
            }
            optionsBuilder.UseMySql(ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            const string GOOGLEMAP = "5";
            const string GUEST = "1";
            //const string ADMIN = "2";

            modelBuilder.Entity<Branch>(entity =>
            {
                entity.ToTable("branch");

                entity.HasIndex(e => e.CompanyId)
                    .HasName("FK_Company_To_Branch");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.CompanyId) 
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("0");
                
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.DeletedFlag)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.Description).HasColumnType("varchar(255)");

                entity.Property(e => e.LastModifiedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Latitude).HasDefaultValueSql("0");

                entity.Property(e => e.Longitude).HasDefaultValueSql("0");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.PhotoUrl)
                    .HasColumnName("PhotoURL")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.TimeZone)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("0");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.Branch)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Company_To_Branch");

            });

            modelBuilder.Entity<Branchdashboard>(entity =>
            {
                entity.ToTable("branchdashboard");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.DeletedFlag)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.LastModifiedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(100)");
            });

            modelBuilder.Entity<Branchdashboardelement>(entity =>
            {
                entity.ToTable("branchdashboardelement");

                entity.HasIndex(e => e.DashboardId)
                    .HasName("FK_BranchDashboard_To_BranchDashboardElement");

                entity.HasIndex(e => e.WidgetId)
                    .HasName("FK_Widget_To_BranchDashboardElement");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Column)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.DashboardId)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.DeletedFlag)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.Height)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("100");

                entity.Property(e => e.LastModifiedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Row)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.WidgetId)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("0");

                entity.HasOne(d => d.Dashboard)
                    .WithMany(p => p.Branchdashboardelement)
                    .HasForeignKey(d => d.DashboardId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_BranchDashboard_To_BranchDashboardElement");

                entity.HasOne(d => d.Widget)
                    .WithMany(p => p.Branchdashboardelement)
                    .HasForeignKey(d => d.WidgetId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Widget_To_BranchDashboardElement");
            });

            modelBuilder.Entity<Branchdashboardlist>(entity =>
            {
                entity.ToTable("branchdashboardlist");

                entity.HasIndex(e => e.BranchId)
                    .HasName("FK_Branch_To_BranchDashboardList");

                entity.HasIndex(e => e.DashboardId)
                    .HasName("FK_BranchDashboard_To_BranchDashboardList");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.BranchId)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.DashboardId)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.DeletedFlag)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.LastModifiedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.Branch)
                    .WithMany(p => p.Branchdashboardlist)
                    .HasForeignKey(d => d.BranchId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Branch_To_BranchDashboardList");

                entity.HasOne(d => d.Dashboard)
                    .WithMany(p => p.Branchdashboardlist)
                    .HasForeignKey(d => d.DashboardId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_BranchDashboard_To_BranchDashboardList");
            });

            modelBuilder.Entity<Branchroles>(entity =>
            {
                entity.ToTable("branchroles");

                entity.HasIndex(e => e.BranchId)
                    .HasName("FK_Branch_To_BranchRoles");

                entity.HasIndex(e => e.EmployeeId)
                    .HasName("FK_Employee_To_BranchRoles");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.BranchId).HasColumnType("int(11)");

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.DeletedFlag)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.EmployeeId).HasColumnType("int(11)");

                entity.Property(e => e.LastModifiedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.Branch)
                    .WithMany(p => p.Branchroles)
                    .HasForeignKey(d => d.BranchId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Branch_To_BranchRoles");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.Branchroles)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Employee_To_BranchRoles");
            });

            modelBuilder.Entity<Company>(entity =>
            {
                entity.ToTable("company");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Address).HasColumnType("varchar(255)");

                entity.Property(e => e.ContactEmail).HasColumnType("varchar(255)");

                entity.Property(e => e.ContactName).HasColumnType("varchar(50)");

                entity.Property(e => e.ContactPhone).HasColumnType("varchar(50)");

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.DeletedFlag)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.LastModifiedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.LogoUrl)
                    .HasColumnName("LogoURL")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.ShortName).HasColumnType("varchar(10)");

                entity.Property(e => e.WebSite).HasColumnType("varchar(255)");
            });

            modelBuilder.Entity<Companydashboard>(entity =>
            {
                entity.ToTable("companydashboard");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.HasIndex(e => e.CompanyId)
                    .HasName("FK_Company_To_Companydashboard");

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.DeletedFlag)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.LastModifiedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(100)");
            });

            modelBuilder.Entity<Companydashboardelement>(entity =>
            {
                entity.ToTable("companydashboardelement");

                entity.HasIndex(e => e.DashboardId)
                    .HasName("FK_CompanyDashboard_To_CompanyDashboardElement");

                entity.HasIndex(e => e.WidgetId)
                    .HasName("FK_Widget_To_CompanyDashboardElement");

                entity.Property(e => e.Id).HasColumnType("int(11)");
                
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.DashboardId)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.DeletedFlag)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.LastModifiedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.IteratorIndex)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("1");

                entity.Property(e => e.WidgetId)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("0");

                entity.HasOne(d => d.Dashboard)
                    .WithMany(p => p.Companydashboardelement)
                    .HasForeignKey(d => d.DashboardId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_CompanyDashboard_To_CompanyDashboardElement");

                entity.HasOne(d => d.Widget)
                    .WithMany(p => p.Companydashboardelement)
                    .HasForeignKey(d => d.WidgetId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Widget_To_CompanyDashboardElement");
            });

            modelBuilder.Entity<Data>(entity =>
            {
                entity.ToTable("data");

                entity.HasIndex(e => e.GroupId)
                    .HasName("FK_DataGroup_To_Data");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.DeletedFlag)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.LastModifiedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.Location)
                    .HasColumnType("varchar(255)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.GroupId)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Numberical)
                    .IsRequired()
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.Unit)
                .HasColumnType("varchar(100)");

                entity.Property(e => e.Dynamic)
                .IsRequired()
                .HasColumnType("bit(1)")
                .HasDefaultValueSql("b'0'");

                entity.Property(e => e.EventMessage)
                .HasColumnType("varchar(255)");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.Data)
                    .HasForeignKey(d => d.GroupId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_DataGroup_To_Data");
            });

            modelBuilder.Entity<DataGroup>(entity =>
            {
                entity.ToTable("datagroup");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.DeletedFlag)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.LastModifiedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(100)");
            });

            modelBuilder.Entity<Device>(entity =>
            {
                entity.ToTable("device");           

                entity.HasIndex(e => e.DeviceClassId)
                    .HasName("FK_DeviceClass_To_Device");

                entity.HasIndex(e => e.OwnerId)
                    .HasName("FK_Employee_To_Device");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Alias).HasColumnType("varchar(100)");

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.DeletedFlag)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.DeviceClassId)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.LastModifiedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Latitude).HasDefaultValueSql("0");

                entity.Property(e => e.Longitude).HasDefaultValueSql("0");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.UploadInterval)
                    .HasColumnType("int(11)");

                entity.Property(e => e.PhotoUrl)
                    .HasColumnName("PhotoURL")
                    .HasColumnType("varchar(100)");

                entity.HasOne(d => d.DeviceClass)
                    .WithMany(p => p.Device)
                    .HasForeignKey(d => d.DeviceClassId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_DeviceClass_To_Device");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.Device)
                    .HasForeignKey(d => d.OwnerId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_Employee_To_Device");
            });

            modelBuilder.Entity<Devicecertificate>(entity =>
            {
                entity.ToTable("devicecertificate");

                entity.HasIndex(e => e.DeviceId)
                    .HasName("FK_Device_To_DeviceCertificate");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.DeletedFlag)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.DeviceId)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.LastModifiedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.ExpiredDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Thumbprint)
                    .IsRequired()
                    .HasColumnType("varchar(255)");

                entity.HasOne(d => d.Device)
                    .WithMany(p => p.Devicecertificate)
                    .HasForeignKey(d => d.DeviceId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Device_To_DeviceCertificate");
            });

            modelBuilder.Entity<Deviceclass>(entity =>
            {
                entity.ToTable("deviceclass");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.DeletedFlag)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.Description).HasColumnType("varchar(255)");

                entity.Property(e => e.LastModifiedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(100)");
            });

            modelBuilder.Entity<Devicedatalist>(entity =>
            {
                entity.ToTable("devicedatalist");

                entity.HasIndex(e => e.DataId)
                    .HasName("FK_Data_To_DeviceDateList");

                entity.HasIndex(e => e.DeviceId)
                    .HasName("FK_Device_To_DeviceDataList");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.DataId)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.DeletedFlag)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.DeviceId)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.LastModifiedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.Data)
                    .WithMany(p => p.Devicedatalist)
                    .HasForeignKey(d => d.DataId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Data_To_DeviceDateList");

                entity.HasOne(d => d.Device)
                    .WithMany(p => p.Devicedatalist)
                    .HasForeignKey(d => d.DeviceId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Device_To_DeviceDataList");
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.ToTable("employee");

                entity.HasIndex(e => e.CompanyId)
                    .HasName("FK_Company_To_Employee");

                entity.HasIndex(e => e.MapId)
                    .HasName("FK_Chart_To_Employee");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.PermissionId)
                .HasColumnType("int(11)")
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql(GUEST)
                .Metadata.IsReadOnlyAfterSave = false;

                entity.Property(e => e.PermissionId)
                .Metadata.IsReadOnlyBeforeSave = false;

                //entity.Property(e => e.AdminFlag)
                //    .HasColumnType("bit(1)")
                //    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.CompanyId).HasColumnType("int(11)");
                entity.Property(e => e.MapId)
                      .HasColumnType("int(11)")
                      .ValueGeneratedOnAdd()
                      .HasDefaultValueSql(GOOGLEMAP)
                      .Metadata.IsReadOnlyAfterSave = false;

                entity.Property(e => e.MapId)
                      .Metadata.IsReadOnlyBeforeSave = false;

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.DeletedFlag)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.Email)                    
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.EmployeeNumber).HasColumnType("varchar(50)");

                entity.Property(e => e.FirstName).HasColumnType("varchar(50)");

                entity.Property(e => e.LastModifiedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.LastName).HasColumnType("varchar(50)");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.LoginName)
                    .IsRequired()
                    .HasColumnType("varchar(255)");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.Employee)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Company_To_Employee");

                entity.HasOne(d => d.Chart)
                    .WithMany(p => p.Employee)
                    .HasForeignKey(d => d.MapId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Chart_To_Employee");

                entity.HasOne(d => d.Permission)
                .WithMany(p => p.Employee)
                .HasForeignKey(d => d.PermissionId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_UserPermission_To_Employee");
            });

            modelBuilder.Entity<Widget>(entity =>
            {
                entity.ToTable("widget");

                entity.HasIndex(e => e.DataId)
                    .HasName("FK_Data_To_Widget");

                entity.HasIndex(e => e.ChartId)
                    .HasName("FK_Chart_To_Widget");
                
                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.DataCount)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("1");

                entity.Property(e => e.DataId)
                    .HasColumnType("int(11)");                  

                entity.Property(e => e.DeletedFlag)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.LastModifiedDate)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.Width)
                    .IsRequired()
                    .HasColumnType("tinyint(2) unsigned");

                entity.Property(e => e.ChartId)
                    .HasColumnType("int(11)");
                    
                entity.Property(e => e.SettingStr)                    
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.ThresholdId)
                    .HasColumnType("int(11)");


                entity.HasOne(d => d.Data)
                    .WithMany(p => p.Widget)
                    .HasForeignKey(d => d.DataId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Data_To_Widget");

                entity.HasOne(d => d.Chart)
                    .WithMany(p => p.Widget)
                    .HasForeignKey(d => d.ChartId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Chart_To_Widget");

                entity.HasOne(d => d.Threshold)
                    .WithMany(p => p.Widget)
                    .HasForeignKey(d => d.ThresholdId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_Threshold_To_Widget");
            });

            modelBuilder.Entity<LicenseList>(entity =>
            {
                entity.ToTable("licenselist");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.DeviceCount).HasColumnType("int(11)").IsRequired().HasDefaultValue(0);

                entity.Property(e => e.Key).HasColumnType("varchar(4096)").IsRequired();

            });

            modelBuilder.Entity<Email>(entity => 
            {
                entity.ToTable("email");

                entity.HasIndex(e => e.CompanyId)
                    .HasName("FK_Company_To_Email");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)");

                entity.Property(e => e.CompanyId)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("1");

                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.LastModifiedDate)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.SMTPAddress)
                      .IsRequired()
                      .HasColumnType("varchar(255)");

                entity.Property(e => e.PortNumber)
                      .IsRequired()
                      .HasColumnType("int(11)");

                //entity.Property(e => e.EnableSSL)
                //      .IsRequired()
                //      .HasColumnType("bit(1)")
                //      .HasDefaultValueSql("b'1'");
                entity.Property(e => e.Encryption)
                .IsRequired()
                .HasColumnType("tinyint(2) unsigned");
                

                entity.Property(e => e.EmailFrom)
                    .IsRequired()
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Enable)
                      .HasColumnType("bit(1)")
                      .ValueGeneratedOnAdd()
                      .HasDefaultValueSql("b'1'")
                      .Metadata.IsReadOnlyAfterSave = false;

                entity.Property(e => e.Enable)
                      .Metadata.IsReadOnlyBeforeSave = false;


                entity.Property(e => e.ResendInterval)
                      .HasColumnType("int(11)")
                      .ValueGeneratedOnAdd()
                      .HasDefaultValueSql("43200")
                      .Metadata.IsReadOnlyAfterSave = false;

                entity.Property(e => e.ResendInterval)
                      .Metadata.IsReadOnlyBeforeSave = false;

                entity.HasOne(d => d.Company)
                    .WithMany(p=>p.Email)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Company_To_Email");
            });

            modelBuilder.Entity<Chart>(entity =>
            {
                entity.ToTable("chart");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.DeletedFlag)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.LastModifiedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasColumnType("int(11)");

                entity.Property(e => e.SizeFlag)                
                    .HasColumnType("int(11)")
                    .HasDefaultValue("1");
            });

            modelBuilder.Entity<DataChartList>(entity =>
            {
                entity.ToTable("datachartlist");

                entity.HasIndex(e => e.DataId)
                    .HasName("FK_Data_To_DataChartList");

                entity.HasIndex(e => e.ChartId)
                    .HasName("FK_Chart_To_DataChartList");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.DataId)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.ChartId)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.DeletedFlag)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.LastModifiedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.Data)
                    .WithMany(p => p.DataChartList)
                    .HasForeignKey(d => d.DataId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Data_To_DataChartList");

                entity.HasOne(d => d.Chart)
                    .WithMany(p => p.DataChartlist)
                    .HasForeignKey(d => d.ChartId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Chart_To_DataChartList");
            });

            modelBuilder.Entity<WidgetBranchList>(entity =>
            {
                entity.ToTable("widgetbranchlist");

                entity.HasIndex(e => e.BranchId)
                    .HasName("FK_Branch_To_WidgetBranchList");

                entity.HasIndex(e => e.WidgetId)
                    .HasName("FK_Widget_To_WidgetBranchList");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.BranchId)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.WidgetId)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.DeletedFlag)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.LastModifiedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.Branch)
                    .WithMany(p => p.WidgetBranchList)
                    .HasForeignKey(d => d.BranchId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Branch_To_WidgetBranchList");

                entity.HasOne(d => d.Widget)
                    .WithMany(p => p.WidgetBranchList)
                    .HasForeignKey(d => d.WidgetId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Widget_To_WidgetBranchList");
            });

            modelBuilder.Entity<BranchDeviceList>(entity =>
            {
                entity.ToTable("branchdevicelist");

                entity.HasIndex(e => e.BranchId)
                    .HasName("FK_Branch_To_BranchDeviceList");

                entity.HasIndex(e => e.DeviceId)
                    .HasName("FK_Device_To_BranchDeviceList");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.BranchId)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.DeviceId)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.DeletedFlag)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.LastModifiedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.Branch)
                    .WithMany(p => p.BranchDeviceList)
                    .HasForeignKey(d => d.BranchId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Branch_To_BranchDeviceList");

                entity.HasOne(d => d.Device)
                    .WithMany(p => p.BranchDeviceList)
                    .HasForeignKey(d => d.DeviceId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Device_To_BranchDeviceList");
            });

            modelBuilder.Entity<Threshold>(entity =>
            {
                entity.ToTable("threshold");

                entity.HasIndex(e => e.DataId)
                    .HasName("FK_Data_To_ThresholdDataId");

                entity.HasIndex(e => e.DenominatorId)
                    .HasName("FK_Data_To_ThresholdDenominatorId");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.DataId)
                    .IsRequired()
                    .HasColumnType("int(11)");

                entity.Property(e => e.DenominatorId)
                    .HasColumnType("int(11)");

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.DeletedFlag)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.LastModifiedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Name)
                .IsRequired()
                .HasColumnType("varchar(100)");

                entity.Property(e => e.Value)
                .HasColumnType("varchar(255)");

                entity.Property(e => e.Func)
                .IsRequired()
                .HasColumnType("int(11)");

                entity.Property(e => e.Enable)
                      .HasColumnType("bit(1)");

                //entity.Property(e => e.Enable)
                //      .HasColumnType("bit(1)")
                //      .ValueGeneratedOnAdd()
                //      .HasDefaultValueSql("b'1'")
                //      .Metadata.IsReadOnlyAfterSave = false;

                //entity.Property(e => e.Enable)
                //      .Metadata.IsReadOnlyBeforeSave = false;

                entity.Property(e => e.Action)
                .IsRequired()
                .HasColumnType("int(11)");

                entity.Property(e => e.Mode)
                .IsRequired()
                .HasColumnType("tinyint(2) unsigned");

                entity.HasOne(d => d.Data)
                .WithMany(p => p.Threshold)
                .HasForeignKey(d => d.DataId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Data_To_Threshold");

                entity.HasOne(d => d.Data)
                .WithMany(p => p.Threshold)
                .HasForeignKey(d => d.DenominatorId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Data_To_Threshold");
            });

            modelBuilder.Entity<ThresholdBranchList>(entity =>
            {
                entity.ToTable("thresholdbranchlist");

                entity.HasIndex(e => e.BranchId)
                    .HasName("FK_Branch_To_ThresholdBranchList");

                entity.HasIndex(e => e.ThresholdId)
                    .HasName("FK_Threshold_To_ThresholdBranchList");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.BranchId)
                    .IsRequired()
                    .HasColumnType("int(11)");
                    

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.ThresholdId)
                    .IsRequired()
                    .HasColumnType("int(11)");
                    

                entity.Property(e => e.DeletedFlag)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.LastModifiedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.Branch)
                    .WithMany(p => p.ThresholdBranchList)
                    .HasForeignKey(d => d.BranchId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Branch_To_ThresholdBranchList");

                entity.HasOne(d => d.Threshold)
                    .WithMany(p => p.ThresholdBranchList)
                    .HasForeignKey(d => d.ThresholdId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Threshold_To_ThresholdBranchList");
            });


            modelBuilder.Entity<ExternalRecipient>(entity =>
            {
                entity.ToTable("externalrecipient");

                entity.Property(e => e.Id).HasColumnType("int(11)");
                entity.Property(e => e.Email).HasColumnType("varchar(255)");
            });

            modelBuilder.Entity<ThresholdExternalRecipientList>(entity =>
            {
                entity.ToTable("thresholdexternalrecipientlist");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.DeletedFlag)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.LastModifiedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasIndex(e => e.ThresholdId)
                    .HasName("FK_Threshold_To_ThresholdExternalRecipientList");

                entity.Property(e => e.ThresholdId)
                    .IsRequired()
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Threshold)
                    .WithMany(p => p.ThresholdExternalRecipientList)
                    .HasForeignKey(d => d.ThresholdId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Threshold_To_ThresholdExternalRecipientList");

                entity.HasIndex(e => e.ExternalRecipientId)
                    .HasName("FK_ExternalRecipient_To_ThresholdExternalRecipientList");

                entity.Property(e => e.ExternalRecipientId)
                    .IsRequired()
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Threshold)
                    .WithMany(p => p.ThresholdExternalRecipientList)
                    .HasForeignKey(d => d.ExternalRecipientId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_ExternalRecipient_To_ThresholdExternalRecipientList");
            });

            modelBuilder.Entity<ThresholdPermissionList>(entity =>
            {
                entity.ToTable("thresholdpermissionlist");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.DeletedFlag)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.LastModifiedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasIndex(e => e.ThresholdId)
                    .HasName("FK_Threshold_To_ThresholdExternalRecipientList");

                entity.Property(e => e.ThresholdId)
                    .IsRequired()
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Threshold)
                    .WithMany(p => p.ThresholdPermissionList)
                    .HasForeignKey(d => d.ThresholdId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Threshold_To_ThresholdPermissionList");

                entity.HasIndex(e => e.PermissionId)
                    .HasName("FK_Permission_To_ThresholdPermissionList");

                entity.Property(e => e.PermissionId)
                    .IsRequired()
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Permission)
                    .WithMany(p => p.ThresholdPermissionList)
                    .HasForeignKey(d => d.PermissionId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Permission_To_ThresholdPermissionList");
            });

            modelBuilder.Entity<ThresholdEmployeeList>(entity =>
            {
                entity.ToTable("thresholdinternalrecipientlist");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.DeletedFlag)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.LastModifiedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasIndex(e => e.ThresholdId)
                    .HasName("FK_Threshold_To_ThresholdInternalRecipientList");

                entity.Property(e => e.ThresholdId)
                    .IsRequired()
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Threshold)
                    .WithMany(p => p.ThresholdInternalRecipientList)
                    .HasForeignKey(d => d.ThresholdId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Threshold_To_ThresholdInternalRecipientList");

                entity.HasIndex(e => e.EmployeeId)
                    .HasName("FK_Employee_To_ThresholdInternalRecipientList");

                entity.Property(e => e.EmployeeId)
                    .IsRequired()
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.ThresholdInternalRecipientList)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Employee_To_ThresholdInternalRecipientList");
            });

            modelBuilder.Entity<Permission>(entity => {
                entity.ToTable("permission");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.DeletedFlag)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.LastModifiedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Create)
                .IsRequired()
                .HasColumnType("int(11)");

                entity.Property(e => e.Update)
                .IsRequired()
                .HasColumnType("int(11)");

                entity.Property(e => e.Delete)
                .IsRequired()
                .HasColumnType("int(11)");

                entity.Property(e => e.Level)
                .IsRequired()
                .HasColumnType("tinyint(2) unsigned");
            });

            modelBuilder.Entity<Marker>(entity =>
            {
                entity.ToTable("marker");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.LastModifiedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasIndex(e => e.CustomizedMapId)
                    .HasName("FK_Widget_To_Marker");

                entity.Property(e => e.CustomizedMapId)
                    .IsRequired()
                    .HasColumnType("int(11)");

                entity.Property(e => e.OffsetX)
                    .IsRequired()
                    .HasColumnType("int(11)");

                entity.Property(e => e.OffsetY)
                    .IsRequired()
                    .HasColumnType("int(11)");

                entity.Property(e => e.PK_Guid)
                    .IsRequired()
                    .HasColumnType("varchar(100)");

                entity.HasOne(d => d.Widget)
                    .WithMany(p => p.Marker)
                    .HasForeignKey(d => d.CustomizedMapId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Widget_To_Marker");
            });

            modelBuilder.Entity<MarkerDevicelist>(entity =>
            {
                entity.ToTable("markerdevicelist");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.LastModifiedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasIndex(e => e.DeviceId)
                    .HasName("FK_Device_To_MarkerDevicelist");

                entity.Property(e => e.DeviceId)
                    .IsRequired()
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Device)
                    .WithMany(p => p.MarkerDevicelist)
                    .HasForeignKey(d => d.DeviceId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Deivce_To_MarkerDevicelist");

                entity.HasIndex(e => e.MarkerGuid)
                    .HasName("FK_Marker_To_MarkerDevicelist");

                entity.Property(e => e.MarkerGuid)
                    .IsRequired()
                    .HasColumnType("varchar(100)");

                entity.HasOne(d => d.Marker)
                    .WithMany(p => p.MarkerDevicelist)
                    .HasForeignKey(d => d.MarkerGuid)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Marker_To_MarkerDevicelist");
            });

            modelBuilder.Entity<DeviceRawData>(entity =>
            {
                entity.ToTable("devicerawdata");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.LastModifiedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.ExpireDate)
                .IsRequired()
                .HasColumnType("int(11)")
                .HasDefaultValue(365);
            });

        }
    }
}
