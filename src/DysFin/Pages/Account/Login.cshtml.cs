using DysFin.Data;
using DysFin.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.Protocols;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DysFin.Pages.Account
{
    /// <summary>
    /// Strona logowania.
    /// </summary>
    public class LoginModel : PageModel
    {
        private readonly DysFinContext _context;

        private readonly IWebHostEnvironment _env;

        public LoginModel(DysFinContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        /// URL odsy�aj�cy.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        /// Tre�� b��du.
        /// </summary>
        [TempData]
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Dane logowania u�ytkownika.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            /// Login.
            /// </summary>
            [Required]
            [Display(Name = "Login")]
            public string Login { get; set; }

            /// <summary>
            /// Has�o.
            /// </summary>
            [Required]
            [Display(Name = "Has�o")]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        /// <summary>
        /// Wy�wietlenie strony logowania.
        /// </summary>
        /// <param name="returnUrl">URL odsy�aj�cy.</param>
        /// <returns>Strona logowania.</returns>
        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            // Wyczy�� istniej�ce zewn�trzne ciasteczko
            #region snippet2
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
            #endregion

            ReturnUrl = returnUrl;
        }

        /// <summary>
        /// Przesy�a dane logowania do aplikacji.
        /// </summary>
        /// <param name="returnUrl">URL odsy�aj�cy.</param>
        /// <returns>Loguje u�ytkownika i odsy�a do przekazanego adresu b�d� wy�wietlania informacj� o nieudanym logowaniu.</returns>
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;

            if (ModelState.IsValid)
            {
                var user = await AuthenticateUserAsync(Input.Login);

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Nie ma takiego u�ytkownika.");
                    return Page();
                }
                else if (!user.Status)
                {
                    ModelState.AddModelError(string.Empty, "U�ytkownik wygaszony.");
                    return Page();
                }

                #region snippet1
                using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, "domain.local:636"))
                {
                    try
                    {
                        bool nonProduction = false;

                        if (_env.IsStaging() || _env.IsDevelopment())
                        {
                            if (user.Login == "testowy")
                            {
                                nonProduction = true;
                            }
                        }

                        if (pc.ValidateCredentials(Input.Login, Input.Password, ContextOptions.Negotiate) || nonProduction)
                        {
                            var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                                new Claim(ClaimTypes.Name, user.ImieNazwisko),
                                new Claim(ClaimTypes.GivenName, user.Imie),
                                new Claim(ClaimTypes.Surname, user.Nazwisko),
                                new Claim(ClaimTypes.Role, user.PoziomUzytkownika.Id.ToString())
                            };

                            if (user.KomorkaId.HasValue)
                            {
                                claims.Add(new Claim("komorka", user.KomorkaId.ToString()));
                            }

                            var claimsIdentity = new ClaimsIdentity(
                                claims, CookieAuthenticationDefaults.AuthenticationScheme);

                            var authProperties = new AuthenticationProperties
                            {
                                //AllowRefresh = <bool>,
                                // Refreshing the authentication session should be allowed.

                                //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                                // The time at which the authentication ticket expires. A 
                                // value set here overrides the ExpireTimeSpan option of 
                                // CookieAuthenticationOptions set with AddCookie.

                                //IsPersistent = true,
                                // Whether the authentication session is persisted across 
                                // multiple requests. When used with cookies, controls
                                // whether the cookie's lifetime is absolute (matching the
                                // lifetime of the authentication ticket) or session-based.

                                //IssuedUtc = <DateTimeOffset>,
                                // The time at which the authentication ticket was issued.

                                //RedirectUri = <string>
                                // The full path or absolute URI to be used as an http 
                                // redirect response value.
                            };

                            await HttpContext.SignInAsync(
                                CookieAuthenticationDefaults.AuthenticationScheme,
                                new ClaimsPrincipal(claimsIdentity),
                                authProperties);
                            #endregion

                            Log
                                .ForContext("UserId", user.Id)
                                .ForContext("Table", user.GetType().Name)
                                .Warning("Logowanie u�ytkownika {Login}.", user.Login);

                            return LocalRedirect(Url.GetLocalUrl(returnUrl));
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Nieprawid�owe dane logowania do domeny.");
                            return Page();
                        }
                    }
                    catch (LdapException)
                    {
                        ModelState.AddModelError(string.Empty, "Serwer LDAP jest niedost�pny.");
                        return Page();
                    }
                }
            }

            // Co� posz�o nie tak. Od�wie� formularz.
            return Page();
        }

        /// <summary>
        /// Uwierzytelnia u�ytkownika.
        /// </summary>
        /// <param name="login">Login u�ytkownika.</param>
        /// <returns>Profil u�ytkownika je�li istnieje.</returns>
        private async Task<Uzytkownik> AuthenticateUserAsync(string login)
        {
            var user = await _context.Uzytkownik.Include(u => u.PoziomUzytkownika).FirstOrDefaultAsync(u => u.Login == login);

            if (user != null)
            {
                return user;
            }
            else
            {
                return null;
            }
        }
    }
}
