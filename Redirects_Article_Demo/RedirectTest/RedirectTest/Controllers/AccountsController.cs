using RedirectTest.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using RedirectTest.Services;
using RedirectTest.Models.DbModels;
using RedirectTest.Models.Forms;

namespace RedirectTest.Controllers
{
    public class AccountsController : Controller
    {
        private CircleScribeDbContext _db;

        private IConfiguration _configuration;

        private StringHelper _stringHelper;

        public AccountsController(CircleScribeDbContext db, IConfiguration configuration, StringHelper stringHelper)
        {
            _db = db;
            _configuration = configuration;
            _stringHelper = stringHelper;
        }

       [HttpPost]
        public IActionResult ChangeBaseApiUrl([FromBody] ChangeBaseApiUrlForm form)
        {
            User user = _db.Users.Where(u => u.Id == form.Id).FirstOrDefault();
            if (user == null)
                return StatusCode(404);

            if (string.IsNullOrEmpty(form.NewBaseApiUrl))
                return StatusCode(500, "Can't set empty base url...");

            if (_stringHelper.CheckIfItIsBaseUrl(form.NewBaseApiUrl))
            {
                user.BaseApiUrl = form.NewBaseApiUrl;
                _db.SaveChanges();
                return Ok("Url was changed...");
            }
            else
            {
                return StatusCode(500, "Please, provide a valid base url...");
            }
        }

        public IActionResult Login()
        {
            if (HttpContext.User.Identity != null && HttpContext.User.Identity.IsAuthenticated)
            {
                List<Claim> claims = HttpContext.User.Claims.ToList();
                int userId = Convert.ToInt32(claims[0].Value);

                User user = _db.Users.Where(u => u.Id == userId).FirstOrDefault();
                if (user != null)
                    return LocalRedirect("/RedirectTest/Home/Index");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromForm] LoginForm loginForm)
        {
            User user = _db.Users.Where(u => u.Email == loginForm.Email).FirstOrDefault();
            if (user == null)
                return StatusCode(500, "Wrong Login or Password...");

            string loginFormPasswordHash = _stringHelper.GetPasswordHash(loginForm.Password);
            if (loginFormPasswordHash != user.PasswordHash)
                return StatusCode(500, "Wrong Login or Password...");

            await SignInUser(loginForm, user.Id);
            return LocalRedirect("/RedirectTest/Home/Index");
        }


        public IActionResult Register()
        {
            if (HttpContext.User.Identity != null && HttpContext.User.Identity.IsAuthenticated)
            {
                List<Claim> claims = HttpContext.User.Claims.ToList();
                int userId = Convert.ToInt32(claims[0].Value);

                User user = _db.Users.Where(u => u.Id == userId).FirstOrDefault();
                if (user != null)
                    return LocalRedirect("/RedirectTest/Home/Index");

            }

            return View();
        }

        [HttpPost]
        public IActionResult Register([FromForm] RegisterForm registerForm)
        {
            User user = _db.Users.Where(u => u.Email == registerForm.Email).FirstOrDefault();
            if (user != null)
                return StatusCode(500, "");

            User newUser = new User()
            {
                Email = registerForm.Email,
                Nickname = registerForm.Nickname,
                PasswordHash = _stringHelper.GetPasswordHash(registerForm.Password),
                BaseApiUrl = _configuration["BaseApiUrl"],
                ProfileImageName = "default.png"
            };

            _db.Users.Add(newUser);
            _db.SaveChanges();

            return LocalRedirect("/RedirectTest/Accounts/Login");
        }


        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return LocalRedirect("/RedirectTest/Accounts/Login");
        }


        private async Task SignInUser(LoginForm form, int UserId)
        {
            var claims = new List<Claim> { new Claim("Id", Convert.ToString(UserId)), new Claim(ClaimTypes.Email, form.Email) };

            ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await Request.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
        }

    }
}
