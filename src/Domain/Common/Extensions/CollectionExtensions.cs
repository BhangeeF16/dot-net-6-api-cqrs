using Domain.Common.Exceptions;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.Reflection;
using System.Text;

namespace Domain.Common.Extensions
{

    public static class CollectionExtensions
    {
        public static List<T> GetListWhere<T>(Func<T, bool> predicate, params T[] entities)
        {
            var list = new List<T>();
            foreach (var item in entities)
            {
                if (predicate.Invoke(item))
                {
                    list.Add(item);
                }
            }
            return list;
        }
        public static IEnumerable<TResult> SelectWhere<TSource, TResult>(this IEnumerable<TSource> enumerable, Func<TSource, bool> predicate, Func<TSource, TResult> selector)
        {
            return enumerable.Where(predicate).Select(selector).AsEnumerable();
        }
        public static IEnumerable<TResult> SelectManyWhere<TSource, TResult>(this IEnumerable<TSource> enumerable, Func<TSource, bool> predicate, Func<TSource, IEnumerable<TResult>> selector)
        {
            return enumerable.Where(predicate).SelectMany(selector).AsEnumerable();
        }
        public static List<T> ToBluePrint<T>(this IFormFile excelFile) where T : class
        {
            IExcelDataReader reader;
            var FileStream = excelFile.OpenReadStream();
            if (excelFile.FileName.EndsWith(".xls"))
            {
                reader = ExcelReaderFactory.CreateBinaryReader(FileStream);
            }
            else if (excelFile.FileName.EndsWith(".xlsx"))
            {
                reader = ExcelReaderFactory.CreateOpenXmlReader(FileStream);
            }
            else if (excelFile.FileName.EndsWith(".csv"))
            {
                reader = ExcelReaderFactory.CreateReader(FileStream, new ExcelReaderConfiguration()
                {
                    FallbackEncoding = Encoding.GetEncoding(1252),
                    AutodetectSeparators = new char[] { ',', ';', '\t', '|', '#' }
                });
            }
            else
            {
                throw new ClientException("The file format is not supported.");
            }

            DataSet ds = reader.AsDataSet();
            reader.Close();

            return ds.Tables[0].BindList<T>(true);
        }
        public static List<T> BindList<T>(this DataTable dataTable, bool ValidateJson = false) where T : class
        {
            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                dataTable.Columns[i].ColumnName = string.IsNullOrEmpty(dataTable.Rows[0][i].ToString()) ? $"Colunm{i}" : dataTable.Rows[0][i].ToString();
            }
            dataTable.Rows.RemoveAt(0);

            var serializeString = JsonConvert.SerializeObject(dataTable);
            var result = JsonConvert.DeserializeObject<List<T>>(serializeString);

            if (ValidateJson)
            {
                if (result.ValidateSchema<List<T>>(true))
                {
                    return result;
                }
                else
                {
                    throw new ClientException("The excel file format is not matching with the provided format, please try again with correct format");
                }
            }

            return result;
        }
        public static bool ContainsDuplicate<T>(this IEnumerable<T> enumerable, IEqualityComparer<T> comparer = null)
        {
            HashSet<T> knownElements = new(comparer);
            foreach (T element in enumerable)
            {
                if (!knownElements.Add(element))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
