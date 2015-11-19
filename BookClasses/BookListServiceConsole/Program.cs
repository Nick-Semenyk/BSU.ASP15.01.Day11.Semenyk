using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookClasses;
using NLog;

namespace BookListServiceConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            BookListServiceToXml repository = new BookListServiceToXml("books.xml");
            //read or recreate??
            //   Stream stream = new FileStream("books.b", FileMode.OpenOrCreate);
            Book book = new Book();
            Logger logger = LogManager.GetCurrentClassLogger();
            BookListService service = new BookListService();
            //service.Load(stream);
            book.Author = "author2";
            book.Title = "title";
            book.Length = 100;
            book.YearOfPublishing = 1999;
            book.EditionNumber = 0;
            service.Add(book);
            book = new Book();
            book.Author = "author";
            book.Title = "title";
            book.Length = 1000;
            book.YearOfPublishing = 2000;
            book.EditionNumber = 1;
            service.Add(book);
            service.Save(repository);
            /*List<Book> list = service.FindByTag(b => b.Author == "author");
            logger.Info("Test");
            foreach (Book result in list)
            {
                Console.WriteLine(result.ToString());
            }
            Console.ReadLine();*/
            service = new BookListService();
            service.Load(repository);
            List<Book> list = service.FindByTag(b => b.Author == "author");
            logger.Info("Test");
            foreach (Book result in list)
            {
                Console.WriteLine(result.ToString());
            }
            Console.ReadLine();
        }
    }
}
