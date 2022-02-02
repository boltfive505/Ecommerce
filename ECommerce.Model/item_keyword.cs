namespace ECommerce.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("item_keyword")]
    public partial class item_keyword
    {
        public long Id { get; set; }

        [Required]
        [StringLength(2147483647)]
        public string ItemNumber { get; set; }

        public string Keywords { get; set; }

        public string SuggestedName { get; set; }

        public long? UpdatedDate { get; set; }
    }
}
