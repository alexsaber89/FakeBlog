using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakeBlog.Controllers.Contracts
{
    interface IDeletePost
    {
        // Authors will be able to delete published posts and drafts
        bool DeletePost(int postId);
    }
}
