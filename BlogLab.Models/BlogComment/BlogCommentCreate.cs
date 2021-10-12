using System;
using System.ComponentModel.DataAnnotations;

namespace BlogLab.Models.BlogComment
{
    public class BlogCommentCreate
    {
        public int BlogCommentId { get; set; }

        public int? ParentBlogCommentId { get; set; }

        public int BlogId { get; set; }

        [Required(ErrorMessage = "Content is required")]
        [MinLength(10, ErrorMessage = "Content must be 10-300 characters!")]
        [MaxLength(300, ErrorMessage = "Content must be 10-300 characters!")]
        public string Content { get; set; }
    }
}
