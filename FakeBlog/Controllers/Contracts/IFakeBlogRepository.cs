using FakeBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakeBlog.Controllers.Contracts
{
    public interface IFakeBlogRepository
    {
        // Published posts will be viewable by everyone
        List<Post> GetPosts(ApplicationUser owner);

        // Authors will be able to make drafts for blog posts
        void CreateDraftPost(ApplicationUser owner, string postTitle, string postContent);

        // Authors will be able to manually publish a draft.
        bool PublishDraftPost(int postId);

        // Authors will be able to unpublish an existing published post
        bool UnpublishPost(int postId);

        // Authors will be able to delete published posts and drafts
        bool DeletePost(int postId);

        // Authors will be able to edit a draft's Title
        bool EditPostTitle(int postId, string editedTitle);

        // Authors will be able to edit a draft's Content
        bool EditPostContent(int postId, string editedContent);
    }
}
