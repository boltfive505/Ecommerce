namespace purchase_request.Model
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public class lucky_details
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public lucky_details()
        {
            luckyItems = new ObservableCollection<lucky>();
        }

        [Key]
        [StringLength(255)]
        public string ShippingLabel { get; set; }

        [StringLength(255)]
        public string Followup { get; set; }

        [StringLength(255)]
        public string Notes { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ObservableCollection<lucky> luckyItems { get; set; }
    }
}
