using System.Diagnostics.CodeAnalysis;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    private readonly StoreContext _context;

    public GenericRepository(StoreContext context){
        _context = context;
    }

    public void Add(T entity)
    {
        _context.Set<T>().Add(entity);
    }

    public async Task<int> CountAsync(ISpecification<T> spec)
    {
        return await ApplySpecification(spec).CountAsync();
    }

    public void Delete(T entity)
    {
        _context.Set<T>().Remove(entity);
    }

    public async Task<T> GetByIdAsync(int id){
#pragma warning disable CS8603 // Possible null reference return.
        return await _context.Set<T>().FindAsync(id); ///if bot found - better for performance return null then throw an exception
#pragma warning restore CS8603 // Possible null reference return.
    }
   
    public async Task<T> GetEntityWithSpec(ISpecification<T> spec)
    {
#pragma warning disable CS8603 // Possible null reference return.
        return await ApplySpecification(spec).FirstOrDefaultAsync(); //if bot found - better for performance return null then throw an exception
#pragma warning restore CS8603 // Possible null reference return.
    }

    public async Task<IReadOnlyList<T>> ListAllAsync(){
        return await _context.Set<T>()
            .ToListAsync();
    }

    public async Task<IReadOnlyList<T>> ListAllWithSpecAsync(ISpecification<T> spec)
    {
        return await ApplySpecification(spec).ToListAsync();
    }

    public void Update(T entity)
    {
        _context.Set<T>().Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
    }

    private IQueryable<T> ApplySpecification(ISpecification<T> specification){
        return SpecificationEvaluator<T>.GetQuery(_context.Set<T>().AsQueryable(), specification);
    }
}