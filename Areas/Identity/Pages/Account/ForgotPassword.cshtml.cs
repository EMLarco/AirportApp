#nullable disable
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AirportWebApp.Models;

namespace AirportWebApp.Areas.Identity.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly Microsoft.AspNetCore.Identity.UI.Services.IEmailSender _emailSender;

        public ForgotPasswordModel(UserManager<ApplicationUser> userManager, Microsoft.AspNetCore.Identity.UI.Services.IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
            [EmailAddress(ErrorMessage = "Introduce un correo electrónico válido.")]
            public string Email { get; set; }
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Input.Email);
            // Do not reveal whether the user exists
            if (user == null)
            {
                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            // Use WebEncoders to encode token safely
            var safeCode = System.Text.Encoding.UTF8.GetBytes(token);
            var encoded = Microsoft.AspNetCore.WebUtilities.WebEncoders.Base64UrlEncode(safeCode);
            var callbackUrl = Url.Page("/Account/ResetPassword", pageHandler: null, values: new { area = "Identity", code = encoded, email = Input.Email }, protocol: Request.Scheme);

            var subject = "Restablecer contraseña";
            var message = $"Se solicitó restablecer la contraseña. Pulsa <a href=\"{System.Text.Encodings.Web.HtmlEncoder.Default.Encode(callbackUrl)}\">aquí</a> para restablecerla.";

            await _emailSender.SendEmailAsync(Input.Email, subject, message);

            return RedirectToPage("./ForgotPasswordConfirmation");
        }
    }
}
