using System;
using System.Collections.Generic;
namespace DanteAPI.Entities {
    public class CartItem {
        public int ID { get; set; }
        public int CartID { get; set; }
        public Cart Cart { get; set; }
        public short Type { get; set; }
        public int Reference { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int? TaxCodeID { get; set; }
        public References.TaxCode TaxCode { get; set; }
        public int? DelegateID { get; set; }
        public Delegate Delegate { get; set; }
        public int? ParentCartItemID { get; set; }
        public string ReferenceText { get; set; }
        public string Name { get; set; }
        public ICollection<CartItem> ChildCartItems { get; set; }
        public decimal UnitTotalPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal TotalPriceWithTax { get; set; }
        public decimal Tax { get; set; }
        public decimal UnitTotalTax { get; set; }
        public decimal TotalTax { get; set; }
        public string CustomField { get; set; }
    }
}
