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
    
    public partial class SML_Machine_Logger
    {
        public int LogId { get; set; }
        public System.DateTime Date { get; set; }
        public System.DateTime OperationDateTime { get; set; }
        public string OperationCode { get; set; }
        public string Operation { get; set; }
        public string Machine { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime Changedate { get; set; }
        public string ChangedBy { get; set; }
        public string Remark { get; set; }
    }
}