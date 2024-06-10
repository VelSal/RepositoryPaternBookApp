using RepositoryPaternBookApp.Interfaces;

namespace RepositoryPaternBookApp.Repositories
{
	public class Repository<T> : IRepository<T> where T : class
	{
		public Task<T> AddAsync(T entity)
		{
			throw new NotImplementedException();
		}

		public Task<T> DeleteAsync(int id)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<T> GetAllAsync()
		{
			throw new NotImplementedException();
		}

		public Task<T> GetByIdAsync(int id)
		{
			throw new NotImplementedException();
		}

		public Task<T> UpdateAsync(T entity)
		{
			throw new NotImplementedException();
		}
	}
}
