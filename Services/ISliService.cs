using Entities;
using System;
using System.Collections.Generic;
using System.Text;
using PkudatYomanWebService;
namespace Services
{
    public interface ISliService
    {
        (bool isSuccess, string msg) CreatePkudatYoman(GeneralBillingSummaryEntity summary, DateTime dateOfRegistration, UserEntity user);
        PkudatYomanShura[] DisplayPkudatYoman(GeneralBillingSummaryEntity summary, DateTime dateOfRegistration);
    }
}
