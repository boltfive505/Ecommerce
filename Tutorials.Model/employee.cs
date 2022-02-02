namespace Tutorials.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("employee")]
    public partial class employee
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public employee()
        {
            videos = new HashSet<video>();
            IsActive = 1;
        }

        public long Id { get; set; }

        [Required]
        [StringLength(2147483647)]
        public string EmployeeName { get; set; }

        public string Description { get; set; }

        public long IsActive { get; set; }

        public virtual ICollection<video> videos { get; set; }
    }
}
