﻿using Microsoft.EntityFrameworkCore;
using RepositoryPaternBookApp.Data;
using RepositoryPaternBookApp.Interfaces;

namespace RepositoryPaternBookApp.Repositories
{
	public class Repository<T> : IRepository<T> where T : class
	{
		protected readonly RepoContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(RepoContext context)
        {
            _context = context;
			_dbSet = _context.Set<T>();
        }

		public async Task<IEnumerable<T>> GetAllAsync()
		{
			return await _dbSet.ToListAsync();
		}
		public async Task AddAsync(T entity)
		{
			await _dbSet.AddAsync(entity);
		}
		public async Task DeleteAsync(int id)
		{
			var entity = await _dbSet.FindAsync(id);
			if (entity != null)
			{
				_dbSet.Remove(entity);
			}
		}
		public async Task<T> GetByIdAsync(int id)
		{
			return await _dbSet.FindAsync(id);
		}
		public async Task UpdateAsync(T entity)
		{
			_context.Entry(entity).State = EntityState.Modified;
			await Task.CompletedTask;
		}
	}
}
