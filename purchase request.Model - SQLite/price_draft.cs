namespace purchase_request.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class price_draft
    {
        public long Id { get; set; }

        [Required]
        [StringLength(2147483647)]
        public string ItemNumber { get; set; }

        public long? DraftDate { get; set; }

        public string CompetitorUrl { get; set; }

        public decimal? CompetitorLowestPrice { get; set; }

        [StringLength(2147483647)]
        public string CompetitorLocation { get; set; }

        [StringLength(2147483647)]
        public string CompetitorNotes { get; set; }

        public decimal? SuggestedPrice { get; set; }

        public long? SuggestedDate { get; set; }

        [StringLength(2147483647)]
        public string SuggestedNotes { get; set; }

        public decimal? ApprovedPrice { get; set; }

        public long? ApprovedDate { get; set; }

        [StringLength(2147483647)]
        public string ApprovedNotes { get; set; }
    }
}
