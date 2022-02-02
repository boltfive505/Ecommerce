namespace purchase_request.Model
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.ModelConfiguration.Conventions;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    [DbConfigurationType(typeof(EF6.ConfigurationTypes.BasicDbConfiguration))]
    public partial class PurchaseRequestModel : DbContext
    {
        public PurchaseRequestModel()
            : base("name=purchaseRequestDb")
        {
        }

        public virtual DbSet<price_draft> price_draft { get; set; }
        public virtual DbSet<delivery_receipt> delivery_receipt { get; set; }
        public virtual DbSet<lucky> lucky { get; set; }
        public virtual DbSet<lucky_details> lucky_details { get; set; }
        public virtual DbSet<noninventory_items> noninventory_items { get; set; }
        public virtual DbSet<purchase> purchase { get; set; }
        public virtual DbSet<packing> packing { get; set; }
        public virtual DbSet<item_details> item_details { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<DecimalPropertyConvention>();
            modelBuilder.Conventions.Add(new DecimalPropertyConvention(10, 2));

            modelBuilder.Entity<packing>()
                .HasMany(e => e.purchases)
                .WithOptional(e => e.packing)
                .HasForeignKey(e => e.PackingId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<delivery_receipt>()
                .HasMany(e => e.packages)
                .WithOptional(e => e.delivery_receipt)
                .HasForeignKey(e => e.DeliveryReceiptId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<purchase>()
                .HasOptional(e => e.lucky)
                .WithMany(e => e.purchases)
                .HasForeignKey(e => e.LuckyId);

            modelBuilder.Entity<lucky_details>()
                .HasMany(e => e.luckyItems)
                .WithOptional(e => e.lucky_details)
                .HasForeignKey(e => e.LuckyDetailsKey)
                .WillCascadeOnDelete(false);
        }

        public void Initialize()
        {
            Database.SetInitializer<PurchaseRequestModel>(null);
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
            Configuration.AutoDetectChangesEnabled = false;
            Database.Initialize(false);
            this.purchase.Load();
            Configuration.AutoDetectChangesEnabled = true;
        }
    }
}
