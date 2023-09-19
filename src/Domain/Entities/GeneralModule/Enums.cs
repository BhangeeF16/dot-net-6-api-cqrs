using System.ComponentModel;

namespace Domain.Entities.GeneralModule;
public enum PermissionLevel
{
    [Description("DO_NOTHING")] None = 1,
    [Description("DO_EVERYTHING")] All = 2,
    [Description("VIEW")] View = 3,
    [Description("EDIT")] Edit = 4,
    [Description("CREATE")] Create = 5,
    [Description("DELETE")] Delete = 6,
    [Description("IMPERSONATE")] Impersonate = 7,
}