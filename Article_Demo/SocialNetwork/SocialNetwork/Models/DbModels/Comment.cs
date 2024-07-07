﻿namespace SocialNetwork.Models.DbModels
{
    public class Comment
    {
        public int Id { get; set; }

        public int ArticleId { get; set; }

        public int AuthorId { get; set; }

        public string AuthorNickname { get; set; }

        public string AuthorProfileImageName { get; set; }

        public DateTime PostDateTime { get; set; }

        public string Content { get; set; }
    }
}
