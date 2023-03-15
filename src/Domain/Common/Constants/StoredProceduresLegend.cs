namespace Domain.Common.Constants
{
    public static class DayLegend
    {
        public const int Monday = 1;
        public const int Tuesday = 2;
        public const int Wednesday = 3;
        public const int Thursday = 4;
        public const int Friday = 5;
        public const int Saturday = 6;
        public const int Sunday = 7;
    }

    public static class StoredProceduresLegend
    {
        public const string GetUserPantryItems = "fp_GetUserPantryItems";
        public const string GetUserUnSubscribedPantryItems = "fp_GetUserUnSubscribedPantryItems";

        public const string GetPantryItemCategories = "fp_GetPantryItemCategories";
        public const string GetPantryItems = "fp_GetPantryItems";
        public const string GetPlans = "fp_GetPlans";
        public const string GetPostCodes = "fp_GetPostCodes";
        public const string GetPostCodeSchedules = "fp_GetPostCodeSchedules";
    }
    public static class ExcelFileLegend
    {
        public const string ExportPostCodeSchedules = "ExportPostCodeSchedules";
    }
}
