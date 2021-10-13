using System;
using System.Data;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using BlogLab.Models.Blog;
using Dapper;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace BlogLab.Repository
{
    public class BlogRepository : IBlogRepository
    {
        private readonly IConfiguration _config;
        private readonly string _connectionString;

        public BlogRepository(IConfiguration config)
        {
            _config = config;
            _connectionString = _config.GetConnectionString(GlobalVariables.connectionString);
        }

        public async Task<int> DeleteAsync(int blogId)
        {
            int affectedRows = 0;

            using (var connection = new SqlConnection(_connectionString)) {
                await connection.OpenAsync();

                affectedRows = await connection.ExecuteAsync("Blog_Delete", new { BlogId = blogId }, commandType: CommandType.StoredProcedure);

            }

            return affectedRows;
        }

        public async Task<PagedResults<Blog>> GetAllAsync(BlogPaging blogPaging)
        {
            var blogResults = new PagedResults<Blog>();

            using(var connection = new SqlConnection(_connectionString)) {
                await connection.OpenAsync();

                using (var multi = await connection.QueryMultipleAsync("Blog_GetAll", new { Offset = (blogPaging.Page - 1) * blogPaging.PageSize, PageSize = blogPaging.PageSize }, commandType: CommandType.StoredProcedure)) {
                    blogResults.Items = multi.Read<Blog>();
                    blogResults.TotalCount = multi.ReadFirst<int>();
                }

               
            }

            return blogResults;
        }

        public async Task<List<Blog>> GetAllByUserIdAsync(int applicationUserId)
        {
            IEnumerable<Blog> blogs;

            using (var connection = new SqlConnection(_connectionString)) {
                await connection.OpenAsync();

                blogs = await connection.QueryAsync<Blog>("Blog_GetByUserId", new { ApplicationUserId = applicationUserId }, commandType: CommandType.StoredProcedure);
            }

            return blogs.ToList();
        }

        public async Task<List<Blog>> GetAllFamousAsync()
        {
            IEnumerable<Blog> famousBlogs;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                famousBlogs = await connection.QueryAsync<Blog>("Blog_GetAllFamous", commandType: CommandType.StoredProcedure);
            }

            return famousBlogs.ToList();
        }

        public async Task<Blog> GetAsync(int blogId)
        {
            Blog blog;

            using(var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                blog = await connection.QueryFirstOrDefaultAsync<Blog>("Blog_Get", new { BlogId = blogId }, commandType: CommandType.StoredProcedure);
            }

            return blog;
        }

        public async Task<Blog> UpsertAsync(BlogCreate blogCreate, int applicationUserId)
        {
            var dataTable = new DataTable();

            dataTable.Columns.Add("blogId", typeof(int));
            dataTable.Columns.Add("title", typeof(string));
            dataTable.Columns.Add("content", typeof(string));
            dataTable.Columns.Add("photoId", typeof(int));

            dataTable.Rows.Add(
                blogCreate.BlogId,
                blogCreate.Title,
                blogCreate.Content,
                blogCreate.PhotoId
                );

            int? newBlogId;

            using (var connection = new SqlConnection(_connectionString)) {
                await connection.OpenAsync();

                newBlogId = await connection.ExecuteScalarAsync<int?>("Blog_Upsert", new { Blog = dataTable.AsTableValuedParameter("dbo.BlogType"), ApplicationUserId = applicationUserId }, commandType: CommandType.StoredProcedure);
            }

            newBlogId = newBlogId ?? blogCreate.BlogId;

            Blog blog = await GetAsync(newBlogId.Value);

            return blog;
        }
    }
}
