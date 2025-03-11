using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DanteAPI.Entities.References
{
    public class BookingItem
    {
        public int Item_Type { get; set; }
        public int Item_ID { get; set; }
        public Nullable<int> Item_ID2 { get; set; }
        public decimal Item_Price { get; set; }
        public decimal Item_Total { get; set; }
        public decimal Item_Tax { get; set; }
        public decimal Item_Total_Tax { get; set; }
        public decimal Item_Total_With_Tax { get; set; }
        public int Item_Quantity { get; set; }
        public string Item_Tax_Code { get; set; }
        public decimal? Item_Tax_Rate { get; set; }
        public string Item_Nominal_Code { get; set; }
        public string Item_Reference { get; set; }
        public string Item_Description { get; set; }

    }
}
