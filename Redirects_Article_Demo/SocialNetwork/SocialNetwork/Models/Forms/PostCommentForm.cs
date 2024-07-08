namespace SocialNetwork.Models.Forms
{
    public class PostCommentForm
    {
        public int AuthorId { get; set; }

        public string AuthorNickname { get; set; }

        public string AuthorProfileImageName { get; set; }

        public int ArticleId { get; set; }

        public string Content { get; set; }
    }
}
