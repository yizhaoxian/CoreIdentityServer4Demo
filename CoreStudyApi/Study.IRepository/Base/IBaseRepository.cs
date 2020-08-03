using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Study.IRepository.Base
{
    public interface IBaseRepository<TEntity>
    {
        Task<IList<TEntity>> LoadAll();
        //Task<IList<TEntity>> LoadAll(string where);
        Task<TEntity> GetById(int id);

        Task<int> Add(TEntity entity);

        //Task<int> Update(TEntity entity);

        //Task<int> Delete(int Id);
    }
}
