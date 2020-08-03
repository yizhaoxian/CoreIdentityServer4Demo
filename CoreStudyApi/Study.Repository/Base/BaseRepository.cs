using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Study.IRepository.Base;
using Study.Models;

namespace Study.Repository.Base
{
    public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity>
    {
        public async Task<int> Add(TEntity entity)
        {
            var sql = "insert into employee values(@Name,@Address,@Mobile,@Gender)";
            using (IDbConnection conn = DbConfig.GetDbConnection())
            {
                var result = await conn.ExecuteAsync(sql, entity);
                return result;
            }
        }

        //public Task<int> Delete(int Id)
        //{

        //}

        public async Task<TEntity> GetById(int id)
        {
            var sql = $"select * from {typeof(TEntity).Name} where id={id}";
            using (IDbConnection conn = DbConfig.GetDbConnection())
            {
                return await conn.QueryFirstOrDefaultAsync<TEntity>(sql);
            }
        }

        public virtual async Task<IList<TEntity>> LoadAll()
        {
            var sql = "select * from " + typeof(TEntity).Name;
            using (IDbConnection conn = DbConfig.GetDbConnection())
            {
                var result = await conn.QueryAsync<TEntity>(sql);
                return result.ToList();
            }
        }

        //public Task<IList<TEntity>> LoadAll(string where)
        //{

        //}

        //public Task<int> Update(TEntity entity)
        //{

        //}
    }
}
