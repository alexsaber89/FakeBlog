using FakeBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakeBlog.Controllers.Contracts
{
    public interface IPostQuery
    {
        // Published posts will be viewable by everyone
        List<Post> GetPosts(string userId);
    }
}
