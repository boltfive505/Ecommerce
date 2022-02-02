namespace ECommerce.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("employee")]
    public partial class employee
    {
        public employee()
        {
            item_infos = new HashSet<item_info>();
        }

        public long Id { get; set; }

        [Required]
        public string EmployeeName { get; set; }

        public long IsActive { get; set; } = 1;

        public virtual ICollection<item_info> item_infos { get; set; }
    }
}
