using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog;
using System.Linq;

namespace DysFin.Pages.Account
{
    /// <summary>
    /// Strona braku dostêpu.
    /// </summary>
    public class AccessDeniedModel : PageModel
    {
        /// <summary>
        /// Wyœwietlenie strony braku dostêpu.
        /// </summary>
        public void OnGet()
        {
            Log
                .ForContext("UserId", int.Parse(User.Claims.FirstOrDefault(u => u.Type.EndsWith("nameidentifier")).Value))
                .ForContext("Table", nameof(Models.Uzytkownik))
                .Warning("Próba nieautoryzowanego dostêpu do {Path}.", Request.Path);
        }
    }
}
