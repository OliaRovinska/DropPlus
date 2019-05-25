using DropPlus.Common.Enums;

namespace DropPlus.Identity.Models
{
    public class RegistrationViewModel
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public RoleEnum Role { get; set; }
    }
}