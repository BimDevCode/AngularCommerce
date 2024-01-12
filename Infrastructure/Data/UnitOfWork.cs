using System.Collections;
using System.Linq.Expressions;
using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StoreContext _context;
        private Hashtable? _repositories;
        public UnitOfWork(StoreContext context)
        {
            _context = context;
        }

        public async Task<int> Complete()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
        {
            if (_repositories == null) _repositories = new Hashtable();

            var type = typeof(TEntity).Name;

            if (!_repositories.ContainsKey(type))
            {

                var repositoryType = typeof(GenericRepository<>);
                var specificRepository = repositoryType.MakeGenericType(typeof(TEntity));

                // IGenericRepository<TEntity>? repositoryInstance;
                // try
                // {
                //     var repositoryConstructor = specificRepository.GetConstructor(new[] { _context.GetType() });
                //     var context = _context;
                //     var newExpression = Expression.New(repositoryConstructor!, Expression.Constant(context));
                //     // Compile the NewExpression into a Func<object> that creates a new instance of the repository type
                //     var newFunc = Expression.Lambda<Func<GenericRepository<TEntity>>>(newExpression).Compile();
                    
                //     // Call the Func<object> to create a new instance of the repository type
                //     repositoryInstance = newFunc();
                // }
                // catch
                // {
                //     repositoryInstance = Activator.CreateInstance(specificRepository, _context) as GenericRepository<TEntity>;
                // }
                var repositoryInstance = Activator.CreateInstance(specificRepository, _context) as GenericRepository<TEntity>;
                if(repositoryInstance is not null) _repositories.Add(type, repositoryInstance);
            }
            var repository = _repositories[type] as IGenericRepository<TEntity>;
            return repository!;
        }
    }
}