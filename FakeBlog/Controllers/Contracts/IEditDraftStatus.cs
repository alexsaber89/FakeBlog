using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakeBlog.Controllers.Contracts
{
    interface IEditDraftStatus
    {
        // Authors will be able to manually publish a draft.
        bool PublishDraftPost(int postId);

        // Authors will be able to unpublish an existing published post
        bool UnpublishPost(int postId);
    }
}
