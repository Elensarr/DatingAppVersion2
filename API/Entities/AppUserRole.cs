using Microsoft.AspNetCore.Identity;

namespace API.Entities
{
    // joined entitiy for many to many ralationship Users-Roles
    public class AppUserRole : IdentityUserRole<int>
    {
        public AppUser User { get; set; }
        public AppRole Role { get; set; }
    }
}
