using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog;
using System.Linq;

namespace DysFin.Pages.Account
{
    /// <summary>
    /// Strona braku dost�pu.
    /// </summary>
    public class AccessDeniedModel : PageModel
    {
        /// <summary>
        /// Wy�wietlenie strony braku dost�pu.
        /// </summary>
        public void OnGet()
        {
            Log
                .ForContext("UserId", int.Parse(User.Claims.FirstOrDefault(u => u.Type.EndsWith("nameidentifier")).Value))
                .ForContext("Table", nameof(Models.Uzytkownik))
                .Warning("Pr�ba nieautoryzowanego dost�pu do {Path}.", Request.Path);
        }
    }
}
