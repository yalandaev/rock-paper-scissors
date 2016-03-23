using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebClient.Models;

namespace WebClient.Infrastructure
{
    public interface IAccountRepository
    {
        void CreateAccount(Account account);
        Account GetByName(string name);
        
    }
}
