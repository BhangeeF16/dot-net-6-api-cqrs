namespace Domain.Common.Extensions
{
    public class FileExtensions
    {
        public static string GetExcelFileTemplate(string TemplateName, string extension = ".xlsx")
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $@"Common\Templates\ExcelTemplates\{TemplateName}{extension}");
        }
        public static string GetEmailTemplate(string TemplateName, string extension = ".html")
        {
            var emailTemplate = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $@"Common\Templates\EmailTemplates\{TemplateName}{extension}");
            return File.ReadAllText(emailTemplate);
        }
        public static string GetStoredProcedureQuery(string StoredProcedureName)
        {
            var sqlFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $@"StoredProcedures\{StoredProcedureName}.sql");
            return File.ReadAllText(sqlFile);
        }
    }
}
