using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WcfServiceLibrary.Contracts;
using WcfServiceLibrary.DataModels;

namespace WcfServiceLibrary.Repositories
{
    
    public class FileDatabaseRepository : IFileDatabaseRepository
    {
        private MyDbContext db;

        public FileDatabaseRepository()
        {
            db = new MyDbContext();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public SearchResultDto SearchRow(string query, int page)
        {
            if (page > 0)
            {
                var records = GetFileRowsNoTrackingQuery().Where(x => x.Text.Contains(query));

                var recordsCount = records.Count();

                var pageSize = 20;
                var recordsToSkip = pageSize * (page - 1);

                var searchResult = records.OrderBy(x => x.Text)
                    .Skip(recordsToSkip)
                    .Take(pageSize)
                    .Select(x => new FileRowModel
                    {
                        RowNumber = x.Number,
                        RowText = x.Text
                    });

                SearchResultDto result = new SearchResultDto
                {
                    ResultsOnPage = searchResult,
                    ResultsCount = recordsCount,
                    CorrentPage = page,
                    PagesCount = (int)Math.Ceiling((double)recordsCount / pageSize)
                };

                return result;
            }
            else
            {
                throw new ArgumentOutOfRangeException("page", "Номер страницы должен быть больше нуля");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public bool SaveFile(Stream file)
        {

            if (file != null)
            {
                var fileLines = GetFileLines(file);

                if(fileLines.Any())
                {
                    var recordsForSave = GetFileRowsForSave(fileLines);

                    if(recordsForSave.Any())
                    {
                        //try
                        //{                      

                        //Deadlock
                        //INSERT[dbo].[FileRows]([Number], [Text])
                        //VALUES(@0, @1)
                        //SELECT[Id]
                        //FROM[dbo].[FileRows]
                        //WHERE @@ROWCOUNT > 0 AND[Id] = scope_identity()

                        db.FileRows.AddRange(recordsForSave);
                        db.SaveChanges();

                        //}
                        //catch
                        //{
                        //    return false;
                        //}

                        return true;                     
                    }
                }               
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private IList<string> GetFileLines(Stream file)
        {
            var fileLines = new List<string>();

            if (file != null)
            {
                file.Position = 0;

                using (StreamReader sr = new StreamReader(file))
                {
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();

                        if (!String.IsNullOrEmpty(line))
                        {
                            fileLines.AddRange(line.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries));
                        }
                    }

                    sr.Close();
                }

                file.Close();
            }

            return fileLines;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileLines"></param>
        /// <returns></returns>
        private IEnumerable<FileRow> GetFileRowsForSave(IList<string> fileLines)
        {
            var rowsForSave = new List<FileRow>(); 

            if (fileLines != null)
            {
                for (int i = 0; i < fileLines.Count; i++)
                {
                    try
                    {
                        rowsForSave.Add(GetNewFileRowRecord(i + 1, fileLines[i]));
                    }
                    catch(ArgumentNullException)
                    {
                        continue;
                    }
                    
                }
            }

            return rowsForSave;            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="number"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        private FileRow GetNewFileRowRecord(int number, string text)
        {
            if (number > 0 && !String.IsNullOrEmpty(text))
                return new FileRow { Number = number, Text = text };
            else
                throw new ArgumentNullException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private IQueryable<FileRow> GetFileRowsNoTrackingQuery()
        {
            return db.FileRows.AsNoTracking();
        }
    }
}
