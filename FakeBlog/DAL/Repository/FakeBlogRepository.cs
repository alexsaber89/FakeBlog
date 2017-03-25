using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FakeBlog.Models;
using FakeBlog.Controllers.Contracts;

namespace FakeBlog.DAL.Repository
{
    public class FakeBlogRepository : ICreatePost, IDeletePost, IEditDraftStatus, IEditPost, IPostQuery
    {
        public FakeBlogContext Context { get; set; }

        public FakeBlogRepository()
        {
            Context = new FakeBlogContext();
        }

        public FakeBlogRepository(FakeBlogContext context)
        {
            Context = context;
        }

        public void CreateDraftPost(ApplicationUser owner, string postTitle, string postContent)
        {
            Post post = new Post { User = owner, PostTitle = postTitle, PostContent = postContent, PostIsDraft = true };
            Context.Posts.Add(post);
            Context.SaveChanges();
        }

        public bool DeletePost(int postId)
        {
            Post postToDelete = Context.Posts.FirstOrDefault(post => post.PostID == postId);
            if (postToDelete != null)
            {
                Context.Posts.Remove(postToDelete);
                Context.SaveChanges();
                // Return true if post exists
                return true;
            }
            return false;
        }

        public bool EditPostTitle(int postId, string editedTitle)
        {
            Post postTitleToEdit = Context.Posts.FirstOrDefault(post => post.PostID == postId);
            if (postTitleToEdit != null)
            {
                postTitleToEdit.PostTitle = editedTitle;
                Context.SaveChanges();
                // Return true if post exists
                return true;
            }
            return false;
        }

        public bool EditPostContent(int postId, string editedContent)
        {
            Post postContentToEdit = Context.Posts.FirstOrDefault(post => post.PostID == postId);
            if (postContentToEdit != null)
            {
                postContentToEdit.PostContent = editedContent;
                Context.SaveChanges();
                // Return true if post exists
                return true;
            }
            return false;
        }

        public List<Post> GetPosts(ApplicationUser owner)
        {
            List<Post> posts = Context.Posts.Where(post => post.User.Id == owner.Id).ToList();
            return posts;
        }

        public bool PublishDraftPost(int postId)
        {
            Post postToPublish = Context.Posts.FirstOrDefault(post => post.PostID == postId);
            if (postToPublish != null)
            {
                postToPublish.PostIsDraft = false;
                Context.SaveChanges();
                // Return true if post exists
                return true;
            }
            return false;
        }

        public bool UnpublishPost(int postId)
        {
            Post postToUnpublish = Context.Posts.FirstOrDefault(post => post.PostID == postId);
            if (postToUnpublish != null)
            {
                postToUnpublish.PostIsDraft = true;
                Context.SaveChanges();
                // Return true if post exists
                return true;
            }
            return false;
        }
    }
}