using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WcfServiceLibrary.DataModels
{
    public class FileRow
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public string Text { get; set; }
    }

    public class EfConf_FileRow : EntityTypeConfiguration<FileRow>
    {
        public EfConf_FileRow()
        {
            ToTable("dbo.FileRows");
            HasKey(x => x.Id);
            Property(x => x.Id).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
        }
    }
}
