using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using RedirectTest.Filters;
using Microsoft.EntityFrameworkCore;
using RedirectTest.Models;
using RedirectTest.Models.Forms;
using RedirectTest.Models.DbModels;
using RedirectTest.Models.ViewModels;

namespace RedirectTest.Controllers
{
    [Authentication]
    public class ArticlesController : Controller
    {
        private CircleScribeDbContext _db;

        private IConfiguration _configuration;

        public ArticlesController(CircleScribeDbContext db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        public IActionResult Show()
        {
            List<Claim> claims = HttpContext.User.Claims.ToList();
            int userId = Convert.ToInt32(claims[0].Value);

            User user = _db.Users.Where(u => u.Id == userId).FirstOrDefault();
            if (user == null)
                return StatusCode(500, "No such user...");

            List<Article> articles = _db.Articles.Where(a => a.UserId == userId || a.IsShared).Include(a => a.User).ToList();
            List<ArticleViewModel> articleViewModels = new List<ArticleViewModel>();

            articles.Reverse();

            foreach (var art in articles)
            {
                articleViewModels.Add(new ArticleViewModel() { ArticleId = art.Id, Content = art.Content, Title = art.Title, IsShared = art.IsShared, OldApi = art.OldApi, AuthorId = art.User.Id, AuthorNickname = art.User.Nickname, AuthorProfileImage = art.User.ProfileImageName });
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

            ArticleViewModel articleViewModel = new ArticleViewModel() { ArticleId = article.Id, Title = article.Title, Content = article.Content, IsShared = article.IsShared, AuthorId = article.UserId, AuthorNickname = article.User.Nickname, AuthorProfileImage = article.User.ProfileImageName };
            ViewBag.BaseUrl = user.BaseApiUrl;
            ViewBag.CommentAuthorId = user.Id;
            ViewBag.CommentAuthorNickname = user.Nickname;
            ViewBag.CommentAuthorProfileImageName = user.ProfileImageName;

            List<Comment> comments = new List<Comment>();
            try
            {
                HttpClient client = new HttpClient();
                Uri requestUri = new Uri($"{_configuration["Protocol"]}://" + user.BaseApiUrl + "/Social/Comments/GetComments?articleId=" + article.Id);
                HttpRequestMessage httpRequestMessage = new HttpRequestMessage() { Method = HttpMethod.Get, RequestUri = requestUri };
                HttpResponseMessage response = client.Send(httpRequestMessage);
                HttpContent content = response.Content;
                comments = (List<Comment>)await content.ReadFromJsonAsync(typeof(List<Comment>));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Need to log this...");
            }

            ViewBag.Comments = comments;
            return View(articleViewModel);
        }

        public IActionResult CreateArticle()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateArticle([FromForm] CreateArticleForm form)
        {
            List<Claim> claims = HttpContext.User.Claims.ToList();
            int userId = Convert.ToInt32(claims[0].Value);

            User user = _db.Users.Where(u => u.Id == userId).Include(u => u.Articles).FirstOrDefault();
            if (user == null)
                return StatusCode(500, "No such user...");

            Article article = new Article() { Title = form.Title, Content = form.Content, IsShared = string.IsNullOrEmpty(form.IsShared) ? false : true, OldApi = true };
            user.Articles.Add(article);
            _db.SaveChanges();

            return LocalRedirect("/RedirectTest/Articles/Show");
        }


        public IActionResult EditArticle(int articleId)
        {
            List<Claim> claims = HttpContext.User.Claims.ToList();
            int userId = Convert.ToInt32(claims[0].Value);

            User user = _db.Users.Where(u => u.Id == userId).Include(u => u.Articles).FirstOrDefault();
            if (user == null)
                return StatusCode(500, "No such user...");

            Article article = user.Articles.Where(a => a.Id == articleId).FirstOrDefault();
            if (article == null)
                return StatusCode(500, "No such article");

            ArticleViewModel articleViewModel = new ArticleViewModel() { ArticleId = articleId, Content = article.Content, Title = article.Title, IsShared = article.IsShared, OldApi = article.OldApi };
            return View(articleViewModel);
        }

        [HttpPost]
        public IActionResult EditArticle([FromForm] EditArticleForm form)
        {
            List<Claim> claims = HttpContext.User.Claims.ToList();
            int userId = Convert.ToInt32(claims[0].Value);

            User user = _db.Users.Where(u => u.Id == userId).Include(u => u.Articles).FirstOrDefault();
            if (user == null)
                return StatusCode(500, "No such user...");

            Article article = user.Articles.Where(a => a.Id == form.ArticleId).FirstOrDefault();
            if (article == null)
                return StatusCode(500, "No such article...");

            article.Title = form.Title;
            article.Content = form.Content;
            article.IsShared = string.IsNullOrEmpty(form.IsShared) ? false : true;

            _db.SaveChanges();
            return LocalRedirect("/RedirectTest/Articles/Show");
        }

        [HttpPost]
        public IActionResult DeleteArticle([FromForm] DeleteArticleForm form)
        {
            List<Claim> claims = HttpContext.User.Claims.ToList();
            int userId = Convert.ToInt32(claims[0].Value);

            User user = _db.Users.Where(u => u.Id == userId).Include(u => u.Articles).FirstOrDefault();
            if (user == null)
                return StatusCode(500, "No such user...");

            Article article = user.Articles.Where(a => a.Id == form.articleId).FirstOrDefault();
            if (article == null)
                return StatusCode(500, "No such article...");

            _db.Articles.Remove(article);
            _db.SaveChanges();

            return LocalRedirect("/RedirectTest/Articles/Show");
        }
    }
}
