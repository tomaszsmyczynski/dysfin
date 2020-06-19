using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DysFin.Pages.Account
{
    /// <summary>
    /// Strona potwierdzenia wylogowania.
    /// </summary>
    public class SignedOutModel : PageModel
    {
        /// <summary>
        /// Wyœwietlenie strony wylogowania u¿ytkownika.
        /// </summary>
        /// <returns>Strona z komunikatem.</returns>
        public IActionResult OnGet()
        {
            if (User.Identity.IsAuthenticated)
            {
                // Przekierowanie do strony domowej jeœli u¿ytkownik zosta³ uwierzytelniony.
                return RedirectToPage("/Index");
            }

            return Page();
        }
    }
}
