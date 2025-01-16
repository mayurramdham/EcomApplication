using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.core.Interface
{
   public interface IEmailTemplateService
    {
        string GenerateRegistrationEmail(string name,string username, string password);
    }
}
