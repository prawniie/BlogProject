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

        public void Run()
        {
            PageMainMenu();
        }

        private void PageMainMenu()
        {
            Header("Huvudmeny");
            //Console.WriteLine("Välkommen!");

            //WriteLine("Hej");
            //WriteLine();

            ShowAllBlogPostsBrief();
        }

        private void ShowAllBlogPostsBrief()
        {
            var dataAccess = new DataAccess();

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
