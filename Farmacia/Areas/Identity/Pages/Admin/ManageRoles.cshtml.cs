using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Farmacia.Areas.Identity.Pages.Admin
{
    [Authorize(Roles = "Administrador")]
    public class ManageRolesModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ManageRolesModel(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IdentityUser SelectedUser { get; set; }
        public IList<string> UserRoles { get; set; }
        public List<string> AllRoles => _roleManager.Roles.Select(r => r.Name).ToList();

        public async Task<IActionResult> OnGetAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return RedirectToPage("/Admin/ManageUsers");

            SelectedUser = await _userManager.FindByIdAsync(userId);
            if (SelectedUser == null)
                return NotFound();

            UserRoles = await _userManager.GetRolesAsync(SelectedUser);
            return Page();
        }

        public async Task<IActionResult> OnPostChangeRoleAsync(string userId, string role, bool add)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            if (add)
                await _userManager.AddToRoleAsync(user, role);
            else
                await _userManager.RemoveFromRoleAsync(user, role);

            return RedirectToPage(new { userId });
        }
    }
}