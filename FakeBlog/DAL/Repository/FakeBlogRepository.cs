using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FakeBlog.Models;
using FakeBlog.Controllers.Contracts;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace FakeBlog.DAL.Repository
{
    public class FakeBlogRepository : ICreatePost, IDeletePost, IEditDraftStatus, IEditPost, IPostQuery
    {
        IDbConnection _blogConnection;

        public FakeBlogRepository(IDbConnection blogConnection)
        {
            _blogConnection = blogConnection;
        }

        public void CreateDraftPost(string name, string content, ApplicationUser owner)
        {
            _blogConnection.Open();

            try
            {
                var addPostCommand = _blogConnection.CreateCommand();
                addPostCommand.CommandText = "Insert into Post(PostTitle,Owner_Id, PostContent) values(@name,@ownerId)";

                var postTitleParameter = new SqlParameter("postTitle", SqlDbType.VarChar);
                postTitleParameter.Value = name;
                addPostCommand.Parameters.Add(postTitleParameter);

                var ownerParameter = new SqlParameter("owner", SqlDbType.Int);
                ownerParameter.Value = owner.Id;
                addPostCommand.Parameters.Add(ownerParameter);

                var postContentParameter = new SqlParameter("postContent", SqlDbType.VarChar);
                postContentParameter.Value = content;
                addPostCommand.Parameters.Add(postContentParameter);

                addPostCommand.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
            }
            finally
            {
                _blogConnection.Close();
            }
        }

        public bool DeletePost(int postId)
        {
            _blogConnection.Open();

            try
            {
                var removePostCommand = _blogConnection.CreateCommand();
                removePostCommand.CommandText = @"Delete From Post Where PostId = @postId";

                var postTitleParameter = new SqlParameter("postId", SqlDbType.Int);
                postTitleParameter.Value = postId;
                removePostCommand.Parameters.Add(postTitleParameter);

                removePostCommand.ExecuteNonQuery();
            }

            catch (SqlException ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
            }

            finally
            {
                _blogConnection.Close();
            }

            return false;
        }

        public void EditPostTitle(int postId, string editedTitle)
        {
            _blogConnection.Open();

            try
            {
                var editPostTitleCommand = _blogConnection.CreateCommand();
                editPostTitleCommand.CommandText = @"
                    Update Post 
                    Set Title = @EditedTitle
                    Where postId = @postId";
                var titleParameter = new SqlParameter("EditedTitle", SqlDbType.VarChar);
                titleParameter.Value = editedTitle;
                editPostTitleCommand.Parameters.Add(titleParameter);

                editPostTitleCommand.ExecuteNonQuery();
            }

            catch (SqlException ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
            }

            finally
            {
                _blogConnection.Close();
            }
        }

        public void EditPostContent(int postId, string editedContent)
        {
            _blogConnection.Open();

            try
            {
                var editPostContentCommand = _blogConnection.CreateCommand();
                editPostContentCommand.CommandText = @"
                    Update Post 
                    Set Content = @EditedContent
                    Where postId = @postId";
                var contentParameter = new SqlParameter("EditedContent", SqlDbType.VarChar);
                contentParameter.Value = editedContent;
                editPostContentCommand.Parameters.Add(contentParameter);

                editPostContentCommand.ExecuteNonQuery();
            }

            catch (SqlException ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
            }

            finally
            {
                _blogConnection.Close();
            }
        }

        public List<Post> GetPosts(string userId)
        {
            _blogConnection.Open();

            try
            {
                var getPostCommand = _blogConnection.CreateCommand();
                getPostCommand.CommandText = @"
                    SELECT PostId, PostTitle, PostContent, PostIsDraft, Owner_Id 
                    FROM Post 
                    WHERE Owner_Id = @userId";
                var postIdParam = new SqlParameter("userId", SqlDbType.VarChar);
                postIdParam.Value = userId;
                getPostCommand.Parameters.Add(postIdParam);

                var reader = getPostCommand.ExecuteReader();

                var posts = new List<Post>();
                while (reader.Read())
                {
                    var post = new Post
                    {
                        PostID = reader.GetInt32(0),
                        PostTitle = reader.GetString(1),
                        PostContent = reader.GetString(2),
                        PostIsDraft = reader.GetBoolean(3),
                        User = new ApplicationUser { Id = reader.GetString(4) }
                    };

                    posts.Add(post);
                }

                return posts;
            }

            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
            }

            finally
            {
                _blogConnection.Close();
            }

            return new List<Post>();
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