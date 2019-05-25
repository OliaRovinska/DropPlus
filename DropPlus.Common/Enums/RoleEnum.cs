using System.ComponentModel;

namespace DropPlus.Common.Enums
{
    public enum RoleEnum
    {
        [Description("User")]
        User = 1,
        [Description("Hiring manager")]
        HiringManager = 2,
        [Description("Admin")]
        Admin = 3
    }
}