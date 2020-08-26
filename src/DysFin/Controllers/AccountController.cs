using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace DysFin.Controllers
{
    /// <summary>
    /// Kontroler pomocniczy do zarządzania użytkownikiem aplikacji.
    /// </summary>
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        /// <summary>
        /// Wylogowuje użytkownika z aplikacji.
        /// </summary>
        /// <returns>Przekierowuje do strony z informacją o wylogowaniu.</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToPage("/Account/SignedOut");
        }
    }
}
