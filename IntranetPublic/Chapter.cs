//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace IntranetPublic
{
    using System;
    using System.Collections.Generic;
    
    public partial class Chapter
    {
        public int id_chapter { get; set; }
        public int id_article { get; set; }
        public string name { get; set; }
        public string body { get; set; }
        public int order { get; set; }
        public byte[] tmstamp { get; set; }
        public bool del { get; set; }
    
        public virtual Article Article { get; set; }
    }
}
