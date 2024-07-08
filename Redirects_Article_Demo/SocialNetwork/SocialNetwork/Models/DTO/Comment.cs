namespace SocialNetwork.Models.DTO
{
    public class Comment
    {
        public int ArticleId { get; set; }

        public int AuthorId { get; set; }

        public string AuthorEmail { get; set; }

        public DateTime PostDateTime { get; set; }

        public string Content { get; set; }
    }
}
