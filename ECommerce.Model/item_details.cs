namespace ECommerce.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("item_details")]
    public partial class item_details
    {
        public long Id { get; set; }

        [Required]
        [StringLength(2147483647)]
        public string ItemNumber { get; set; }

        public string Memo { get; set; }

        public long? MemoUpdatedDate { get; set; }

        public long ForUpdate { get; set; }
    }
}
