using System.Threading.Tasks;

namespace TaskAPI.Repository
{
	public interface IReporitory<T>
	{
		public Task<IEnumerable<T>> GetList(int first, int count);

		public Task<T?> Read(int id);

		public Task<bool> Update(T entity);

		public Task<T> Create(T entity);

		public Task<bool> Delete(int id);
	}
}
