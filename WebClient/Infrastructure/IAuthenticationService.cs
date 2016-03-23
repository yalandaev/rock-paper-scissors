using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebClient.Models;

namespace WebClient.Infrastructure
{
    public interface IAuthenticationService
    {
        void Login(string name, string password, bool rememberMe);
        void Logoff();
        Account CurrentUser { get; }
    }
}