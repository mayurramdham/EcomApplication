using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.core.Interface
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string to, string name, string subject, string message);
    }
}
