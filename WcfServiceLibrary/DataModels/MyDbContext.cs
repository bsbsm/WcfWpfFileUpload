using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WcfServiceLibrary.DataModels
{
    public class MyDbContext : DbContext
    {
        public MyDbContext() : base ("MyDatabase")
        {
            Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations
                .Add(new EfConf_FileRow());
        }

        public virtual DbSet<FileRow> FileRows { get; set; }
    }
}
