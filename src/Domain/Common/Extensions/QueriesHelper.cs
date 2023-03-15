namespace Domain.Common.Extensions
{
    public class QueriesHelper
    {
        public static string CheckAndDropStoredProcedure(string StoredProcedureName)
        {
            return $"IF EXISTS ( SELECT * FROM sysobjects WHERE  id = object_id(N'[{StoredProcedureName}]')) BEGIN DROP PROCEDURE [{StoredProcedureName}] END";
        }
    }
}
