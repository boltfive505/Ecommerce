namespace purchase_request.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("purchase")]
    public partial class purchase
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string ItemNumber { get; set; }

        public int PurchaseQuantity { get; set; }

        public int ReceivedQuantity { get; set; }

        public int? PackingId { get; set; }

        public int? LuckyId { get; set; }

        public int? QuantityPerBox { get; set; }

        [StringLength(255)]
        public string BoxLabel { get; set; }

        public decimal? UnitPrice { get; set; }

        public decimal? UnitWeight { get; set; }

        public long IsShippedDirect { get; set; }

        public long? RequestDate { get; set; }

        public long? ApprovedDate { get; set; }

        public long? ReceivedDate { get; set; }

        public long? ShippedDate { get; set; }

        public long? DeliveryDate { get; set; }

        public long? ArrivalDate { get; set; }

        [StringLength(255)]
        public string PurchaseNote { get; set; }

        [StringLength(255)]
        public string PreparationNote { get; set; }

        [StringLength(255)]
        public string IncomingNote { get; set; }

        [StringLength(255)]
        public string IssueNote { get; set; }

        [StringLength(255)]
        public string ShippingNote { get; set; }

        [StringLength(255)]
        public string DeliveryNote { get; set; }

        [StringLength(255)]
        public string ArrivalNote { get; set; }

        public virtual packing packing { get; set; }

        public virtual lucky lucky { get; set; }
    }
}
