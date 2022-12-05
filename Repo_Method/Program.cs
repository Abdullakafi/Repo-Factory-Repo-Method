using Repo_Method.Models;
using Repo_Method.Repoes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repo_Method
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var b1 = new Book(1, "Othelo", (Genre)Genre.Novel, 99.00M);
            var b2 = new Book(2, "War and Peace", (Genre)Genre.Classic, 199.00M);
            var b3 = new Book(3, "Ancient Mariner", (Genre)Genre.Poetry, 299.00M);
            var ru = new RepoUnit();
            var repoBook = ru.GetRepo<Book>();
            repoBook.Add(b1);
            repoBook.Add(b2);
            repoBook.Add(b3);
            repoBook.Get()
                .ToList()
                .ForEach(b => Console.WriteLine($"Title: {b.Title} Genre: {b.Genre} Price: {b.Price:0.00}"));
            Console.WriteLine("Update");
            b2.Price = 189.00M;
            repoBook.Update(b2);
            var bb = repoBook.Get(2);
            Console.WriteLine($"Title: {bb.Title} Genre: {bb.Genre} Price: {bb.Price:0.00}");
            Console.WriteLine("Delete");
            repoBook.Delete(2);
            repoBook.Get()
               .ToList()
               .ForEach(b => Console.WriteLine($"Title: {b.Title} Genre: {b.Genre} Price: {b.Price:0.00}"));
            Console.ReadLine();
        }
    }
}
namespace Repo_Method.Models
{
    public enum Genre { Novel, Poetry, Classic }
    public interface IEntity
    {
        int Id { get; set; }
    }
    public class Book : IEntity
    {
        public Book()
        {
            //Console.WriteLine("Book");
        }
        public Book(int id, string title, Genre genre, decimal price)
        {
            this.Id = id;
            this.Title = title;
            this.Genre = genre;
            this.Price = price;
            //Console.WriteLine(id);
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public Genre Genre { get; set; }
        public decimal Price { get; set; }
    }
}
namespace Repo_Method.Repoes
{
    public interface IRepo<T> where T : class, IEntity, new()
    {
        IList<T> Get();
        T Get(int id);
        void Add(T entity);
        void Update(T entity);
        void Delete(int id);
    }
    public class Repo<T> : IRepo<T> where T : class, IEntity, new()
    {
        IList<T> list;

        public Repo(IList<T> list)
        {
            this.list = list;
        }
        public IList<T> Get()
        {
            return this.list;
        }
        public T Get(int id)
        {
            return this.list.FirstOrDefault(x => x.Id == id);
        }
        public void Add(T entity)
        {
            this.list.Add(entity);
        }
        public void Update(T entity)
        {

            var item = this.list.FirstOrDefault(x => x.Id == entity.Id);
            if (item != null)
            {
                var index = this.list.IndexOf(item);
                this.list.RemoveAt(index);
                this.list.Insert(index, item);
            }
        }
        public void Delete(int id)
        {
            var item = this.list.FirstOrDefault(x => x.Id == id);
            if (item != null) this.list.Remove(item);

        }
    }
    public interface IRepoUnit
    {
        IRepo<T> GetRepo<T>() where T: class, IEntity, new();
    }
    public class RepoUnit : IRepoUnit
    {
        public IRepo<T> GetRepo<T>() where T : class, IEntity, new()
        {
            return new Repo<T>(new List<T>());
        }
    }
}
