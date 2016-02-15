﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class PubSystemEntities : DbContext
    {
        public PubSystemEntities()
            : base("name=PubSystemEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<FormItemField> FormItemFields { get; set; }
        public virtual DbSet<File> Files { get; set; }
        public virtual DbSet<Folder> Folders { get; set; }
        public virtual DbSet<UserPage> UserPages { get; set; }
        public virtual DbSet<Download> Downloads { get; set; }
        public virtual DbSet<FormItem> FormItems { get; set; }
        public virtual DbSet<Cours> Courses { get; set; }
        public virtual DbSet<Article> Articles { get; set; }
        public virtual DbSet<ArticleSection> ArticleSections { get; set; }
        public virtual DbSet<Chapter> Chapters { get; set; }
        public virtual DbSet<Section> Sections { get; set; }
    
        public virtual int spIntranetGetSectionRights(Nullable<int> id_user, Nullable<int> id_section, ObjectParameter canEdit, ObjectParameter canEditAll)
        {
            var id_userParameter = id_user.HasValue ?
                new ObjectParameter("id_user", id_user) :
                new ObjectParameter("id_user", typeof(int));
    
            var id_sectionParameter = id_section.HasValue ?
                new ObjectParameter("id_section", id_section) :
                new ObjectParameter("id_section", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("spIntranetGetSectionRights", id_userParameter, id_sectionParameter, canEdit, canEditAll);
        }
    
        public virtual ObjectResult<Nullable<int>> spIntranetGetUserSections(Nullable<int> id_user)
        {
            var id_userParameter = id_user.HasValue ?
                new ObjectParameter("id_user", id_user) :
                new ObjectParameter("id_user", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("spIntranetGetUserSections", id_userParameter);
        }
    }
}
