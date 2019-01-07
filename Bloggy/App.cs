using Bloggy.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloggy
{
    public class App

    {
        DataAccess dataAccess = new DataAccess();
        //Konvention egentligen att göra _dataAccess när det är privata fält

        public void Run()
        {
            PageMainMenu();
        }

        private void PageMainMenu()
        {
            Header("Huvudmeny");

            ShowAllBlogPostsBrief();

            WriteLine("Vad vill du göra?");
            WriteLine("a) Gå till huvudmenyn");
            WriteLine("b) Uppdatera en bloggpost");
            WriteLine("c) Radera en bloggpost");
            WriteLine("d) Skapa ny bloggpost");

            ConsoleKey command = Console.ReadKey(true).Key; //true gör att värdet inte skrivs ut på skärmen

            if (command == ConsoleKey.A)
                PageMainMenu();

            if (command == ConsoleKey.B)
                PageUpdatePost();

            if (command == ConsoleKey.C)
                PageDeletePost();

            if (command == ConsoleKey.D)
                PageCreatePost();
        }

        private void PageCreatePost()
        {
            BlogPost blogPost = new BlogPost();

            Header("Ny blogpost");

            Console.Write("Författare: ");
            blogPost.Author = Console.ReadLine();

            Console.Write("Titel: ");
            blogPost.Title = Console.ReadLine();

            WriteLine("\nSkriv text här:");
            blogPost.Description = Console.ReadLine();

            blogPost.Created = DateTime.Now;



            dataAccess.CreateBlogpost(blogPost);

            WriteLine("Bloggposten är skapad!");
            Console.ReadKey();
            PageMainMenu();
        }

        private void PageDeletePost()
        {
            Header("Radera");

            ShowAllBlogPostsBrief();

            Console.Write("Vilken bloggpost vill du radera? ");
            int postId = int.Parse(Console.ReadLine());

            BlogPost blogpost = dataAccess.GetPostById(postId);

            Console.WriteLine($"Den bloggpost du vill radera är alltså '{blogpost.Title}'");

            dataAccess.DeleteBlogpost(blogpost);
            Console.WriteLine("Bloggposten är raderad.");
            Console.ReadKey();
            PageMainMenu();

        }

        private void PageUpdatePost()
        {
            Header("Uppdatera");

            ShowAllBlogPostsBrief();

            Console.Write("Vilken bloggpost vill du uppdatera? ");
            int postId = int.Parse(Console.ReadLine());

            BlogPost blogpost = dataAccess.GetPostById(postId);

            Console.WriteLine($"Den nuvarande titeln är {blogpost.Title}");

            Console.Write("Skriv in ny titel: ");

            string newTitle = Console.ReadLine();

            blogpost.Title = newTitle;

            dataAccess.UpdateBlogpost(blogpost);

            Console.WriteLine("Bloggposten uppdaterad.");
            Console.ReadKey();
            PageMainMenu();
            //string input = Console.ReadLine();
            //int.TryParse(input, out int blogId);

            //foreach (var blogPost in list)
            //{
            //    if (blogPost.Id == blogId)
            //    {
            //        Console.Write("Skriv in ny titel: ");
            //        string newTitel = Console.ReadLine();
            //        blogPost.Title = newTitel;
            //        break;
            //    }
            //    else
            //    {
            //        Console.WriteLine("Kan inte hitta en blogpost med detta id");
            //    }
            //}
        }

        private void ShowAllBlogPostsBrief()
        {

            List<BlogPost> list = dataAccess.GetAllBlogPostsBrief();

            PrintBlogPosts(list);

        }

        private void PrintBlogPosts(List<BlogPost> list)
        {
            foreach (var blogPost in list)
            {
                //WriteLine($"{blogPost.Id.ToString().PadRight(5)}{blogPost.Title.ToString().PadRight(40)}{blogPost.Author}");
                Console.Write(blogPost.Id.ToString().PadRight(5));
                Console.Write(blogPost.Title.ToString().PadRight(40));
                Console.Write(blogPost.Author);
                Console.WriteLine();
            }
                Console.WriteLine();
        }

        private void WriteLine(string text="") //Om man inte skickar in ngn text blir den automatisk tom
        {
            Console.WriteLine(text);
        }

        private void Header(string text)
        {
            Console.Clear();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(text.ToUpper());
            Console.ResetColor();
            Console.WriteLine();
        }
    }
}
