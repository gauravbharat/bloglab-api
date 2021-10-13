using System;
using System.Data;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using BlogLab.Models.BlogComment;
using Dapper;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace BlogLab.Repository
{
    public class BlogCommentRepository : IBlogCommentRepository
    {
        private readonly IConfiguration _config;
        private readonly string _connectionString;

        public BlogCommentRepository(IConfiguration config)
        {
            _config = config;
            _connectionString = _config.GetConnectionString(GlobalVariables.connectionString);
        }

        public async Task<int> DeleteAsync(int blogCommentId)
        {
            int affectedRows = 0;

            using (var connection = new SqlConnection(_connectionString)) {
                await connection.OpenAsync();

                affectedRows = await connection.ExecuteAsync("BlogComment_Delete", new { BlogCommentId = blogCommentId }, commandType: CommandType.StoredProcedure);
            }

            return affectedRows;
        }

        public async Task<List<BlogComment>> GetAllByBlogIdAsync(int blogId)
        {
            IEnumerable<BlogComment> blogComments;

            using (var connection = new SqlConnection(_connectionString)) {
                await connection.OpenAsync();

                blogComments = await connection.QueryAsync<BlogComment>("BlogComment_GetAll", new { BlogId = blogId }, commandType: CommandType.StoredProcedure);
            }

            return blogComments.ToList();
        }

        public async Task<BlogComment> GetAsync(int blogCommentId)
        {
            BlogComment blogComment;

            using (var connection = new SqlConnection(_connectionString)) {
                await connection.OpenAsync();

                blogComment = await connection.QueryFirstOrDefaultAsync<BlogComment>("BlogComment_Get", new { BlogCommentId = blogCommentId }, commandType: CommandType.StoredProcedure);
            }

            return blogComment;
        }

        public async Task<BlogComment> UpsertAsync(BlogCommentCreate blogCommentCreate, int applicationUserId)
        {
            var dataTable = new DataTable();

            dataTable.Columns.Add("blogCommentId", typeof(int));
            dataTable.Columns.Add("parentBlogCommentId", typeof(int));
            dataTable.Columns.Add("blogId", typeof(int));
            dataTable.Columns.Add("content", typeof(string));

            dataTable.Rows.Add(
                blogCommentCreate.BlogCommentId,
                blogCommentCreate.ParentBlogCommentId,
                blogCommentCreate.BlogId,
                blogCommentCreate.Content
                );

            int? newBlogCommentId;

            using (var connection = new SqlConnection(_connectionString)) {
                await connection.OpenAsync();

                newBlogCommentId = await connection.ExecuteScalarAsync<int?>(
                    "BlogComment_Upsert"
                    , new { BlogComment = dataTable.AsTableValuedParameter("dbo.BlogComment"), ApplicationUserId = applicationUserId }
                    , commandType: CommandType.StoredProcedure);
            }

            newBlogCommentId = newBlogCommentId ?? blogCommentCreate.BlogCommentId;

            BlogComment blogComment = await GetAsync(newBlogCommentId.Value);

            return blogComment;
        }
    }
}
