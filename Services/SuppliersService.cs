using Entities;
using ManagingDebts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Services
{
    public class SuppliersService : ISuppliersService
    {
        private readonly ManagingDebtsContext context;

        public SuppliersService(ManagingDebtsContext context)
        {
            this.context = context;
        }
        public SupplierEntity[] GetSuppliersByCustomer(CustomerEntity customer)
        {
            var data = context.Suppliers.Where(x => x.SupplierCustomerId == customer.Id).AsNoTracking().Select( x=> new SupplierEntity { 
            Id =x.SupplierId,
            Name = x.SupplierName,
            IsEnable = x.SupplierEnabled,
            CustomerId = x.SupplierCustomerId,
            PkudatYomanNumber = x.SupplierPkudatYomanNumber,
            WithBanks = x.SupplierWithBanks,
            SupplierNumberInFinance = x.SupplierNumberInFinance
            });
            return data != null ? data.ToArray() : null;
        }
        public bool Add(SupplierEntity supplier)
        {
            try
            {
                string supplierId = Convert.ToInt32(supplier.Id).ToString("D9");
                context.Suppliers.Add(new Suppliers { SupplierId = supplierId, SupplierEnabled = true, 
                    SupplierName = supplier.Name,SupplierWithBanks =supplier.WithBanks , SupplierPkudatYomanNumber =supplier.PkudatYomanNumber, SupplierCustomerId = supplier.CustomerId
                    ,SupplierNumberInFinance = supplier.SupplierNumberInFinance
                });
                context.SaveChanges();
                return true;
            }
            catch (Exception e)
            {

                throw e;
            }
        }
        public bool Edit(SupplierEntity supplier)
        {
            try
            {
                var oldSupplier = context.Suppliers.Find(supplier.Id,supplier.CustomerId);
                if (oldSupplier != null)
                {
                    oldSupplier.SupplierName = supplier.Name;
                    oldSupplier.SupplierEnabled = supplier.IsEnable;
                    oldSupplier.SupplierWithBanks = supplier.WithBanks;
                    oldSupplier.SupplierPkudatYomanNumber = supplier.PkudatYomanNumber;
                    oldSupplier.SupplierNumberInFinance = supplier.SupplierNumberInFinance;
                    context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {

                throw e;
            }
        }
    }
}
