using Code9Insta.API.Core.DTO;
using Code9Insta.API.Infrastructure.Entities;
using System;
using System.Collections.Generic;

namespace Code9Insta.API.Infrastructure.Interfaces
{
    public interface IDataRepository
    {
        IEnumerable<Post> GetPosts(string searchString);
        IEnumerable<Post> GetPage(int pageNumber, int pageSize, string searchString);
        bool UserExists(Guid userId);
        bool PostExists(Guid userId);
        void CreatePost(Guid userId, CreatePostDto post);
        void DeletePost(Post post);

        bool Save();
        Post GetPostForUser(Guid userId, Guid id);
        IEnumerable<Post> GetPostsForUser(Guid userId);
        Post GetPostById(Guid id);
        void ReactToPost(Post post, Guid userId);
        void EditPost(Post post, string description, string[] tags);
        void CreateComment(Comment comment);
        void DeleteComment(Comment comment);
        Comment GetCommentById(Guid commentId);
        List<Comment> GetCommentsByPostId(Guid postId);
    }
}
