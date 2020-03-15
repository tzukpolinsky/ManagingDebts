using Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IBezekFileReaderService
    {
        Task<bool> GetDataFromExcel(FileEntity fileEntity, bool hasHeader = true);
    }
}
