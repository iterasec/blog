using RedirectTest.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RedirectTest.Models.DbModels;
using RedirectTest.Models.Forms;
using RedirectTest.Models.ViewModels;
using RedirectTest.Services;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace RedirectTest.Controllers
{
    [Authentication]
    public class ProfileController : Controller
    {
        private CircleScribeDbContext _db;

        private IConfiguration _configuration;

        private StringHelper _stringHelper;

        public ProfileController(CircleScribeDbContext db, IConfiguration configuration, StringHelper stringHelper)
        {
            _db = db;
            _configuration = configuration;
            _stringHelper = stringHelper;
        }

        public IActionResult Me()
        {
            List<Claim> claims = HttpContext.User.Claims.ToList();
            int userId = Convert.ToInt32(claims[0].Value);

            User user = _db.Users.Where(u => u.Id == userId).FirstOrDefault();
            if (user == null)
                return StatusCode(500);

            ProfileViewModel profile = new ProfileViewModel() { Id = user.Id, NickName = user.Nickname, BaseApiUrl = user.BaseApiUrl, ProfileImageName = user.ProfileImageName };
            return View(profile);
        }

        [HttpPost]
        public IActionResult ChangeNickname([FromForm] ChangeNicknameForm form)
        {
            List<Claim> claims = HttpContext.User.Claims.ToList();
            int userId = Convert.ToInt32(claims[0].Value);

            User user = _db.Users.Where(u => u.Id == userId).FirstOrDefault();
            if (user == null)
                return StatusCode(500);

            user.Nickname = form.newNickname;
            _db.SaveChanges();

            return LocalRedirect("/RedirectTest/Profile/Me");
        }

        [HttpPost]
        public IActionResult ChangePassword([FromForm] ChangePasswordForm form)
        {
            List<Claim> claims = HttpContext.User.Claims.ToList();
            int userId = Convert.ToInt32(claims[0].Value);

            User user = _db.Users.Where(u => u.Id == userId).FirstOrDefault();
            if (user == null)
                return StatusCode(500);

            user.PasswordHash = _stringHelper.GetPasswordHash(form.Content);
            _db.SaveChanges();

            return LocalRedirect("/RedirectTest/Profile/Me");

        }

        [HttpPost]
        public async Task<IActionResult> ChangeProfileImage(ChangeProfileImage form)
        {
            List<Claim> claims = HttpContext.User.Claims.ToList();
            int userId = Convert.ToInt32(claims[0].Value);

            User user = _db.Users.Where(u => u.Id == userId).FirstOrDefault();
            if (user == null)
                return StatusCode(500);

            byte[] imageBytes = new byte[form.newProfileImage.Length];
            await form.newProfileImage.OpenReadStream().ReadAsync(imageBytes, 0, imageBytes.Length);
            string newImageName = _stringHelper.GenerateRandomImageName(form.newProfileImage.FileName);

            using (FileStream fs = new FileStream(Path.Combine("wwwroot", "RedirectTestStatic", "ProfileImages", newImageName), FileMode.Create, FileAccess.Write))
            {
                fs.Write(imageBytes, 0, imageBytes.Length);
            }

            user.ProfileImageName = newImageName;
            _db.SaveChanges();

            return LocalRedirect("/RedirectTest/Profile/Me");
        }

    }
}
