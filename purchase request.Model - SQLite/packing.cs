namespace purchase_request.Model
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("packing")]
    public partial class packing
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public packing()
        {
            purchases = new ObservableCollection<purchase>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string PackingNumber { get; set; }

        public long? PackingDate { get; set; }

        [StringLength(255)]
        public string ShipType { get; set; }

        public int TotalBoxQuantity { get; set; }

        public decimal TotalWeight { get; set; }

        public int? DeliveryReceiptId { get; set; }

        public virtual delivery_receipt delivery_receipt { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ObservableCollection<purchase> purchases { get; set; }

        public override int GetHashCode()
        {
            return PackingNumber.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is packing)
            {
                return ((packing)obj).PackingNumber == this.PackingNumber;
            }
            return false;
        }
    }
}
