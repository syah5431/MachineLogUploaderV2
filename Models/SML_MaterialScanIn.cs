//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MachineLogUploader.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class SML_MaterialScanIn
    {
        public int ID { get; set; }
        public string LotNo { get; set; }
        public int Sequence { get; set; }
        public string Catalogue { get; set; }
        public int Quantity { get; set; }
        public int BalanceQty { get; set; }
        public Nullable<System.DateTime> Expired { get; set; }
        public System.DateTime InDate { get; set; }
        public string InBy { get; set; }
        public string Remark { get; set; }
        public Nullable<int> CategoryId { get; set; }
    }
}
