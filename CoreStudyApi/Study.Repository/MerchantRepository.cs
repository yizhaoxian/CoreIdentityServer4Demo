using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Study.IRepository;
using Study.Models;
using Study.Repository.Base;

namespace Study.Repository
{
    public class MerchantRepository : BaseRepository<Merchant>, IMerchantRepository
    {
        public override Task<IList<Merchant>> LoadAll()
        {
            return GetSourceDatasAsync();
        }

        private Task<IList<Merchant>> GetSourceDatasAsync()
        {
            IList<Merchant> datas = new List<Merchant>();
            datas.Add(new Merchant
            {
                Id = 1,
                Name = "张三",
                AppId = "admin",
                AppSecret = "123456",
                Roles = "Admin"
            });
            datas.Add(new Merchant
            {
                Id = 1,
                Name = "李四",
                AppId = "client",
                AppSecret = "123456",
                Roles = "Client"
            });
            datas.Add(new Merchant
            {
                Id = 1,
                Name = "王五",
                AppId = "system",
                AppSecret = "123456",
                Roles = "System,Admin,Client"
            });

            return Task.Run(() => datas);
        }
    }
}
