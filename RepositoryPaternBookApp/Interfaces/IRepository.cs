namespace RepositoryPaternBookApp.Interfaces
{
	public interface IRepository<T> where T : class    //where Y : class == Constraints
	{
		IEnumerable<T> GetAllAsync();
		Task<T> GetByIdAsync(int id);
		Task<T> AddAsync(T entity);
		Task<T> UpdateAsync(T entity);
		Task<T> DeleteAsync(int id);
	}
}
