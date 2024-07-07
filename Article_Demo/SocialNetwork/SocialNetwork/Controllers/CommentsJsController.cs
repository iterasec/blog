using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Filters;
using SocialNetwork.Models.DbModels;
using SocialNetwork.Models.Forms;

namespace SocialNetwork.Controllers
{
    public class CommentsJsController : Controller
    {
        private SocialNetworkDbContext _db;

        public CommentsJsController(SocialNetworkDbContext db)
        {
            _db = db;
        }

        [Authentication]
        public IActionResult GetComments(int articleId)
        {
            List<Comment> comments = _db.Comments.Where(c => c.ArticleId == articleId).ToList();
            comments.Reverse();
            return Json(comments);
        }

        [Authentication]
        [HttpPost]
        public IActionResult PostComment([FromBody] PostCommentForm form)
        {
            Comment comment = new Comment() { ArticleId = form.ArticleId, AuthorId = form.AuthorId, AuthorNickname = form.AuthorNickname, AuthorProfileImageName = form.AuthorProfileImageName, Content = form.Content, PostDateTime = DateTime.Now };
            _db.Comments.Add(comment);
            _db.SaveChanges();
            return Ok();
        }
    }
}
