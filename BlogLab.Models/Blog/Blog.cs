﻿using System;
namespace BlogLab.Models.Blog
{
    public class Blog : BlogCreate
    {

        public string Username { get; set; }

        public string ApplicationUserId { get; set; }

        public DateTime PublishDate { get; set; }

        public DateTime UpdateDate { get; set; }
    }
}
