using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CookieAuthentication.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace CookieAuthentication.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {

        private readonly ApplicationDbContext Db;

        public LoginModel(ApplicationDbContext Db)
        {
            this.Db = Db;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }


        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (ModelState.IsValid)
            {
                var user = Db.Users.Where(f => f.Email == Input.Email && f.Password == Input.Password).FirstOrDefault();
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid Email or Password");
                    return Page();
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim("UserDefined", "whatever"),
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        principal,
                        new AuthenticationProperties { IsPersistent = true });

                //Different configurations that you can set to SignInAsync:
                    //AllowRefresh = <bool>,
                    // Refreshing the authentication session should be allowed.
 
                    //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                    // The time at which the authentication ticket expires. A value set here overrides the ExpireTimeSpan option of CookieAuthenticationOptions set with AddCookie.
 
                    //IsPersistent = true,
                    // Whether the authentication session is persisted across multiple requests. When used with cookies, controls whether the cookie's lifetime is absolute (matching thelifetime of the authentication ticket) or session-based.
 
                    //IssuedUtc = <DateTimeOffset>,
                    // The time at which the authentication ticket was issued.
 
                    //RedirectUri = <string>
                    // The full path or absolute URI to be used as an http 
                    // redirect response value.



                return LocalRedirect(returnUrl);
            }

            return Page();
        }
    }
}
