using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using NLog;

namespace BookClasses
{
    public interface IRepository<T>
    {
        void Add(T entity);
        void Delete(T entity);
        void AddValues(IEnumerable<T> values);
        void DeleteValues(IEnumerable<T> values);
        T ReadNext();
        List<T> ReadAll();  
        void Clear();
    }

    public class BookListServiceToXml : IRepository<Book>
    {
        //private FileStream stream;
        private string path;
        private XmlSerializer serializer;
        private FileStream file;

        public BookListServiceToXml(string path)
        {
            serializer = new XmlSerializer(typeof(Book[]));
            this.path = path;
        }

        public void Add(Book entity)
        {
        }

        public void AddValues(IEnumerable<Book> values)
        {
            file = new FileStream(path, FileMode.Create);
            serializer.Serialize(file, values.ToArray());
            file.Close();
        }

        public List<Book> ReadAll()
        {
            Book[] result;
            file = new FileStream(path, FileMode.Open);
            result = (Book[]) serializer.Deserialize(file);
            file.Close();
            return new List<Book>(result);
        }

        public void Clear()
        {
            file = new FileStream(path, FileMode.Create);
        }

        public void Delete(Book entity)
        {
        }

        public void DeleteValues(IEnumerable<Book> values)
        {
        }

        public Book ReadNext()
        {
            return null;
        }
    }


    public class BookListService : ICollection<Book>
    {
        private List<Book> books;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public int Count
        {
            get { return books.Count(); }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public BookListService()
        {
            books = new List<Book>();
            logger.Info("BookListService was created.");
        }

        public void Add(Book book)
        {
            if (!books.Contains(book))
            {
                books.Add(book);
            }
            else
            {
                logger.Error($"AddBook: book {book.ToString()} already exists.");
                throw new BookAlreadyExistsException();
            }
        }

        public List<Book> FindByTag(Predicate<Book> predicate)
        {
            if (predicate == null)
            {
                logger.Error("FindByTag: argument is null");
                throw new ArgumentNullException();
            }
            List<Book> result = books.FindAll(predicate);
            logger.Info("FindByTag: matching books were found");
            return result;
        } 

        public void SortBooksByTag(IComparer<Book> comparer)
        {
            if (comparer == null)
            {
                logger.Error("SortBooksByTag: argument is null");
                throw new ArgumentNullException();
            }
            books.Sort(comparer);
            logger.Info("SortByTag: books were sorted successfully");
        }

        public bool Remove(Book book)
        {
            if (books.Contains(book))
            {
                books.Remove(book);
                logger.Info($"RemoveBook: book {book.ToString()} was successfully removed");
            }
            else
            {
                logger.Error("RemoveBook: no such book in BookListService");
                throw new BookDoesntExistException();
            }
            return true;
        }

        public void Load(IRepository<Book> repository )
        {
            if (repository == null)
            {
                logger.Error("Load: argument is null");
                throw new ArgumentNullException();
            }
            books = GetBooks(repository);
            logger.Info("Load: books were loaded successfully");
        }

        public void Save(IRepository<Book> repository)
        {
            if (repository == null)
            {
                logger.Error("Save: argument is null");
                throw new ArgumentNullException();
            }
            WriteBooks(repository);
            logger.Info("Save: books were saved successfully");
        }

        public void Clear()
        {
            books.Clear();
        }

        public bool Contains(Book item)
        {
            return books.Contains(item);
        }

        public void CopyTo(Book[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException();
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException();
            books.CopyTo(array, arrayIndex);
        }

        public IEnumerator<Book> GetEnumerator()
        {
            return books.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return books.GetEnumerator();
        }

        private List<Book> GetBooks(IRepository<Book> repository)
        {
            List<Book> result = new List<Book>();
           // BinaryReader reader = new BinaryReader(input);
            try
            {
                //while (true)
                //{
                    result = repository.ReadAll();
                    //Book book = repository.ReadNext();
                    //if (book != null)
                    //    result.Add(book);
                    //else
                    //    break;
                //}
                //while (reader.BaseStream.Position != reader.BaseStream.Length)
                //{
                //    Book book = new Book();
                //    book.Title = reader.ReadString();
                //    book.Author = reader.ReadString();
                //    book.Length = reader.ReadInt32();
                //    book.YearOfPublishing = reader.ReadInt32();
                //    book.EditionNumber = reader.ReadInt32();
                //    result.Add(book);
                //}

            }
            catch (Exception)
            {
                logger.Error("Error during reading data from stream");
                throw new IOException();
            }
            return result;
        }

        private void WriteBooks(IRepository<Book> repository)
        {
           // BinaryWriter writer = new BinaryWriter(output);
           // writer.BaseStream.Position = 0;
            try
            {
                repository.AddValues(books);
            }
            catch (Exception)
            {
                logger.Error("Error during writing data into repository");
                throw new IOException();
            }
        }
    }

    class BookAlreadyExistsException : Exception
    {
        public BookAlreadyExistsException()
        {
        }

        public BookAlreadyExistsException(string message) : base(message)
        {
        }

        public BookAlreadyExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected BookAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    class BookDoesntExistException : Exception
    {
        public BookDoesntExistException()
        {
        }

        public BookDoesntExistException(string message) : base(message)
        {
        }

        public BookDoesntExistException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected BookDoesntExistException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
