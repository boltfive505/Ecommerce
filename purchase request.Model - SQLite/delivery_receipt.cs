namespace purchase_request.Model
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class delivery_receipt
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public delivery_receipt()
        {
            packages = new ObservableCollection<packing>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string ReceiptNumber { get; set; }

        public long? ReceiptDate { get; set; }

        public decimal? Amount { get; set; }

        public decimal? AdditionalAmount { get; set; }

        public decimal? Weight { get; set; }

        public int? Quantity { get; set; }

        [StringLength(255)]
        public string FileAttachment { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ObservableCollection<packing> packages { get; set; }
    }
}
