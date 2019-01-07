using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bloggy.Domain;

namespace Bloggy
{
    public class DataAccess
    {
        private string conString = "Server=(localdb)\\mssqllocaldb; Database=Blogposts";

        internal List<BlogPost> GetAllBlogPostsBrief()
        {

            var sql = @"SELECT BlogpostId, Author, Title
                        FROM BlogPostNEW2";

            using (SqlConnection connection = new SqlConnection(conString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                var list = new List<BlogPost>();

                while (reader.Read())
                {
                    var bp = new BlogPost
                    {
                        Id = reader.GetSqlInt32(0).Value,
                        Author = reader.GetSqlString(1).Value,
                        Title = reader.GetSqlString(2).Value
                    };
                    list.Add(bp);
                }

                return list;
            }

            //return new List<BlogPost>
            //{
            //new BlogPost
            //    {
            //        Id = 1,
            //        Author = "Ali",
            //        Title = "Måndag igen"
            //    },

            //    new BlogPost
            //    {
            //        Id = 2,
            //        Author = "Sue",
            //        Title = "Inga julklappar"
            //    },

            //    new BlogPost
            //    {
            //        Id = 3,
            //        Author = "Markus",
            //        Title = "Lunch"
            //    }
            //};

        }

        internal void UpdateBlogpost(BlogPost blogpost)
        {
            var sql = @"UPDATE Blogpost
                        SET Blogpost.Title = @Title
                        WHERE BlogpostId = @Id";

            using (SqlConnection connection = new SqlConnection(conString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                connection.Open();
                command.Parameters.Add(new SqlParameter("Title", blogpost.Title));
                command.Parameters.Add(new SqlParameter("Id", blogpost.Id));

                SqlDataReader reader = command.ExecuteReader();

            }
        }

        internal BlogPost GetPostById(int postId)
        {
            var sql = @"SELECT BlogpostId, Author, Title
                        FROM BlogPostNEW2 WHERE BlogpostId=@Id";

            using (SqlConnection connection = new SqlConnection(conString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                connection.Open();
                command.Parameters.Add(new SqlParameter("Id", postId));

                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    var blogPost = new BlogPost
                    {
                        Id = reader.GetSqlInt32(0).Value,
                        Author = reader.GetSqlString(1).Value,
                        Title = reader.GetSqlString(2).Value
                    };
                    return blogPost;
                }

                return null;

            }
        }
    }
}
