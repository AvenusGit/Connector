using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ConnectorCenter.Models.Repository
{
    public interface IRepository<Type> 
    {
        /// <summary>
        /// Get element by identifier
        /// </summary>
        /// <param name="Id">Identifier</param>
        /// <returns>Element from current repository</returns>
        public Task<Type?> GetById(long Id);
        public Task<IEnumerable<Type>> GetAll();
        public Task Add(Type element);
        public Task<Type> Remove(Type? element);
        public Task<Type?> RemoveById(long Id);
        public Task Update(Type element);
        public Task<int> Count();
    }
}
