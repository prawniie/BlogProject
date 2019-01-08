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

            //ShowAllBlogPostsBrief();

            WriteLine("a) Skapa nytt blogginlägg");
            WriteLine("b) Läs blogginlägg");
            WriteLine("c) Uppdatera blogginlägg");
            WriteLine("d) Radera blogginlägg");
            WriteLine("e) Lägg till taggar");

            ConsoleKey command = Console.ReadKey(true).Key; //true gör att värdet inte skrivs ut på skärmen

            if (command == ConsoleKey.A)
                PageCreatePost();

            if (command == ConsoleKey.B)
                PageReadPost();

            if (command == ConsoleKey.C)
                PageUpdatePost();

            if (command == ConsoleKey.D)
                PageDeletePost();

            if (command == ConsoleKey.E)
                PageCreateTags();

        }

        private void PageCreateTags()
        {
            Header("Skapa taggar");

            while (true)
            {
                Console.Write("Skriv in tagg: ");
                string input = Console.ReadLine();

                if (input.Trim() == "")
                {
                    break;
                }

                var tag = new Tag();
                tag.Name = input;

                dataAccess.CreateTags(tag);
            }
            WriteLine("\nTryck på valfri knapp för att gå tillbaka till huvudmenyn");
            Console.ReadKey();
            PageMainMenu();
        }

        private void PageReadPost()
        {
            Header("Blogginlägg");

            ShowAllBlogPostsBrief();

            Console.Write("Välj inlägg att läsa: ");
            int postId = int.Parse(Console.ReadLine());
            BlogPost blogpost = dataAccess.GetFullPostById(postId);

            Console.Clear();
            Header($"Titel: {blogpost.Title}");
            WriteLine("__________________________________________________________________");
            WriteLine($"\n{blogpost.Description}");
            WriteLine("__________________________________________________________________");
            WriteLine($"\nSkriven av {blogpost.Author}");
            WriteLine($"Skapad den {blogpost.Created}");

            List<Tag> blogPostTags = dataAccess.GetTagsFromId(blogpost);
            Console.WriteLine($"Taggar: {string.Join(',', blogPostTags.Select(b => b.Name))}");

            Console.WriteLine("\nSkriv en kommentar: ");
            //CreateComment();

            WriteLine("\nTryck på valfri knapp för att gå tillbaka till huvudmenyn");
            Console.ReadKey();
            PageMainMenu();
        }

        private void PageCreatePost()
        {
            BlogPost blogPost = new BlogPost();

            Header("Nytt blogginlägg");

            Console.Write("Författare: ");
            blogPost.Author = Console.ReadLine();

            Console.Write("Titel: ");
            blogPost.Title = Console.ReadLine();

            WriteLine("\nSkriv text här:");
            blogPost.Description = Console.ReadLine();

            blogPost.Created = DateTime.Now;

            dataAccess.CreateBlogpost(blogPost);

            WriteLine("\nLägg till taggar:");
            while (true)
            {
                string input = Console.ReadLine();

                if (input.Trim() == "")
                    break;

                Tag tag = new Tag();
                tag.Name = input;
                blogPost.Tags.Add(tag);
            }

            dataAccess.CreateTags(blogPost);

            WriteGreen("Blogginlägget är skapat!");
            WriteLine("\nTryck på valfri knapp för att gå tillbaka till huvudmenyn");
            Console.ReadKey();
            PageMainMenu();
        }

        private void PageDeletePost()
        {
            Header("Radera");

            ShowAllBlogPostsBrief();

            Console.Write("Vilket blogginlägg vill du radera? ");
            int postId = int.Parse(Console.ReadLine());

            BlogPost blogpost = dataAccess.GetPostById(postId);

            Console.WriteLine($"Det blogginlägg du vill radera är alltså '{blogpost.Title}'?");
            string input = Console.ReadLine();

            switch (input.ToLower())
            {
                case "ja":
                    dataAccess.DeleteBlogpostInBlogpostTagTable(blogpost);
                    dataAccess.DeleteBlogpost(blogpost);
                    WriteGreen("\nBlogginlägget är raderat.");
                    Console.ReadKey();
                    PageMainMenu();
                    break;
                case "nej":
                    WriteLine("\nTryck på valfri knapp för att gå tillbaka till huvudmenyn");
                    Console.ReadKey();
                    PageMainMenu();
                    break;
                default:
                    WriteLine("Vänligen skriv 'ja' eller 'nej'");
                    Console.ReadKey();
                    PageDeletePost();
                    break;
            }
        }

        private void PageUpdatePost()
        {
            Header("Uppdatera");

            ShowAllBlogPostsBrief();

            Console.Write("Vilket blogginlägg vill du uppdatera? ");
            int postId = int.Parse(Console.ReadLine());

            BlogPost blogpost = dataAccess.GetPostById(postId);

            Console.WriteLine($"Den nuvarande titeln är {blogpost.Title}");

            Console.Write("Skriv in ny titel: ");

            string newTitle = Console.ReadLine();

            blogpost.Title = newTitle;

            dataAccess.UpdateBlogpost(blogpost);

            WriteGreen("Blogginlägget är uppdaterat.");
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

        private void WriteGreen(string text)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(text);
            Console.ResetColor();
        }
    }
}
