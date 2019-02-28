using Demo.Identity.Enums;

namespace Demo.Identity.Models
{
    public class RegistrationViewModel
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public RoleEnum Role { get; set; }
    }
}