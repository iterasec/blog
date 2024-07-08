using System.ComponentModel.DataAnnotations.Schema;

namespace RedirectTest.Models.DbModels
{
    public class Article
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public bool IsShared { get; set; }

        public bool OldApi { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        public User User { get; set; }
    }
}
