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
            //db = new MyDbContext();
        }

        /// <summary>
        /// Возвращает результат поиска в виде объекта для передачи
        /// </summary>
        /// <param name="query">Строка поиска</param>
        /// <param name="page">Необходимая страница</param>
        /// <returns></returns>
        public SearchResultDto SearchRow(string query, int page)
        {
            if (page > 0)
            {
                SearchResultDto result;

                using (db = new MyDbContext())
                {
                    var records = GetFileRowsNoTrackingQuery().Where(x => x.Text.Contains(query));

                    var recordsCount = records.Count();

                    var pageSize = 20;
                    var recordsToSkip = pageSize * (page - 1);

                    var searchResult = records.OrderBy(x => x.Text)
                        .Skip(recordsToSkip)
                        .Take(pageSize)
                        .OrderBy(x => x.Number)
                        .Select(x => new FileRowModel
                        {
                            RowNumber = x.Number,
                            RowText = x.Text
                        })
                        .ToList();

                    result = new SearchResultDto
                    {
                        ResultsOnPage = searchResult,
                        ResultsCount = recordsCount,
                        CurrentPage = page,
                        PagesCount = (int)Math.Ceiling((double)recordsCount / pageSize)
                    };                   
                }

                return result;
            }
            else
            {
                throw new ArgumentOutOfRangeException("page");
            }
        }

        /// <summary>
        /// Возвращает результат, получилось ли сохранить файл в бд. 
        /// </summary>
        /// <param name="file">Поток для сохранения в бд</param>
        /// <returns></returns>
        public bool SaveFile(Stream file)
        {
            if (file != null)
            {
                var fileLines = GetFileLines(file);

                if (fileLines.Any())
                {
                    var recordsForSave = GetFileRowsForSave(fileLines);

                    if (recordsForSave.Any())
                    {

                        //try
                        //{     

                        //строки не сохраняются
                        //INSERT[dbo].[FileRows]([Number], [Text])
                        //VALUES(@0, @1)
                        //SELECT[Id]
                        //FROM[dbo].[FileRows]
                        //WHERE @@ROWCOUNT > 0 AND[Id] = scope_identity()

                        //using (var transaction = db.Database.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted))
                        //{
                        using (var scope = new System.Transactions.TransactionScope())
                        {
                            db = null;
                            try
                            {
                                db = new MyDbContext();
                                db.Configuration.AutoDetectChangesEnabled = false;

                                int count = 0;
                                foreach (var rec in recordsForSave)
                                {
                                    ++count;
                                    db = AddToContext(db, rec, count, 100);
                                }

                                db.SaveChanges();
                            }
                            finally
                            {
                                if (db != null)
                                    db.Dispose();
                            }

                            scope.Complete();
                        }

                        //db.FileRows.AddRange(recordsForSave);

                        //db.SaveChanges();

                        // transaction.Commit();
                        //}


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

        private MyDbContext AddToContext(MyDbContext context, FileRow entity, int count, int commitCount)
        {
            context.FileRows.Add(entity);

            if (count % commitCount == 0)
            {
                context.SaveChanges();

                context.Dispose();
                context = new MyDbContext();
                context.Configuration.AutoDetectChangesEnabled = false;
            }

            return context;
        }


        /// <summary>
        /// Возвращает коллекцию строк (string) из потока
        /// </summary>
        /// <param name="file">Поток, строки которого необходимо вернуть</param>
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
        /// Возвращает коллекцию объектов таблицы FileRows для сохранения 
        /// </summary>
        /// <param name="fileLines">Строки из файла для сохранения</param>
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
                    catch (ArgumentNullException)
                    {
                        continue;
                    }

                }
            }

            return rowsForSave;
        }

        /// <summary>
        /// Возвращает новый объект таблицы FileRows
        /// </summary>
        /// <param name="number">Номер строки в файле</param>
        /// <param name="text">Текст строки</param>
        /// <returns></returns>
        private FileRow GetNewFileRowRecord(int number, string text)
        {
            if (number > 0 && !String.IsNullOrEmpty(text))
                return new FileRow { Number = number, Text = text };
            else
                throw new ArgumentNullException();
        }

        /// <summary>
        /// Возвращает запрос на выборку данных из таблицы FileRows без отслеживания изменений, которой можно достраивать
        /// </summary>
        /// <returns></returns>
        private IQueryable<FileRow> GetFileRowsNoTrackingQuery()
        {
            return db.FileRows.AsNoTracking();
        }
    }
}
