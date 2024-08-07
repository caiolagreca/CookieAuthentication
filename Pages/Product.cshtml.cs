using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CookieAuthentication.Pages
{
    [Authorize(AuthenticationSchemes = "Cookies")]
    public class ProductModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
