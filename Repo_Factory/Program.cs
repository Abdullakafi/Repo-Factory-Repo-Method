using Repo_Factory.Factories;
using Repo_Factory.Models;
using Repo_Factory.Repoes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repo_Factory
{
    internal class Program
    {
        public static void Main()
        {
            EntityFactory ef = new EntityFactory();
            var rf = new RepoFactory();
            //var b1 = new Book();
            //var b2 = new Book(1, "d", Genre.Classic, 99);
            var b1 = ef.Create<Book>(1, "Othelo",(Genre)Genre.Novel,  99.00M);
            var b2 = ef.Create<Book>(2, "War and Peace", (Genre)Genre.Classic, 199.00M);
            var b3 = ef.Create<Book>(3, "Ancient Mariner", (Genre)Genre.Poetry, 299.00M);
            var repoBook = rf.CreateRepo<Book>();
            repoBook.Add(b1);
            repoBook.Add(b2);
            repoBook.Add(b3);
            repoBook.Get()
                .ToList()
                .ForEach(b=> Console.WriteLine($"Title: {b.Title} Genre: {b.Genre} Price: {b.Price:0.00}"));
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
#region Models
namespace Repo_Factory.Models
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
        public Book(int id, string title,Genre genre, decimal price)
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
#endregion
#region Repo
namespace Repo_Factory.Repoes
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
}
#endregion Repo
#region Factory
namespace Repo_Factory.Factories
{
    public interface IEntityFactory
    {
        T Create<T>() where T : class, IEntity;
        T Create<T>(params object[] paramsArray) where T : class, IEntity;
    }

    public class EntityFactory : IEntityFactory
    {
        public T Create<T>() where T : class, IEntity
        {
            return Activator.CreateInstance(typeof(T)) as T;
        }
        public T Create<T>(params object[] paramters) where T : class, IEntity
        {
            return Activator.CreateInstance(typeof(T), paramters) as T;
        }
    }
    public interface IRepoFactory
    {
        Repo<T> CreateRepo<T>() where T : class, IEntity, new();
    }
    public class RepoFactory : IRepoFactory
    {
        public Repo<T> CreateRepo<T>() where T:class, IEntity, new()
        {
            return Activator.CreateInstance(typeof(Repo<T>), new object[] {new List<T>()}) as Repo<T>;
        }
    }
}
#endregion