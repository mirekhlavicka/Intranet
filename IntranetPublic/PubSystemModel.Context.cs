﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
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
    
        public virtual DbSet<FormItemField> FormItemFields { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserPage> UserPages { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<StaffItem> Staff { get; set; }
        public virtual DbSet<FormItem> FormItems { get; set; }
        public virtual DbSet<File> Files { get; set; }
        public virtual DbSet<Cours> Courses { get; set; }
        public virtual DbSet<Download> Downloads { get; set; }
        public virtual DbSet<Article> Articles { get; set; }
        public virtual DbSet<ArticleSection> ArticleSections { get; set; }
        public virtual DbSet<Chapter> Chapters { get; set; }
        public virtual DbSet<Section> Sections { get; set; }
    }
}
