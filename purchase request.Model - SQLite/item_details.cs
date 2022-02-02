namespace purchase_request.Model
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("item_details")]
    public class item_details
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ItemNumber { get; set; }

        public int? ReorderQuantity { get; set; }
    }
}
