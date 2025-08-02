using Microsoft.AspNetCore.Identity;

namespace Project.Domain.Entities
{
    public class User : IdentityUser<Guid>
    {
        public string FullName { get; set; }
    }
}
