using System;
using System.Collections.Generic;
using System.Linq;
using Code9Insta.API.Core.DTO;
using Code9Insta.API.Infrastructure.Data;
using Code9Insta.API.Infrastructure.Entities;
using Code9Insta.API.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Code9Insta.API.Infrastructure.Repository
{
    public class DataRepository : IDataRepository
    {
        private CodeNineDbContext _context;

        public DataRepository(CodeNineDbContext context)
        {
            _context = context;
        }

        public void CreatePost(Guid userId, CreatePostDto post)
        {
            var profile = _context.Profiles.SingleOrDefault(p => p.UserId == userId);

            var image = new Image();
            image.Data = post.ImageData;
     
            var newPost = new Post
            {
                Image = image,            
                ProfileId = profile.Id,
                CreatedOn = DateTime.UtcNow,
                Description = post.Description,
                PostTags = new List<PostTag>(),

            };

            if(post.Tags != null)
            {
                foreach (var tag in post.Tags)
                {
                    var newTag = new Tag
                    {
                        Text = tag
                    };

                    newPost.PostTags.Add(new PostTag
                    {
                        Post = newPost,
                        Tag = newTag
                    });

                }
            }

            _context.Posts.Add(newPost);


        }

        public void EditPost(Post post, string description, string[] tags)
        {
            post.Description = description;

            //clear removed tags
            var tagsForRemoval = post.PostTags?.Where(pt => tags.All(x => x != pt.Tag.Text)).ToList();

            foreach (var item in tagsForRemoval)
            {
                post.PostTags.Remove(item);
            }

            if(tags != null)
            {
                //add new tags
                foreach (var tag in tags)
                {
                    if (!post.PostTags.Any(t => t.Tag.Text == tag))
                    {
                        var newTag = new Tag
                        {
                            Text = tag
                        };

                        post.PostTags.Add(new PostTag
                        {
                            Post = post,
                            Tag = newTag
                        });
                    }
                }
            }
        }

        public Post GetPostForUser(Guid userId, Guid id)
        {
            return _context.Posts
                .Include(p => p.Image)
                .Include(p => p.UserLikes)
                .Include(p => p.Profile)
                .Include(p => p.Comments)
                .Include(e => e.PostTags)
                   .ThenInclude(e => e.Tag).SingleOrDefault(p => p.Id == id && p.Profile.UserId == userId);
        }

        public IEnumerable<Post> GetPostsForUser(Guid userId)
        {
            return _context.Posts.Where(p => p.Profile.UserId == userId)
                .Include(p => p.Image)
                .Include(p => p.Profile)
                .Include(p => p.Comments)
                .Include(p => p.UserLikes)
                .Include(p => p.PostTags)
                   .ThenInclude(pt => pt.Tag)
                .OrderByDescending(p => p.CreatedOn)
                .ToList();
        }

        public Post GetPostById(Guid id)
        {
            return _context.Posts
                .Include(p => p.Image)
                .Include(p => p.UserLikes)
                .Include(p => p.Profile)
                .Include(p => p.Comments)
                .Include(p => p.PostTags)
                   .ThenInclude(pt => pt.Tag)
                .SingleOrDefault(p => p.Id == id);
        }

        public void DeletePost(Post post)
        {
            _context.Remove(post);
        }

        public void ReactToPost(Post post, Guid userId)
        {
            var like = _context.UserLikes.SingleOrDefault(pl => pl.UserId == userId && pl.PostId == post.Id);
            if (like == null)
            {
                var newLike = new UserLike
                {
                    PostId = post.Id,
                    UserId = userId
                };

                post.UserLikes = post.UserLikes ?? new List<UserLike>();

                post.UserLikes.Add(newLike);

            }
            else
            {
                _context.UserLikes.Remove(like);
            }
        }

        public IEnumerable<Post> GetPosts(string searchString)
        {
            var query = _context.Posts
                .Include(p => p.Image)
                .Include(p => p.Profile)
                .Include(p => p.Comments)
                .Include(p => p.UserLikes)
                .Include(e => e.PostTags)
                   .ThenInclude(e => e.Tag)
                   .OrderByDescending(p => p.CreatedOn)
                   .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(p => p.PostTags.Any(pt => pt.Tag.Text == searchString));
            }

            return query.ToList();

        }

        public IEnumerable<Post> GetPage(int pageNumber, int pageSize, string searchString)
        {
            var query = _context.Posts
               .Include(p => p.Image)
               .Include(p => p.Profile)
               .Include(p => p.Comments)
               .Include(p => p.UserLikes)
               .Include(e => e.PostTags)
                  .ThenInclude(e => e.Tag).AsQueryable()
                  .OrderByDescending(p => p.CreatedOn)
                   .Skip((pageNumber - 1) * pageSize)
                   .Take(pageSize);

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(p => p.PostTags.Any(pt => pt.Tag.Text == searchString));
            }

            return query.ToList();

        }

        public bool UserExists(Guid userId)
        {
            return _context.Users.Any(u => u.Id == userId);
        }

        public bool PostExists(Guid postId)
        {
            return _context.Posts.Any(u => u.Id == postId);
        }

        public void CreateComment(Comment comment)
        {
            _context.Comments.Add(comment);
        }

        public Comment GetCommentById(Guid commentId)
        {
            return _context.Comments.FirstOrDefault(f => f.Id == commentId);
        }

        public void DeleteComment(Comment comment)
        {
            _context.Remove(comment);
        }

        public List<Comment> GetCommentsByPostId(Guid postId)
        {
            return _context.Posts.Where(w => w.Id == postId).SelectMany(s => s.Comments).Include(c => c.Profile).ToList();
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }


    }
}
