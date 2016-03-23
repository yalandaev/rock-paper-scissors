using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebClient.Models;

namespace WebClient.Infrastructure
{
    /// <summary>
    /// Эмуляция хранилища данных о пользователях
    /// </summary>
    public class FakeAccountRepository : IAccountRepository
    {
        private static List<Account> Accounts = new List<Account>() {
            new Account { Name = "Player_1", Password = "123" },
            new Account { Name = "Player_2", Password = "123" },
            new Account { Name = "Player_3", Password = "123" },
            new Account { Name = "Player_4", Password = "123" },
            new Account { Name = "Player_5", Password = "123" },
            new Account { Name = "Player_6", Password = "123" },
            new Account { Name = "Player_7", Password = "123" },
            new Account { Name = "Player_8", Password = "123" },
            new Account { Name = "Player_9", Password = "123" },
            new Account { Name = "Player_10", Password = "123" }
        };

        public void CreateAccount(Account account)
        {
            Accounts.Add(account);
        }

        public Account GetByName(string name)
        {
            return Accounts.Find(x => x.Name == name);
        }
    }
}