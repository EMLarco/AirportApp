#nullable disable
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AirportWebApp.Models;

namespace AirportWebApp.Areas.Identity.Pages.Account
{
    public class ResetPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ResetPasswordModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
            [EmailAddress(ErrorMessage = "Introduce un correo electrónico válido.")]
            public string Email { get; set; }

            [Required(ErrorMessage = "La contraseña es obligatoria.")]
            [StringLength(100, ErrorMessage = "La contraseña debe tener al menos {2} caracteres.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            public string Code { get; set; }
        }

        public void OnGet(string code = null)
        {
            Input = new InputModel { Code = code };
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                // Do not reveal that the user does not exist
                return RedirectToPage("./ResetPasswordConfirmation");
            }

            try
            {
                var decodedBytes = Microsoft.AspNetCore.WebUtilities.WebEncoders.Base64UrlDecode(Input.Code ?? string.Empty);
                var decodedToken = System.Text.Encoding.UTF8.GetString(decodedBytes);
                var result = await _userManager.ResetPasswordAsync(user, decodedToken, Input.Password);
                if (result.Succeeded)
                {
                    return RedirectToPage("./ResetPasswordConfirmation");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "El código de restablecimiento no es válido.");
            }

            return Page();
        }
    }
}
