namespace RedirectTest.Models.DbModels
{
    public class User
    {
        public int Id { get; set; }

        public string Email { get; set; }

        public string ProfileImageName { get; set; }

        public string Nickname { get; set; }

        public string PasswordHash { get; set; }

        public string BaseApiUrl { get; set; }

        public virtual List<Article> Articles { get; set; }
    }
}
