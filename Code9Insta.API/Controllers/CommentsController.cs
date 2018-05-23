using Code9Insta.API.Infrastructure.Entities;
using Code9Insta.API.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Code9Insta.API.Helpers;
using System;
using System.Collections.Generic;
using Code9Insta.API.Core.DTO;

namespace Code9Insta.API.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Produces("application/json")]
    [Route("api/Comments")]
    public class CommentsController : Controller
    {
        private IDataRepository _dataRepository;
        private IProfileRepository _profileRepository;
        public Func<string> _getUserId; //For testing

        public CommentsController(IDataRepository dataRepository, IProfileRepository profileRepository)
        {
            _dataRepository = dataRepository;
            _profileRepository = profileRepository;
            _getUserId = () => HttpContext.User.GetUserId();
        }

        [HttpPost]
        public IActionResult CreateComment(Guid postId, string text)
        {
            if (!_dataRepository.PostExists(postId))
            {
                return BadRequest("Post does not exist");
            }

            var userId = Guid.Parse(_getUserId.Invoke());

            var userProfileId = _profileRepository.GetProfileIdByUserId(userId);

            if (userProfileId == null)
            {
                return BadRequest($"Profile for user with userId: {userId}  does not exist");
            }

            var comment = new Comment
            {
                PostId =  postId,
                ProfileId = userProfileId.Value,
                CreatedOn = DateTime.Now,
                Text = text
            };

            _dataRepository.CreateComment(comment);

            if(!_dataRepository.Save())
            {
                return StatusCode(500, "There was a problem while handling your request.");
            }

            return StatusCode(200, "Comment created");

        }

        [HttpDelete]
        public IActionResult DeleteComment(Guid commentId)
        {
            var comment = _dataRepository.GetCommentById(commentId);

            if (comment == null)
            {
                return BadRequest("Comment does not exist");
            }

            _dataRepository.DeleteComment(comment);

            if (!_dataRepository.Save())
            {
                return StatusCode(500, "There was a problem while handling your request.");
            }

            return StatusCode(200, "Comment deleted");
        }

        [Route("GetPostComments")]
        [HttpGet]
        public IActionResult GetCommentsByPostId(Guid postId)
        {
            var comments = new List<Comment>();
            comments = _dataRepository.GetCommentsByPostId(postId);

            var commentsDto = AutoMapper.Mapper.Map<List<GetCommentDto>>(comments);

            return Ok(commentsDto);
        }
    }
}