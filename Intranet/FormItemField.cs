//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Intranet
{
    using System;
    using System.Collections.Generic;
    
    public partial class FormItemField
    {
        public int id_field { get; set; }
        public int id_item { get; set; }
        public int id_form { get; set; }
        public int id_fcontrol { get; set; }
        public byte datetype { get; set; }
        public string strvalue { get; set; }
        public int intvalue { get; set; }
        public decimal numvalue { get; set; }
        public decimal moneyvalue { get; set; }
        public System.DateTime datevalue { get; set; }
        public string richvalue { get; set; }
    
        public virtual FormItem FormItem { get; set; }
    }
}
