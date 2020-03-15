using Entities;
using ManagingDebts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class CustomersService: ICustomersService
    {
        private readonly ManagingDebtsContext context;

        public CustomersService(ManagingDebtsContext context)
        {
            this.context = context;
        }
        public CustomerEntity[] GetAll()
        {
            var data= context.Customers.Select(x=>new CustomerEntity {Id=x.FinanceId,Name=x.MgaName, IsActive=x.IsActive }).AsNoTracking();
            return data != null ? data.ToArray() : null;
        }
        public CustomerEntity[] GetByUser(UserEntity user)
        {
            var dbUser = new UsersService(context).GetById(user);
            if (dbUser.IsSuperAdmin)
            {
                return GetAll();
            }
            var data = context.Customers.Where(x => x.FinanceId == dbUser.CustomerId)
                .Select(x => new CustomerEntity { Id = x.FinanceId, Name = x.MgaName, IsActive = x.IsActive }).AsNoTracking();
            return data != null ? data.ToArray() : null;
        }
    }
}
