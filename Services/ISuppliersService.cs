using Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services
{
    public interface ISuppliersService
    {
       SupplierEntity[] GetSuppliersByCustomer(CustomerEntity customer);
       bool Add(SupplierEntity supplier);
       bool Edit(SupplierEntity supplier);
    }
}
