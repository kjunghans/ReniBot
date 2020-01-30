using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ReniBot.Repository
{
    public class GenericRepository<TEntity> where TEntity : class
    {
        internal BotContext _context;
        internal DbSet<TEntity> _dbSet;

        public GenericRepository(BotContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = context.Set<TEntity>();
        }

        public virtual IEnumerable<TEntity> GetItem(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "")
        {
            IQueryable<TEntity> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (includeProperties == null)
                throw new ArgumentNullException(nameof(includeProperties));

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
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

        public virtual IEnumerable<TEntity> PagedGet(int pageIndex, int pageSize,
           Expression<Func<TEntity, bool>> filter = null,
           Expression<Func<TEntity, string>> orderBy = null,
           string includeProperties = "" )
        {
            IQueryable<TEntity> query = _dbSet;

            if (orderBy != null)
            {
                query = query.OrderBy(orderBy);
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            query = query.Skip(pageIndex * pageSize).Take(pageSize);

            if (includeProperties == null)
                throw new ArgumentNullException(nameof(includeProperties));

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

             return query.ToList();
            
        }

        public virtual TEntity GetFirst(Expression<Func<TEntity, bool>> filter,
          Expression<Func<TEntity, DateTimeOffset>> orderBy = null)
        {
            IQueryable<TEntity> query = _dbSet;

            
            if (orderBy != null)
            {
                query = query.OrderBy(orderBy);
            }

            return query.FirstOrDefault(filter);
        }

        public virtual TEntity GetLast(Expression<Func<TEntity, bool>> filter,
         Expression<Func<TEntity, DateTimeOffset>> orderBy = null)
        {
            IQueryable<TEntity> query = _dbSet;


            if (orderBy != null)
            {
                query = query.OrderByDescending(orderBy);
            }

            return query.FirstOrDefault(filter);
        }

        public virtual TEntity GetN(int pageIndex,
           Expression<Func<TEntity, bool>> filter = null,
           Expression<Func<TEntity, DateTimeOffset>> orderBy = null)
        {
            IQueryable<TEntity> query = _dbSet;

            if (orderBy != null)
            {
                query = query.OrderByDescending(orderBy);
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            query = query.Skip(pageIndex).Take(1);


            return query.SingleOrDefault();

        }

        public virtual TEntity GetByID(object id)
        {
            return _dbSet.Find(id);
        }

        public virtual void Insert(TEntity entity)
        {
            _dbSet.Add(entity);
        }

        public virtual void Delete(params object[] keyValues)
        {
            TEntity entityToDelete = _dbSet.Find(keyValues);
            Delete(entityToDelete);
        }

        public virtual void Delete(TEntity entityToDelete)
        {
            if (_context.Entry(entityToDelete).State ==  EntityState.Detached)
            {
                _dbSet.Attach(entityToDelete);
            }
            _dbSet.Remove(entityToDelete);
        }

        public virtual void Update(TEntity entityToUpdate)
        {
            _dbSet.Attach(entityToUpdate);
            _context.Entry(entityToUpdate).State = EntityState.Modified;
        }
    }
}
