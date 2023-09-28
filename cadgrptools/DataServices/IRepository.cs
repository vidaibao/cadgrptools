using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cadgrptools.DataServices
{
    public interface IRepository<T> : IDisposable where T : class
    {

        Task<bool> Insert(T entity);

        Task<T> Get(int id);

        Task<IEnumerable<T>> GetAll();

        Task<bool> Update(T entity);

        Task<bool> Delete(T entity);

        void Commit();

        void Revert();

    }
}
