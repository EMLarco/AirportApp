#nullable disable
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;

namespace AirportWebApp.Areas.Identity.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<Models.ApplicationUser> _signInManager;

        public LogoutModel(SignInManager<Models.ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            if (string.IsNullOrEmpty(returnUrl))
            {
                return RedirectToPage("~/");
            }
            return LocalRedirect(returnUrl);
        }
    }
}
