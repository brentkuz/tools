using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;

namespace Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        IEnumerable<T> Get(Expression<Func<T, bool>> filter = null,
                            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null);
        T GetByID(object id);
        void Insert(T entity);
        void Delete(object id);
        void Delete(T entityToDelete);
        void Update(T entityToUpdate);
    }
    public class GenericRepository<T> : IGenericRepository<T> where T : class 
    {        
        #region private
        //DB context
        internal DbContext context;
        //DataSet for this
        internal DbSet<T> dbSet;
        #endregion

        #region constructor
        public GenericRepository(DbContext context)
        {      
            this.context = context;
            this.dbSet = context.Set<T>();
        }
        #endregion

        #region public
        //Method to get a collection of the given entity
        //In: Lambda expression for filtering (ex. q => q.Age > 25), Func with Lambda expression (ex. orderBy: q => q.OrderBy(d => d.Name))
        //Out: Enumerable set of <TEntity>
        public virtual IEnumerable<T> Get(Expression<Func<T, bool>> filter = null,
                                                Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null)
        {
            IQueryable<T> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }    
            if (orderBy != null)
            {
                return orderBy(query).ToList();               
            }
            else
            {
                return query.ToList();
            }
        }
        //Method to get an entity by id
        //In: entity id; Out: obj <TEntity>
        public virtual T GetByID(object id)
        {
            return dbSet.Find(id);
        }
        //Method to insert a new obj <TEntity>
        //In: entity obj; Out: n/a
        public virtual void Insert(T entity)
        {
            dbSet.Add(entity);
        }
        //Method to initiate deletion if only id is provided
        //In: entity id; Out: n/a
        public virtual void Delete(object id)
        {
            T entityToDelete = dbSet.Find(id);
            Delete(entityToDelete);
        }
        // Method to delete entity obj
        //In: obj <TEntity>; Out: n/a
        public virtual void Delete(T entityToDelete)
        {            
            dbSet.Attach(entityToDelete);            
            dbSet.Remove(entityToDelete);
        }
        //Method to update entity obj
        //In: obj <TEntity>; Out: n/a
        public virtual void Update(T entityToUpdate)
        {
            dbSet.Attach(entityToUpdate);
            context.Entry(entityToUpdate).State = EntityState.Modified;
        }
        #endregion
    }
}