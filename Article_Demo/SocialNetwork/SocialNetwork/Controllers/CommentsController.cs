using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Models.DbModels;
using SocialNetwork.Models.Forms;

namespace SocialNetwork.Controllers
{
    public class CommentsController : Controller
    {
        private SocialNetworkDbContext _db;

        private IConfiguration _configuration;

        public CommentsController(SocialNetworkDbContext db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        public IActionResult GetComments([FromQuery]int articleId)
        {
            List<Comment> comments = _db.Comments.Where(c => c.ArticleId == articleId).ToList();
            comments.Reverse();
            return Json(comments);
        }

        [HttpPost]
        public IActionResult PostComment([FromForm]PostCommentForm form)
        {
            Comment comment = new Comment() { ArticleId = form.ArticleId, AuthorId = form.AuthorId, AuthorNickname = form.AuthorNickname, AuthorProfileImageName = form.AuthorProfileImageName, Content = form.Content, PostDateTime = DateTime.Now };
            _db.Comments.Add(comment);
            _db.SaveChanges();
            return Redirect($"{_configuration["Protocol"]}://{_configuration["ArticlesDomain"]}/RedirectTest/Articles/ViewArticle?articleId=" + form.ArticleId);
        }
    }
}
