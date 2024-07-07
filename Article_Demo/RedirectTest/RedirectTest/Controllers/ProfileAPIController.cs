using RedirectTest.Filters;
using Microsoft.AspNetCore.Mvc;
using RedirectTest.Models.DbModels;
using RedirectTest.Models.Forms;
using RedirectTest.Models.ViewModels;
using RedirectTest.Services;
using System.Security.Claims;
using System.Text;

namespace RedirectTest.Controllers
{
    [Authentication]
    public class ProfileAPIController : Controller
    {
        private CircleScribeDbContext _db;

        private IConfiguration _configuration;

        private StringHelper _stringHelper;

        public ProfileAPIController(CircleScribeDbContext db, IConfiguration configuration, StringHelper stringHelper)
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
            return Json(profile);
        }

        [HttpPost]
        public IActionResult ChangeNickname([FromBody] ChangeNicknameForm form)
        {
            List<Claim> claims = HttpContext.User.Claims.ToList();
            int userId = Convert.ToInt32(claims[0].Value);

            User user = _db.Users.Where(u => u.Id == userId).FirstOrDefault();
            if (user == null)
                return StatusCode(500);

            user.Nickname = form.newNickname;
            _db.SaveChanges();

            return Ok("Nickname was changed...");
        }

        [HttpPost]
        public IActionResult ChangePassword([FromBody] ChangePasswordForm form)
        {
            List<Claim> claims = HttpContext.User.Claims.ToList();
            int userId = Convert.ToInt32(claims[0].Value);

            User user = _db.Users.Where(u => u.Id == userId).FirstOrDefault();
            if (user == null)
                return StatusCode(500);

            user.PasswordHash = _stringHelper.GetPasswordHash(form.Content);
            _db.SaveChanges();

            return Ok("Password was changed...");
        }

        //Still working on it.........
        //[HttpPost]
        //public async Task<IActionResult> ChangeProfileImage([FromBody]B64ChangeProfileImage form)
        //{
        //    List<Claim> claims = HttpContext.User.Claims.ToList();
        //    int userId = Convert.ToInt32(claims[0].Value);

        //    User user = _db.Users.Where(u => u.Id == userId).FirstOrDefault();
        //    if (user == null)
        //        return StatusCode(500);

        //    byte[] imageBytes = Convert.FromBase64String(form.B64EncodedImage);
        //    string[] parts = form.B64EncodedImage.Split(';');
        //    if (parts.Length != 2)
        //        return StatusCode(500, "Incorrect Format...");

        //    string[] parts2 = parts[0].Split('/');
        //    if (parts2.Length != 2)
        //        return StatusCode(500, "Incorrect Format...");

        //    string newImageName = _stringHelper.GenerateRandomImageName("." + parts2[1]);

        //    using (FileStream fs = new FileStream(Path.Combine("wwwroot", "RedirectTestStatic", "ProfileImages", newImageName), FileMode.Create, FileAccess.Write))
        //    {
        //        fs.Write(imageBytes, 0, imageBytes.Length);
        //    }

        //    user.ProfileImageName = newImageName;
        //    _db.SaveChanges();

        //    return Ok("Profile Image Changed...");
        //}
    }
}
