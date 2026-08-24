using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BC.PAYMENT.REPORTS.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _config;

        public HomeController(IConfiguration config)
        {
            _config = config;
        }

        public IActionResult Index()
        {
            ViewBag.HideFooter = true;
            ViewBag.HideHeader = true;
            return View();
        }

        public IActionResult Designer()
        {
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Index");

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _config["Jwt:Issuer"],
                    ValidAudience = _config["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(_config["Jwt:Key"]))
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                ViewBag.Username = principal.Identity?.Name ?? "Unknown";
            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }

            return View();
        }
        

        public IActionResult Viewer()
        {
            return View();
        }

        public IActionResult Login()
        {
            ViewBag.HideHeader = true;
            ViewBag.HideFooter = true;

            return View();
        }
    }
}
