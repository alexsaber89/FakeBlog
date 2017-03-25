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
                addPostCommand.CommandText = @"
                    INSERT INTO Posts(Title,AuthorId,Content)
                    VALUES(@name,@ownerId,@content)";

                var postTitleParameter = new SqlParameter("title", SqlDbType.VarChar);
                postTitleParameter.Value = name;
                addPostCommand.Parameters.Add(postTitleParameter);

                var postContentParameter = new SqlParameter("content", SqlDbType.VarChar);
                postContentParameter.Value = content;
                addPostCommand.Parameters.Add(postContentParameter);

                var authorParameter = new SqlParameter("authorId", SqlDbType.Int);
                authorParameter.Value = owner.Id;
                addPostCommand.Parameters.Add(authorParameter);

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
            try
            {
                var removePostCommand = _blogConnection.CreateCommand();
                removePostCommand.CommandText = @"
                    DELETE
                    FROM Post
                    WHERE PostId = @postId";

                var postTitleParameter = new SqlParameter("postId", SqlDbType.Int);
                postTitleParameter.Value = postId;
                removePostCommand.Parameters.Add(postTitleParameter);

                _blogConnection.Open();
                var rowsAffected = removePostCommand.ExecuteNonQuery();

                if (rowsAffected != 1)
                {
                    throw new Exception($"Query didn't work.  {rowsAffected} rows were affected.");
                }
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
            try
            {
                var editPostContentCommand = _blogConnection.CreateCommand();
                editPostContentCommand.CommandText = @"
                    UPDATE Post 
                    SET Content = @EditedContent
                    WHERE PostId = @id";

                var contentParameter = new SqlParameter("EditedContent", SqlDbType.VarChar);
                contentParameter.Value = editedContent;
                editPostContentCommand.Parameters.Add(contentParameter);

                var idParam = new SqlParameter("id", SqlDbType.Int);
                idParam.Value = postId;
                editPostContentCommand.Parameters.Add(idParam);

                _blogConnection.Open();
                var rowsAffected = editPostContentCommand.ExecuteNonQuery();

                if (rowsAffected != 1)
                {
                    throw new Exception("Query didn't work");
                }
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
            try
            {
                var getPostCommand = _blogConnection.CreateCommand();
                getPostCommand.CommandText = @"
                    UPDATE Posts
                    SET IsDraft = 0
                    WHERE PostId = @id";

                var postIdParameter = new SqlParameter("postId", SqlDbType.Bit);
                postIdParameter.Value = postId;
                getPostCommand.Parameters.Add(postIdParameter);

                _blogConnection.Open();
                var matchingPosts = getPostCommand.ExecuteReader();

                return true;
            }

            finally
            {
                _blogConnection.Close();
            }
        }

        public bool UnpublishPost(int postId)
        {
            try
            {
                var getPostCommand = _blogConnection.CreateCommand();
                getPostCommand.CommandText = @"
                    UPDATE Posts
                    SET IsDraft = 1
                    WHERE PostId = @id";

                var postIdParameter = new SqlParameter("postId", SqlDbType.Bit);
                postIdParameter.Value = postId;
                getPostCommand.Parameters.Add(postIdParameter);

                _blogConnection.Open();
                var matchingPosts = getPostCommand.ExecuteReader();

                return true;
            }

            finally
            {
                _blogConnection.Close();
            }
        }

        public Post GetPost(int postId)
        {
            try
            {
                var getPostCommand = _blogConnection.CreateCommand();
                getPostCommand.CommandText = @"
                    SELECT PostId,IsDraft,Title,Contents,AuthorId
                    FROM Posts
                    WHERE PostId = @postId";

                var postIdParameter = new SqlParameter("postId", SqlDbType.Int);
                postIdParameter.Value = postId;
                getPostCommand.Parameters.Add(postIdParameter);

                _blogConnection.Open();
                var matchingPosts = getPostCommand.ExecuteReader();

                if (matchingPosts.Read())
                {
                    return new Post
                    {
                        PostID = matchingPosts.GetInt32(0),
                        PostIsDraft = matchingPosts.GetBoolean(1),
                        PostTitle = matchingPosts.GetString(2),
                        PostContent = matchingPosts.GetString(3),
                        User = new ApplicationUser { Id = matchingPosts.GetString(4) }
                    };
                }
            }

            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
            }

            finally
            {
                _blogConnection.Close();
            }

            return null;
        }
    }
}