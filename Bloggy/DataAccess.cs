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
                        FROM Blogpost";

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

        internal BlogPost GetFullPostById(int postId)
        {
            var sql = @"SELECT Blogpost.Title, Blogpost.Description, Blogpost.Author, Blogpost.Created, Tag.Name
                        FROM Blogpost
                        FULL JOIN BlogpostTag ON Blogpost.BlogpostId = BlogpostTag.BlogpostId
                        FULL JOIN Tag ON BlogpostTag.TagId = Tag.TagId
                        WHERE Blogpost.BlogpostId=@Id";

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
                        Title = reader.GetSqlString(0).Value,
                        Description = reader.GetSqlString(1).Value,
                        Author = reader.GetSqlString(2).Value,
                        Created = reader.GetSqlDateTime(3).Value
                        //Tags = reader.GetSqlString(2).Value.ToList()
                    };
                    return blogPost;
                }

                return null;
            }
        }

        internal void CreateBlogpost(BlogPost blogPost)
        {
            var sql = @"INSERT INTO Blogpost(Title,Description,Author,Created)
                        VALUES(@Title,@Description,@Author,@Created) ";

            using (SqlConnection connection = new SqlConnection(conString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                connection.Open();
                command.Parameters.Add(new SqlParameter("Title", blogPost.Title));
                command.Parameters.Add(new SqlParameter("Description", blogPost.Description));
                command.Parameters.Add(new SqlParameter("Author", blogPost.Author));
                command.Parameters.Add(new SqlParameter("Created", blogPost.Created));

                command.ExecuteNonQuery();
            }

        }

        internal void AddToBlogpostTagTable(Tag tag, BlogPost blogPost)
        {

            //int blogPostId = GetIdOnBlogpost(blogPost);

            //Anv sql:
            //select blogPostId from Blogpost where Name == blogPost.Name
            //tex select blogPostId from Blogpost where Title == 'Morgon'

            //int tagId = GetIdOnTag(tag);

            //Anv sql: 
            //select TagId from Tag where Name == tag.Name
            //tex select TagId from Tag where Name = 'Hundar'

            var sql = @"INSERT INTO BlogpostTag(BlogpostId, TagId)
                        VALUES (@Id,@Tag)";

            using (SqlConnection connection = new SqlConnection(conString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                connection.Open();
                command.Parameters.Add(new SqlParameter("Id", blogPost.Id)); //Just nu är id = 0, måste hämta från db
                command.Parameters.Add(new SqlParameter("Tag", tag.Id)); //just nu är id = 0, måste hämta från db

                command.ExecuteNonQuery();
            }
        }

        internal void CreateTags(BlogPost blogPost)
        {
            CheckIfTagExist(blogPost);
        }

        internal void CreateTags(Tag tag)
        {
            CheckIfTagExist(tag);
        }

        private void CheckIfTagExist(Tag tag)
        {
            var sql = @"SELECT Name
                        FROM Tag"; //Får alla tagnamnen i tabellen Tag

            List<Tag> tagsInDatabase = new List<Tag>(); //Sparar alla taggnamnen i en lista av taggar

            using (SqlConnection connection = new SqlConnection(conString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var tagDB = new Tag
                    {
                        Name = reader.GetSqlString(0).Value,
                    };
                    tagsInDatabase.Add(tagDB);
                }

                if(!tagsInDatabase.Select(t=> t.Name).Contains(tag.Name))
                {
                    AddTagToDatabase(tag.Name.ToString());
                }
            }
        }

        private void CheckIfTagExist(BlogPost blogPost)
        {
            var sql = @"SELECT Name
                        FROM Tag"; //Får alla tagnamnen i tabellen Tag

            List<Tag> tagsInDatabase = new List<Tag>(); //Sparar alla taggnamnen i en lista av taggar

            using (SqlConnection connection = new SqlConnection(conString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var tag = new Tag
                    {
                        Name = reader.GetSqlString(0).Value,
                    };
                    tagsInDatabase.Add(tag);
                }

                foreach (var tag in blogPost.Tags)
                {
                    if (tagsInDatabase.Select(t=>t.Name).Contains(tag.Name))
                    {
                        continue;
                    }

                    AddTagToDatabase(tag.Name.ToString());
                    AddToBlogpostTagTable(tag, blogPost);

                }
            }
        }

        private void AddTagToDatabase(string tagName)
        {
            var sql = @"INSERT INTO Tag(Name)
                        VALUES (@tagName)";

            using (SqlConnection connection = new SqlConnection(conString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                connection.Open();
                command.Parameters.Add(new SqlParameter("tagName", tagName));
                command.ExecuteNonQuery();
            }
        }

        internal void DeleteBlogpost(BlogPost blogpost)
        {
            var sql = @"DELETE FROM Blogpost
                        WHERE BlogpostId = @Id";

            using (SqlConnection connection = new SqlConnection(conString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                connection.Open();
                command.Parameters.Add(new SqlParameter("Id", blogpost.Id));
                command.ExecuteNonQuery();
            }

        }

        internal void UpdateBlogpost(BlogPost blogpost)
        {
            var sql = @"UPDATE Blogpost
                        SET Title = @Title
                        WHERE BlogpostId = @Id";

            using (SqlConnection connection = new SqlConnection(conString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                connection.Open();
                command.Parameters.Add(new SqlParameter("Title", blogpost.Title));
                command.Parameters.Add(new SqlParameter("Id", blogpost.Id));
                command.ExecuteNonQuery();
            }
        }

        internal BlogPost GetPostById(int postId)
        {
            var sql = @"SELECT BlogpostId, Author, Title
                        FROM Blogpost WHERE BlogpostId=@Id";

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
