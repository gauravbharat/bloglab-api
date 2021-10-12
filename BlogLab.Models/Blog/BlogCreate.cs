using System;
using System.ComponentModel.DataAnnotations;

namespace BlogLab.Models.Blog
{
    public class BlogCreate
    {
        public int BlogId { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [MinLength(10, ErrorMessage = "Title must be 10-50 characters!")]
        [MaxLength(50, ErrorMessage = "Title must be 10-50 characters!")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Content is required")]
        [MinLength(300, ErrorMessage = "Content must be 300-3000 characters!")]
        [MaxLength(3000, ErrorMessage = "Content must be 300-3000 characters!")]
        public string Content { get; set; }

        public int? PhotoId { get; set; }


    }
}
