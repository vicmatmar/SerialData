//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SerialData
{
    using System;
    using System.Collections.Generic;
    
    public partial class SerialNumber
    {
        public int SerialNumberId { get; set; }
        public int SerialNumber1 { get; set; }
        public int ProductId { get; set; }
        public int EuiId { get; set; }
        public System.DateTime CreateDate { get; set; }
        public int TesterId { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
    
        public virtual EuiList EuiList { get; set; }
        public virtual Product Product { get; set; }
    }
}
