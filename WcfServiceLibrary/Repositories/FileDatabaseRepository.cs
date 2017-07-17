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
        /// Возвращает результат поиска в виде объекта для передачи
        /// </summary>
        /// <param name="query">Строка поиска</param>
        /// <param name="page">Необходимая страница</param>
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
                    CurrentPage = page,
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

                        using (var transaction = db.Database.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted))
                        {

                            db.FileRows.AddRange(recordsForSave);

                            db.SaveChanges();

                            transaction.Commit();
                        }


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
