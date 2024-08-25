using BLL.Interfaces;
using DAL.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Classes
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext applicationDbContext;
        DbSet<T> set;
        public Repository(ApplicationDbContext applicationDbContext )
        {
            this.applicationDbContext = applicationDbContext;
           set = applicationDbContext.Set<T>();
        }
        public void Add(T entity)
        {
            set.Add(entity);
        }

        public void Delete(int id)
        {
            T entity = Get(id);
            set.Remove(entity);
        }

        public T Get(int id)
        {
           return set.Find(id);
        }

        public IEnumerable<T> GetAll()
        {
            return set.ToList();
        }

        public void Update(T UpdatedEntity) 
        {
            set.Update(UpdatedEntity);
        }
        public IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
        {
            return applicationDbContext.Set<T>().Where(predicate).ToList();
        }
        public IEnumerable<T> FilterIncluded(string Included, Func<T, bool> func)
        {
            return applicationDbContext.Set<T>().Include(Included).Where(func);
        }
        public IEnumerable<T> GetAll(Func<T, bool> predicate)
        {
            return set.ToList().Where(predicate);
        }
    }
}
