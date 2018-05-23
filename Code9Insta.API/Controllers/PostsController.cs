using System;
using Code9Insta.API.Infrastructure.Interfaces;
using System.IO;
using System.Threading.Tasks;
using Code9Insta.API.Core.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Code9Insta.API.Helpers;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Code9Insta.API.Infrastructure.Entities;

namespace Code9Insta.API.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Produces("application/json")]
    [Route("api/Posts")]
    public class PostsController : Controller
    {
        private IDataRepository _repository;

        public PostsController(IDataRepository repository)
        {
            _repository = repository;
        }
        // GET: api/Posts
        [HttpGet]
        [Route("All")]
        public IActionResult GetAllPosts(string searchString)
        {
            var userId = Guid.Parse(HttpContext.User.GetUserId());

            var posts = _repository.GetPosts(searchString).ToList();

            var result = new List<PostDto>();

            foreach(var item in posts)
            {
                var dto = AutoMapper.Mapper.Map<Post, PostDto>(item, opt =>
                 opt.AfterMap((src, dest) => dest.IsLikedByUser = src.UserLikes.Any(x => x.UserId == userId)));

                result.Add(dto);
            }

            
            return Ok(result);
        }

        [HttpGet]
        public IActionResult Get(int pageNumber = 1, int pageSize = 10, string searchString = null)
        {
            var posts = _repository.GetPage(pageNumber, pageSize, searchString);
            var userId = Guid.Parse(HttpContext.User.GetUserId());

            var result = new List<PostDto>();

            foreach (var item in posts)
            {
                var dto = AutoMapper.Mapper.Map<Post, PostDto>(item, opt =>
                 opt.AfterMap((src, dest) => dest.IsLikedByUser = src.UserLikes.Any(x => x.UserId == userId)));

                result.Add(dto);
            }

            return Ok(result);
        }

        // GET: api/Posts/5
        [HttpGet("{id}", Name = "GetPost")]
        public IActionResult Get(Guid id)
        {
            var post = _repository.GetPostById(id);
            if (post == null)
            {
                return NotFound();
            }

            var userId = Guid.Parse(HttpContext.User.GetUserId());
            var postDto = AutoMapper.Mapper.Map<PostDto>(post);

            if(post.UserLikes.SingleOrDefault(ul => ul.UserId == userId) != null)
            {
                postDto.IsLikedByUser = true;
            }

            return Ok(postDto);
        }
        
        // POST: api/Posts
        [HttpPost]
        public IActionResult Post([FromBody]CreatePostDto model)
        {
            if(model == null)
            {
                return BadRequest();
            }                

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = Guid.Parse(HttpContext.User.GetUserId());
                 
            _repository.CreatePost(userId, model);

            if(!_repository.Save())
            {
                return StatusCode(500, "A problem happened with handling your request.");
            }


            return StatusCode(200, "Post created");

        }

        [HttpPut("{id}", Name = "EditPost")]
        [Route("{id}")]
        public IActionResult Edit([FromBody]EditPostDto model, Guid id)
        {
            if (model == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = Guid.Parse(HttpContext.User.GetUserId());

            var post = _repository.GetPostForUser(userId, id);
            if (post == null)
            {
                return NotFound();
            }
            else
            {

                _repository.EditPost(post, model.Description, model.Tags);

            }
            if (!_repository.Save())
            {
                throw new Exception($"Updating post likes {id} failed on save.");
            }

            return NoContent();
        }

        // PUT: api/Posts/5
        [HttpPut("{id}/reactToPost", Name ="ReactToPost")]
        [Route("{id}/reactToPost")]
        public IActionResult Put(Guid id)
        {
            var post = _repository.GetPostById(id);
            if (post == null)
            {
                return NotFound();
            }
            else
            {
                var userId = Guid.Parse(HttpContext.User.GetUserId());
                _repository.ReactToPost(post, userId);

            }
            if (!_repository.Save())
            {
                throw new Exception($"Updating post likes {id} failed on save.");
            }

            return NoContent();
        }

       
        // DELETE: api/Posts/5
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var userId = Guid.Parse(HttpContext.User.GetUserId());

            var post = _repository.GetPostForUser(userId, id);
            if (post == null)
            {
                return NotFound();
            }

            _repository.DeletePost(post);

            if (!_repository.Save())
            {
                throw new Exception($"Deleting post {id} for user {userId} failed on save.");
            }

           
            return NoContent();
        }
    }
}
