namespace purchase_request.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class noninventory_items
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string ItemNumber { get; set; }

        [StringLength(255)]
        public string ItemName { get; set; }

        [StringLength(255)]
        public string ItemDescription { get; set; }

        [StringLength(255)]
        public string PartsNumber { get; set; }

        [StringLength(255)]
        public string Url { get; set; }

        [StringLength(255)]
        public string ProductDetails { get; set; }

        [StringLength(255)]
        public string KoreanName { get; set; }

        public decimal? TargetPrice { get; set; }

        [StringLength(255)]
        public string CustomerName { get; set; }

        [StringLength(255)]
        public string CustomerContact { get; set; }

        [StringLength(255)]
        public string InquiryMemo { get; set; }

        [StringLength(255)]
        public string ReplyMemo { get; set; }

        public bool Active { get; set; } = true;
    }
}
