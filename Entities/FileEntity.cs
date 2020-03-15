using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities
{
    public class FileEntity
    {
        public IFormFile File { get; set; }
        public int CustomerId { get; set; }
        public string SupplierId { get; set; }

    }
}
