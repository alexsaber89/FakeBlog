using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakeBlog.Controllers.Contracts
{
    interface IEditPost
    {
        // Authors will be able to edit a draft's Title
        void EditPostTitle(int postId, string editedTitle);

        // Authors will be able to edit a draft's Content
        void EditPostContent(int postId, string editedContent);
    }
}
