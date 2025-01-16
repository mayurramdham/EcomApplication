using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.core.Interface
{
    public interface IImageService
    {
        Task<object> Upload(IFormFile file);
    }
}
