using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Data.SQLite;

namespace Tutorials.Model
{
    [DbConfigurationType(typeof(EF6.ConfigurationTypes.BasicDbConfiguration))]
    public partial class TutorialsModel : DbContext
    {
        public TutorialsModel()
            : base("name=tutorialDb")
        {
        }

        public virtual DbSet<video> video { get; set; }
        public virtual DbSet<category> category { get; set; }
        public virtual DbSet<employee> employee { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<video>()
                .HasRequired(e => e.category)
                .WithMany(e => e.videos)
                .HasForeignKey(e => e.CategoryId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<category>()
                .HasMany(e => e.subCategories)
                .WithOptional(e => e.parentCategory)
                .HasForeignKey(e => e.ParentCategoryId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<employee>()
                .HasMany(e => e.videos)
                .WithOptional(e => e.employeeAssignedTo)
                .HasForeignKey(e => e.EmployeeAssignedToId)
                .WillCascadeOnDelete(false);
        }

        public void Initialize()
        {
            Database.SetInitializer<TutorialsModel>(null);
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
            Configuration.AutoDetectChangesEnabled = false;
            Database.Initialize(false);
            this.category.Load();
            Configuration.AutoDetectChangesEnabled = true;
        }
    }
}
