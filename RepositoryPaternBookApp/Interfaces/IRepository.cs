namespace RepositoryPaternBookApp.Interfaces
{
	public interface IRepository<T> where T : class    //where Y : class == Constraints
	{
		Task<IEnumerable<T>> GetAllAsync();
		Task<T> GetByIdAsync(int id);
		Task AddAsync(T entity);
		Task UpdateAsync(T entity);
		Task DeleteAsync(int id);
	}
}
