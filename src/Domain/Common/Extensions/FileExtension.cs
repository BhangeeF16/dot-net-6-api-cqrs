namespace Domain.Common.Extensions
{
    public class FileExtensions
    {
        public static string GetExcelFileTemplate(string TemplateName)
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $@"Common\Templates\ExcelTemplates\{TemplateName}.xlsx");
        }
        public static string GetStoredProcedureQuery(string StoredProcedureName)
        {
            var sqlFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $@"StoredProcedures\{StoredProcedureName}.sql");
            return File.ReadAllText(sqlFile);
        }
    }
}
