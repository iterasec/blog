using RedirectTest.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using RedirectTest.Filters;
using RedirectTest.Models.Forms;
using RedirectTest.Models.DbModels;
using RedirectTest.Models.ViewModels;
using RedirectTest.Services;

namespace RedirectTest.Controllers
{
    [Authentication]
    public class ArticlesJsController : Controller
    {
        private CircleScribeDbContext _db;

        private IConfiguration _configuration;

        private JWT_Generator _jwt_generator;

        public ArticlesJsController(CircleScribeDbContext db, IConfiguration configuration, JWT_Generator jWT_Generator)
        {
            _db = db;
            _configuration = configuration;
            _jwt_generator = jWT_Generator;
        }

        public IActionResult Show()
        {
            List<Claim> claims = HttpContext.User.Claims.ToList();
            int userId = Convert.ToInt32(claims[0].Value);

            User user = _db.Users.Where(u => u.Id == userId).Include(u => u.Articles).FirstOrDefault();
            if (user == null)
                return StatusCode(500, "No such user...");

            List<Article> articles = _db.Articles.Where(a => a.UserId == user.Id || a.IsShared).Include(a => a.User).ToList();
            List<ArticleViewModel> articleViewModels = new List<ArticleViewModel>();
            articles.Reverse();

            foreach (var art in articles)
            {
                articleViewModels.Add(new ArticleViewModel() { ArticleId = art.Id, Content = art.Content, Title = art.Title, AuthorId = art.UserId, IsShared = art.IsShared, OldApi = art.OldApi, AuthorNickname = art.User.Nickname, AuthorProfileImage = art.User.ProfileImageName });
            }
            ViewBag.userId = userId;
            return View(articleViewModels);
        }

        public async Task<IActionResult> ViewArticle(int articleId)
        {
            List<Claim> claims = HttpContext.User.Claims.ToList();
            int userId = Convert.ToInt32(claims[0].Value);

            User user = _db.Users.Where(u => u.Id == userId).FirstOrDefault();
            if (user == null)
                return StatusCode(500, "No such user...");

            Article article = _db.Articles.Where(a => a.Id == articleId).Include(a => a.User).FirstOrDefault();
            if (article == null)
                return StatusCode(500, "No such article...");

            if (article.UserId != userId && !article.IsShared)
                return StatusCode(403, "You can't view this article...");

            ArticleViewModel articleViewModel = new ArticleViewModel() { ArticleId = article.Id, Title = article.Title, Content = article.Content, AuthorId = article.UserId, OldApi = article.OldApi, IsShared = article.IsShared, AuthorNickname = article.User.Nickname, AuthorProfileImage = article.User.ProfileImageName };
            ViewBag.BaseUrl = user.BaseApiUrl;
            ViewBag.CommentAuthorId = user.Id;
            ViewBag.CommentAuthorNickname = user.Nickname;
            ViewBag.CommentAuthorProfileImageName = user.ProfileImageName;
            ViewBag.Token = _jwt_generator.GenerateJwtToken(userId, user.Nickname, articleId);
            return View(articleViewModel);
        }

        public IActionResult CreateArticle()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateArticle([FromBody] CreateArticleForm form)
        {
            List<Claim> claims = HttpContext.User.Claims.ToList();
            int userId = Convert.ToInt32(claims[0].Value);

            User user = _db.Users.Where(u => u.Id == userId).Include(u => u.Articles).FirstOrDefault();
            if (user == null)
                return StatusCode(500, "No such user...");

            Article article = new Article() { Title = form.Title, Content = form.Content, IsShared = string.IsNullOrEmpty(form.IsShared) ? false : true, OldApi = false };
            user.Articles.Add(article);
            _db.SaveChanges();

            return LocalRedirect("/RedirectTest/ArticlesJs/Show");
        }

        public IActionResult EditArticle(int articleId)
        {
            List<Claim> claims = HttpContext.User.Claims.ToList();
            int userId = Convert.ToInt32(claims[0].Value);

            User user = _db.Users.Where(u => u.Id == userId).Include(u => u.Articles).FirstOrDefault();
            if (user == null)
                return StatusCode(500, "No such user...");

            Article article = _db.Articles.Where(a => a.Id == articleId).FirstOrDefault();
            if (article == null)
                return StatusCode(500, "No such article");

            if (article.User.Id != userId)
                return StatusCode(401, "This is not your article. You can't modify it...");

            ArticleViewModel articleViewModel = new ArticleViewModel() { ArticleId = articleId, Content = article.Content, Title = article.Title, IsShared = article.IsShared, OldApi = article.OldApi };
            return View(articleViewModel);
        }

        [HttpPost]
        public IActionResult EditArticle([FromBody] EditArticleForm form)
        {
            List<Claim> claims = HttpContext.User.Claims.ToList();
            int userId = Convert.ToInt32(claims[0].Value);

            User user = _db.Users.Where(u => u.Id == userId).Include(u => u.Articles).FirstOrDefault();
            if (user == null)
                return StatusCode(500, "No such user...");


            Article article = _db.Articles.Where(a => a.Id == form.ArticleId).FirstOrDefault();
            if (article == null)
                return StatusCode(500, "No such article...");

            if (userId != article.User.Id)
                return StatusCode(401, "This is not your article. You can't modify it...");

            article.Title = form.Title;
            article.Content = form.Content;
            article.IsShared = string.IsNullOrEmpty(form.IsShared) ? false : true;

            _db.SaveChanges();
            return LocalRedirect("/RedirectTest/ArticlesJs/Show");
        }

        [HttpPost]
        public IActionResult DeleteArticle([FromBody] DeleteArticleForm form)
        {
            List<Claim> claims = HttpContext.User.Claims.ToList();
            int userId = Convert.ToInt32(claims[0].Value);

            User user = _db.Users.Where(u => u.Id == userId).Include(u => u.Articles).FirstOrDefault();
            if (user == null)
                return StatusCode(500, "No such user...");

            Article article = _db.Articles.Where(a => a.Id == form.articleId).Include(a => a.User).FirstOrDefault();
            if (article == null)
                return StatusCode(500, "No such article...");

            if (user.Id != article.User.Id)
                return StatusCode(401, "This is not your article. You can't delete it...");

            _db.Articles.Remove(article);
            _db.SaveChanges();

            return LocalRedirect("/RedirectTest/ArticlesJs/Show");
        }
    }
}
