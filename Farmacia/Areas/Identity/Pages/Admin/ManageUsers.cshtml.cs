using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Farmacia.Areas.Identity.Pages.Admin
{
    [Authorize(Roles = "Administrador")]
    public class ManageUsersModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public ManageUsersModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public List<UserInfo> Users { get; set; }

        public class UserInfo
        {
            public string Id { get; set; }
            public string Email { get; set; }
            public IList<string> Roles { get; set; }
        }

        public async Task OnGetAsync()
        {
            Users = new List<UserInfo>();
            var users = _userManager.Users.ToList();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                Users.Add(new UserInfo
                {
                    Id = user.Id,
                    Email = user.Email,
                    Roles = roles
                });
            }
        }
    }
}