using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using BlogLab.Models.Blog;
using BlogLab.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BlogLab.Web.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly IBlogRepository _blogRespository;
        private readonly IPhotoRepository _photoRepository;

        public BlogController(
            IBlogRepository blogRepository,
            IPhotoRepository photoRepository
            )
        {
            _blogRespository = blogRepository;
            _photoRepository = photoRepository;
        }

        //http://localhost:5000/api/Blog [POST] with BlogCreate payload and token in header
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Blog>> Create(BlogCreate blogCreate)
        {
            int applicationUserId = int.Parse(User.Claims.First(i => i.Type == JwtRegisteredClaimNames.NameId).Value);

            if (blogCreate.PhotoId.HasValue)
            {
                var photo = await _photoRepository.GetAsync(blogCreate.PhotoId.Value);

                if(photo.ApplicationUserId != applicationUserId)
                {
                    return BadRequest("You did not upload the photo!");
                }
            }

            var blog = await _blogRespository.UpsertAsync(blogCreate, applicationUserId);

            return Ok(blog);
        }

        // http://localhost:5000/api/Blog?Page=1&PageSize=10 [GET] from query string
        [HttpGet]
        public async Task<ActionResult<PagedResults<Blog>>> GetAll([FromQuery] BlogPaging blogPaging) {
            var blogs = await _blogRespository.GetAllAsync(blogPaging);
            return Ok(blogs);
        }

        // http://localhost:5000/api/Blog/1
        [HttpGet("{blogId}")]
        public async Task<ActionResult<Blog>> Get(int blogId)
        {
            var blog = _blogRespository.GetAsync(blogId);

            return Ok(blog);
        }

        // http://localhost:5000/api/Blog/user/9
        [HttpGet("user/{applicationUserId}")]
        public async Task<ActionResult<List<Blog>>> GetByApplicationUserId(int applicationUserId)
        {
            var blogs = await _blogRespository.GetAllByUserIdAsync(applicationUserId);
            return Ok(blogs);
        }

        // http://localhost:5000/api/Blog/famous
        [HttpGet("famous")]
        public async Task<ActionResult<List<Blog>>> GetAllFamous()
        {
            var famousBlogs = await _blogRespository.GetAllFamousAsync();
            return Ok(famousBlogs);
        }

        [Authorize]
        [HttpDelete("{blogId}")]
        public async Task<ActionResult<int>> Delete(int blogId)
        {
            int applicationUserId = int.Parse(User.Claims.First(i => i.Type == JwtRegisteredClaimNames.NameId).Value);

            var foundBlog = await _blogRespository.GetAsync(blogId);
            if (foundBlog == null) return BadRequest("Blog does not exist!");

            if(foundBlog.ApplicationUserId != applicationUserId) return BadRequest("You are not authorized to remove this Blog, since you did not create this blog!");

            var affectedRows = await _blogRespository.DeleteAsync(foundBlog.BlogId);

            return Ok(affectedRows);

        }

    }
}
