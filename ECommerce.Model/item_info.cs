namespace ECommerce.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("item_info")]
    public partial class item_info
    {
        public long Id { get; set; }

        [Required]
        [StringLength(2147483647)]
        public string ItemNumber { get; set; }

        [Required]
        [StringLength(2147483647)]
        public string Category { get; set; }

        public long UpdatedDate { get; set; }

        public long? UpdatedByEmployeeId { get; set; }

        [StringLength(2147483647)]
        public string ShortDescription { get; set; }

        [StringLength(2147483647)]
        public string LongDescription { get; set; }

        [StringLength(2147483647)]
        public string CompetitorUrl { get; set; }

        public decimal? CompetitorPrice { get; set; }

        [StringLength(2147483647)]
        public string CompetitorLocation { get; set; }

        public int CompetitorRatings { get; set; }

        public int CompetitorHasStocks { get; set; } = -1;

        public virtual employee UpdatedByEmployee { get; set; }
    }
}
