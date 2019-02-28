using System.ComponentModel;

namespace Demo.Identity.Enums
{
    public enum RoleEnum
    {
        User = 1,
        [Description("Hiring manager")]
        HiringManager = 2,
        Admin = 3
    }
}