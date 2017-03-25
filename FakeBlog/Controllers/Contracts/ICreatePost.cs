using FakeBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakeBlog.Controllers.Contracts
{
    interface ICreatePost
    {
        // Authors will be able to make drafts for blog posts
        void CreateDraftPost(ApplicationUser owner, string postTitle, string postContent);
    }
}
