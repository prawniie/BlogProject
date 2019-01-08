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
            var sql = @"SELECT Title, Description, Author, Created, BlogpostId
                        FROM Blogpost
                        WHERE BlogpostId=@Id";

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
                        Created = reader.GetSqlDateTime(3).Value,
                        Id = reader.GetSqlInt32(4).Value
                    };
                    return blogPost;
                }
                return null;
            }
        }

        internal List<Tag> GetTagsFromId(BlogPost blogpost)
        {
            var sql = @"SELECT Tag.Name 
                        FROM Tag
                        FULL JOIN BlogpostTag ON BlogpostTag.TagId = Tag.TagId
                        WHERE BlogpostTag.BlogpostId = @Id";

            List<Tag> blogPostTags = new List<Tag>();

            using (SqlConnection connection = new SqlConnection(conString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                connection.Open();
                command.Parameters.Add(new SqlParameter("Id", blogpost.Id));

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var tag = new Tag();
                    tag.Name = reader.GetSqlString(0).Value;
                    blogPostTags.Add(tag);
                }
                return blogPostTags;
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
            int blogPostId = GetIdOnBlogpost(blogPost);

            int tagId = GetIdOnTag(tag);

            var sql = @"INSERT INTO BlogpostTag(BlogpostId, TagId)
                        VALUES (@Id,@Tag)";

            using (SqlConnection connection = new SqlConnection(conString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                connection.Open();
                command.Parameters.Add(new SqlParameter("Id", blogPostId));
                command.Parameters.Add(new SqlParameter("Tag", tagId));

                command.ExecuteNonQuery();
            }
        }

        internal void DeleteBlogpostInBlogpostTagTable(BlogPost blogpost)
        {
            var sql = "DELETE FROM BlogpostTag WHERE BlogpostId = @Id";

            using (SqlConnection connection = new SqlConnection(conString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                connection.Open();
                command.Parameters.Add(new SqlParameter("Id", blogpost.Id));
                command.ExecuteNonQuery();
            }
        }

        private int GetIdOnTag(Tag tag)
        {

            var sql = @"SELECT TagId FROM Tag
                        WHERE Name = @Name";

            int tagId = 0;

            using (SqlConnection connection = new SqlConnection(conString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                connection.Open();
                command.Parameters.Add(new SqlParameter("Name", tag.Name));

                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    tagId = reader.GetInt32(0);
                    return tagId;
                }

                return tagId;
            }
        }

        private int GetIdOnBlogpost(BlogPost blogPost)
        {
            var sql = @"SELECT BlogpostId FROM Blogpost
                        WHERE Title = @Title";

            int blogPostId = 0;

            using (SqlConnection connection = new SqlConnection(conString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                connection.Open();
                command.Parameters.Add(new SqlParameter("Title", blogPost.Title));

                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    blogPostId = reader.GetInt32(0);
                    return blogPostId;
                }

                return blogPostId;
            }
         }

        internal void CreateTags(BlogPost blogPost)
        {
            CheckIfTagExist(blogPost);

            Console.WriteLine($"Följande taggar har lagts till: {string.Join(',',blogPost.Tags.Select(b => b.Name))}");
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
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Taggen '{tag.Name}' har lagts till!");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Taggen '{tag.Name}' finns redan!");
                    Console.ResetColor();
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
