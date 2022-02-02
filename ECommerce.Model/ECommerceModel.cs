using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace ECommerce.Model
{
    [DbConfigurationType(typeof(EF6.ConfigurationTypes.BasicDbConfiguration))]
    public partial class ECommerceModel : DbContext
    {
        public ECommerceModel()
            : base("name=ECommerceModel")
        {
        }

        public virtual DbSet<item_info> item_info { get; set; }
        public virtual DbSet<employee> employee { get; set; }
        public virtual DbSet<item_details> item_details { get; set; }
        public virtual DbSet<item_keyword> item_keyword { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<employee>()
                .HasMany(e => e.item_infos)
                .WithOptional(e => e.UpdatedByEmployee)
                .HasForeignKey(e => e.UpdatedByEmployeeId)
                .WillCascadeOnDelete(false);
        }

        public void Initialize()
        {
            Database.SetInitializer<ECommerceModel>(null);
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
            Configuration.AutoDetectChangesEnabled = false;
            Database.Initialize(false);
            this.item_info.Load();
            Configuration.AutoDetectChangesEnabled = true;
        }
    }
}
