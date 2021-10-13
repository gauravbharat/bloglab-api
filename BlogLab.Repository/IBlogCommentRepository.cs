using System.Collections.Generic;
using System.Threading.Tasks;
using BlogLab.Models.BlogComment;

namespace BlogLab.Repository
{
    public interface IBlogCommentRepository
    {
        public Task<BlogComment> UpsertAsync(BlogCommentCreate blogCommentCreate, int applicationUserId);

        public Task<BlogComment> GetAsync(int blogCommentId);

        public Task<List<BlogComment>> GetAllByBlogIdAsync(int blogId);

        public Task<int> DeleteAsync(int blogCommentId);
    }
}
