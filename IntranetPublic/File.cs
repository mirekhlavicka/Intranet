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
    
    public partial class File
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public File()
        {
            this.Downloads = new HashSet<Download>();
        }
    
        public int id_file { get; set; }
        public int id_folder { get; set; }
        public int id_user { get; set; }
        public string name { get; set; }
        public string filename { get; set; }
        public int filesize { get; set; }
        public string fileext { get; set; }
        public bool image { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public System.DateTime upload_date { get; set; }
    
        public virtual User User { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Download> Downloads { get; set; }
    }
}