using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PizzaShop.Repository.Models;

public partial class PizzaShopContext : DbContext
{
    public PizzaShopContext()
    {
    }

    public PizzaShopContext(DbContextOptions<PizzaShopContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<Item> Items { get; set; }

    public virtual DbSet<ItemModifiergroupMapping> ItemModifiergroupMappings { get; set; }

    public virtual DbSet<Modfierandgroupsmapping> Modfierandgroupsmappings { get; set; }

    public virtual DbSet<Modifier> Modifiers { get; set; }

    public virtual DbSet<Modifiergroup> Modifiergroups { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<PermissionsRole> PermissionsRoles { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Section> Sections { get; set; }

    public virtual DbSet<State> States { get; set; }

    public virtual DbSet<Table> Tables { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Database=pizzaShop;Username=postgres;password=tatva123");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Accountid).HasName("account_pkey");

            entity.ToTable("account");

            entity.Property(e => e.Accountid).HasColumnName("accountid");
            entity.Property(e => e.Createdat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Createdbyid).HasColumnName("createdbyid");
            entity.Property(e => e.Deletedat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deletedat");
            entity.Property(e => e.Deletedbyid).HasColumnName("deletedbyid");
            entity.Property(e => e.Editedat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("editedat");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.Isdeleted)
                .HasDefaultValue(false)
                .HasColumnName("isdeleted");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Rememberme)
                .HasDefaultValue(false)
                .HasColumnName("rememberme");
            entity.Property(e => e.Roleid).HasColumnName("roleid");
            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .HasColumnName("username");

            entity.HasOne(d => d.Role).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.Roleid)
                .HasConstraintName("fk_roleid");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Categoryid).HasName("category_pkey");

            entity.ToTable("category");

            entity.Property(e => e.Categoryid).HasColumnName("categoryid");
            entity.Property(e => e.Categorydescription)
                .HasMaxLength(255)
                .HasColumnName("categorydescription");
            entity.Property(e => e.Categoryname)
                .HasMaxLength(50)
                .HasColumnName("categoryname");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Createdbyid).HasColumnName("createdbyid");
            entity.Property(e => e.Deletedat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deletedat");
            entity.Property(e => e.Deletedbyid).HasColumnName("deletedbyid");
            entity.Property(e => e.Editedat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("editedat");
            entity.Property(e => e.Editedbyid).HasColumnName("editedbyid");
            entity.Property(e => e.Isdeleted)
                .HasDefaultValue(false)
                .HasColumnName("isdeleted");
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.Cityid).HasName("city_pkey");

            entity.ToTable("city");

            entity.Property(e => e.Cityid).HasColumnName("cityid");
            entity.Property(e => e.Cityname)
                .HasMaxLength(50)
                .HasColumnName("cityname");
            entity.Property(e => e.Stateid).HasColumnName("stateid");

            entity.HasOne(d => d.State).WithMany(p => p.Cities)
                .HasForeignKey(d => d.Stateid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("city_stateid_fkey");
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.Countryid).HasName("country_pkey");

            entity.ToTable("country");

            entity.Property(e => e.Countryid).HasColumnName("countryid");
            entity.Property(e => e.Countryname)
                .HasMaxLength(50)
                .HasColumnName("countryname");
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.HasKey(e => e.Imageid).HasName("image_pkey");

            entity.ToTable("image");

            entity.Property(e => e.Imageid).HasColumnName("imageid");
            entity.Property(e => e.Imageurl)
                .HasMaxLength(500)
                .HasColumnName("imageurl");
            entity.Property(e => e.Userid).HasColumnName("userid");

            entity.HasOne(d => d.User).WithMany(p => p.Images)
                .HasForeignKey(d => d.Userid)
                .HasConstraintName("image_userid_fkey");
        });

        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.Itemid).HasName("item_pkey");

            entity.ToTable("item");

            entity.Property(e => e.Itemid).HasColumnName("itemid");
            entity.Property(e => e.Categoryid).HasColumnName("categoryid");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Createdbyid).HasColumnName("createdbyid");
            entity.Property(e => e.DefaultTax)
                .HasDefaultValue(false)
                .HasColumnName("defaultTax");
            entity.Property(e => e.Deletedat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deletedat");
            entity.Property(e => e.Deletedbyid).HasColumnName("deletedbyid");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Editedat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("editedat");
            entity.Property(e => e.Editedbyid).HasColumnName("editedbyid");
            entity.Property(e => e.Favourite).HasColumnName("favourite");
            entity.Property(e => e.Imageid)
                .HasMaxLength(500)
                .HasColumnName("imageid");
            entity.Property(e => e.Isavailabe)
                .HasDefaultValue(false)
                .HasColumnName("isavailabe");
            entity.Property(e => e.Isdeleted)
                .HasDefaultValue(false)
                .HasColumnName("isdeleted");
            entity.Property(e => e.Itemname)
                .HasMaxLength(100)
                .HasColumnName("itemname");
            entity.Property(e => e.Itemtype).HasColumnName("itemtype");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.Rate).HasColumnName("rate");
            entity.Property(e => e.Shortcode).HasColumnName("shortcode");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Taxpercentage).HasColumnName("taxpercentage");
            entity.Property(e => e.Units).HasColumnName("units");

            entity.HasOne(d => d.Category).WithMany(p => p.Items)
                .HasForeignKey(d => d.Categoryid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("item_categoryid_fkey");
        });

        modelBuilder.Entity<ItemModifiergroupMapping>(entity =>
        {
            entity.HasKey(e => e.Itemmodifierid).HasName("item_modifiergroup_mapping_pkey");

            entity.ToTable("item_modifiergroup_mapping");

            entity.Property(e => e.Itemmodifierid).HasColumnName("itemmodifierid");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Createdbyid)
                .HasDefaultValue(0)
                .HasColumnName("createdbyid");
            entity.Property(e => e.Deletedat)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deletedat");
            entity.Property(e => e.Deletedbyid)
                .HasDefaultValue(0)
                .HasColumnName("deletedbyid");
            entity.Property(e => e.Editedat)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("editedat");
            entity.Property(e => e.Editedbyid)
                .HasDefaultValue(0)
                .HasColumnName("editedbyid");
            entity.Property(e => e.Isdeleted)
                .HasDefaultValue(false)
                .HasColumnName("isdeleted");
            entity.Property(e => e.Itemid).HasColumnName("itemid");
            entity.Property(e => e.Maxvalue)
                .HasDefaultValue(0)
                .HasColumnName("maxvalue");
            entity.Property(e => e.Minvalue)
                .HasDefaultValue(0)
                .HasColumnName("minvalue");
            entity.Property(e => e.Modifiergroupid).HasColumnName("modifiergroupid");

            entity.HasOne(d => d.Item).WithMany(p => p.ItemModifiergroupMappings)
                .HasForeignKey(d => d.Itemid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("item_modifiergroup_mapping_itemid_fkey");

            entity.HasOne(d => d.Modifiergroup).WithMany(p => p.ItemModifiergroupMappings)
                .HasForeignKey(d => d.Modifiergroupid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("item_modifiergroup_mapping_modifiergroupid_fkey");
        });

        modelBuilder.Entity<Modfierandgroupsmapping>(entity =>
        {
            entity.HasKey(e => e.Modfierandgroupsmappingid).HasName("modfierandgroupsmapping_pkey");

            entity.ToTable("modfierandgroupsmapping");

            entity.Property(e => e.Modfierandgroupsmappingid).HasColumnName("modfierandgroupsmappingid");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Createdbyid).HasColumnName("createdbyid");
            entity.Property(e => e.Deletedat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deletedat");
            entity.Property(e => e.Deletedbyid).HasColumnName("deletedbyid");
            entity.Property(e => e.Editedat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("editedat");
            entity.Property(e => e.Editedbyid).HasColumnName("editedbyid");
            entity.Property(e => e.Isdelete)
                .HasDefaultValue(false)
                .HasColumnName("isdelete");
            entity.Property(e => e.Modifiergroupid).HasColumnName("modifiergroupid");
            entity.Property(e => e.Modifierid).HasColumnName("modifierid");

            entity.HasOne(d => d.Modifiergroup).WithMany(p => p.Modfierandgroupsmappings)
                .HasForeignKey(d => d.Modifiergroupid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("modfierandgroupsmapping_modifiergroupid_fkey");

            entity.HasOne(d => d.Modifier).WithMany(p => p.Modfierandgroupsmappings)
                .HasForeignKey(d => d.Modifierid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("modfierandgroupsmapping_modifierid_fkey");
        });

        modelBuilder.Entity<Modifier>(entity =>
        {
            entity.HasKey(e => e.Modifierid).HasName("modifiers_pkey");

            entity.ToTable("modifiers");

            entity.Property(e => e.Modifierid).HasColumnName("modifierid");
            entity.Property(e => e.Createdat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Createdbyid).HasColumnName("createdbyid");
            entity.Property(e => e.Deletedat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deletedat");
            entity.Property(e => e.Deletedbyid).HasColumnName("deletedbyid");
            entity.Property(e => e.Editedat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("editedat");
            entity.Property(e => e.Editedbyid).HasColumnName("editedbyid");
            entity.Property(e => e.Isdeleted)
                .HasDefaultValue(false)
                .HasColumnName("isdeleted");
            entity.Property(e => e.Modifierdescription)
                .HasMaxLength(500)
                .HasColumnName("modifierdescription");
            entity.Property(e => e.Modifiername)
                .HasMaxLength(50)
                .HasColumnName("modifiername");
            entity.Property(e => e.Modifierquantity).HasColumnName("modifierquantity");
            entity.Property(e => e.Modifierrate).HasColumnName("modifierrate");
            entity.Property(e => e.Modifierunit).HasColumnName("modifierunit");
            entity.Property(e => e.Taxdefault)
                .HasDefaultValue(false)
                .HasColumnName("taxdefault");
            entity.Property(e => e.Taxpercentage).HasColumnName("taxpercentage");
        });

        modelBuilder.Entity<Modifiergroup>(entity =>
        {
            entity.HasKey(e => e.Modifiergroupid).HasName("modifiergroup_pkey");

            entity.ToTable("modifiergroup");

            entity.Property(e => e.Modifiergroupid).HasColumnName("modifiergroupid");
            entity.Property(e => e.Createdat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Createdbyid).HasColumnName("createdbyid");
            entity.Property(e => e.Deletedat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deletedat");
            entity.Property(e => e.Deletedbyid).HasColumnName("deletedbyid");
            entity.Property(e => e.Editedat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("editedat");
            entity.Property(e => e.Editedbyid).HasColumnName("editedbyid");
            entity.Property(e => e.Isdeleted)
                .HasDefaultValue(false)
                .HasColumnName("isdeleted");
            entity.Property(e => e.Modifiergroupdescription)
                .HasMaxLength(50)
                .HasColumnName("modifiergroupdescription");
            entity.Property(e => e.Modifiergroupname)
                .HasMaxLength(50)
                .HasColumnName("modifiergroupname");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.Permissionid).HasName("permissions_pkey");

            entity.ToTable("permissions");

            entity.Property(e => e.Permissionid).HasColumnName("permissionid");
            entity.Property(e => e.Permissionname)
                .HasMaxLength(50)
                .HasColumnName("permissionname");
        });

        modelBuilder.Entity<PermissionsRole>(entity =>
        {
            entity.HasKey(e => e.Permissionroleid).HasName("permissions_role_pkey");

            entity.ToTable("permissions_role");

            entity.Property(e => e.Permissionroleid).HasColumnName("permissionroleid");
            entity.Property(e => e.Candelete).HasColumnName("candelete");
            entity.Property(e => e.Canedit).HasColumnName("canedit");
            entity.Property(e => e.Canview).HasColumnName("canview");
            entity.Property(e => e.Createdat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Createdbyid).HasColumnName("createdbyid");
            entity.Property(e => e.Deletedat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deletedat");
            entity.Property(e => e.Deletedbyid).HasColumnName("deletedbyid");
            entity.Property(e => e.Editedat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("editedat");
            entity.Property(e => e.Editedbyid).HasColumnName("editedbyid");
            entity.Property(e => e.Isdeleted).HasColumnName("isdeleted");
            entity.Property(e => e.Permissionid).HasColumnName("permissionid");
            entity.Property(e => e.Roleid).HasColumnName("roleid");

            entity.HasOne(d => d.Permission).WithMany(p => p.PermissionsRoles)
                .HasForeignKey(d => d.Permissionid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("permissions_role_permissionid_fkey");

            entity.HasOne(d => d.Role).WithMany(p => p.PermissionsRoles)
                .HasForeignKey(d => d.Roleid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("permissions_role_roleid_fkey");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Roleid).HasName("role_pkey");

            entity.ToTable("role");

            entity.Property(e => e.Roleid).HasColumnName("roleid");
            entity.Property(e => e.Rolename)
                .HasMaxLength(50)
                .HasColumnName("rolename");
        });

        modelBuilder.Entity<Section>(entity =>
        {
            entity.HasKey(e => e.Sectionid).HasName("sections_pkey");

            entity.ToTable("sections");

            entity.Property(e => e.Sectionid).HasColumnName("sectionid");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Createdbyid)
                .HasDefaultValue(0)
                .HasColumnName("createdbyid");
            entity.Property(e => e.Deletedat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deletedat");
            entity.Property(e => e.Deletedbyid)
                .HasDefaultValue(0)
                .HasColumnName("deletedbyid");
            entity.Property(e => e.Editedat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("editedat");
            entity.Property(e => e.Editedbyid)
                .HasDefaultValue(0)
                .HasColumnName("editedbyid");
            entity.Property(e => e.Isdeleted)
                .HasDefaultValue(false)
                .HasColumnName("isdeleted");
            entity.Property(e => e.Sectiondescription)
                .HasMaxLength(50)
                .HasColumnName("sectiondescription");
            entity.Property(e => e.Sectionname)
                .HasMaxLength(50)
                .HasColumnName("sectionname");
        });

        modelBuilder.Entity<State>(entity =>
        {
            entity.HasKey(e => e.Stateid).HasName("state_pkey");

            entity.ToTable("state");

            entity.Property(e => e.Stateid).HasColumnName("stateid");
            entity.Property(e => e.Countryid).HasColumnName("countryid");
            entity.Property(e => e.Statename)
                .HasMaxLength(50)
                .HasColumnName("statename");

            entity.HasOne(d => d.Country).WithMany(p => p.States)
                .HasForeignKey(d => d.Countryid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("state_countryid_fkey");
        });

        modelBuilder.Entity<Table>(entity =>
        {
            entity.HasKey(e => e.Tableid).HasName("tables_pkey");

            entity.ToTable("tables");

            entity.Property(e => e.Tableid).HasColumnName("tableid");
            entity.Property(e => e.Capacity).HasColumnName("capacity");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Createdbyid)
                .HasDefaultValue(0)
                .HasColumnName("createdbyid");
            entity.Property(e => e.Deletedat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deletedat");
            entity.Property(e => e.Deletedbyid)
                .HasDefaultValue(0)
                .HasColumnName("deletedbyid");
            entity.Property(e => e.Editedat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("editedat");
            entity.Property(e => e.Editedbyid)
                .HasDefaultValue(0)
                .HasColumnName("editedbyid");
            entity.Property(e => e.Isdeleted).HasColumnName("isdeleted");
            entity.Property(e => e.Sectionid).HasColumnName("sectionid");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Tablename)
                .HasMaxLength(50)
                .HasColumnName("tablename");

            entity.HasOne(d => d.Section).WithMany(p => p.Tables)
                .HasForeignKey(d => d.Sectionid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tables_sectionid_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Userid).HasName("users_pkey");

            entity.ToTable("users");

            entity.Property(e => e.Userid).HasColumnName("userid");
            entity.Property(e => e.Accountid).HasColumnName("accountid");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.Cityid).HasColumnName("cityid");
            entity.Property(e => e.Countryid).HasColumnName("countryid");
            entity.Property(e => e.Createdat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Createdbyid).HasColumnName("createdbyid");
            entity.Property(e => e.Deletedat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deletedat");
            entity.Property(e => e.Deletedbyid).HasColumnName("deletedbyid");
            entity.Property(e => e.Editedat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("editedat");
            entity.Property(e => e.Firstname)
                .HasMaxLength(50)
                .HasColumnName("firstname");
            entity.Property(e => e.Isactive).HasColumnName("isactive");
            entity.Property(e => e.Isdeleted)
                .HasDefaultValue(false)
                .HasColumnName("isdeleted");
            entity.Property(e => e.Lastname)
                .HasMaxLength(50)
                .HasColumnName("lastname");
            entity.Property(e => e.Phone)
                .HasPrecision(50)
                .HasColumnName("phone");
            entity.Property(e => e.Roleid).HasColumnName("roleid");
            entity.Property(e => e.Stateid).HasColumnName("stateid");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Userimage)
                .HasMaxLength(50)
                .HasColumnName("userimage");
            entity.Property(e => e.Zipcode)
                .HasPrecision(20)
                .HasColumnName("zipcode");

            entity.HasOne(d => d.Account).WithMany(p => p.Users)
                .HasForeignKey(d => d.Accountid)
                .HasConstraintName("fk_accountid");

            entity.HasOne(d => d.City).WithMany(p => p.Users)
                .HasForeignKey(d => d.Cityid)
                .HasConstraintName("fk_cityid");

            entity.HasOne(d => d.Country).WithMany(p => p.Users)
                .HasForeignKey(d => d.Countryid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("users_countryid_fkey");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.Roleid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("users_roleid_fkey");

            entity.HasOne(d => d.State).WithMany(p => p.Users)
                .HasForeignKey(d => d.Stateid)
                .HasConstraintName("fk_stateid");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
