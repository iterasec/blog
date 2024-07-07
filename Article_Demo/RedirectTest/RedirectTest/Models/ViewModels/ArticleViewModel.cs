namespace RedirectTest.Models.ViewModels
{
    public class ArticleViewModel
    {
        public int ArticleId { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public bool IsShared { get; set; }

        public bool OldApi { get; set; }

        public int AuthorId { get; set; }

        public string AuthorNickname { get; set; }

        public string AuthorProfileImage { get; set; }
    }
}
