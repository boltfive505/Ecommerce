namespace purchase_request.Model
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("lucky")]
    public partial class lucky
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public lucky()
        {
            purchases = new ObservableCollection<purchase>();
        }

        public int Id { get; set; }

        public string Key { get; set; }

        [StringLength(255)]
        public string LuckyDetailsKey { get; set; }

        [Required]
        [StringLength(255)]
        public string ShippingLabel { get; set; }

        public long? ArriveKorea { get; set; }

        public string Location { get; set; }

        public long? ProcessDate { get; set; }

        [StringLength(255)]
        public string ItemName { get; set; }

        public int? Quantity { get; set; }

        public decimal? Weight { get; set; }

        [StringLength(255)]
        public string CBM { get; set; }

        public string Memo1 { get; set; }

        [StringLength(255)]
        public string Memo2 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ObservableCollection<purchase> purchases { get; set; }

        public virtual lucky_details lucky_details { get; set; }

        public override int GetHashCode()
        {
            return Key.GetHashCode() ^ ShippingLabel.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is lucky)
            {
                var o = obj as lucky;
                return this.Key == o.Key &&
                    this.ShippingLabel == o.ShippingLabel &&
                    this.ItemName == o.ItemName &&
                    this.Quantity == o.Quantity &&
                    this.Weight == o.Weight &&
                    this.Memo1 == o.Memo1;
            }
            return false;
        }
    }
}
