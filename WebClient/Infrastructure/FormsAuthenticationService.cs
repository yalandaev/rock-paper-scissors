using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using WebClient.Models;

namespace WebClient.Infrastructure
{
    /// <summary>
    /// Простейшая система forms-авторизации
    /// </summary>
    public class FormsAuthenticationService : IAuthenticationService
    {
        private const string AuthCookieName = "AuthCookie";
        private IAccountRepository accountRepository;

        public FormsAuthenticationService(IAccountRepository accountRepository)
        {
            this.accountRepository = accountRepository;
        }

        public Account CurrentUser
        {
            get
            {
                object cookie = HttpContext.Current.Request.Cookies[AuthCookieName] != null ? HttpContext.Current.Request.Cookies[AuthCookieName].Value : null;
                if (cookie != null && !string.IsNullOrEmpty(cookie.ToString()))
                {
                    var ticket = FormsAuthentication.Decrypt(cookie.ToString());
                    return accountRepository.GetByName(ticket.Name);
                }
                return null;
            }
        }

        public void Login(string name, string password, bool rememberMe)
        {
            DateTime expiresDate = DateTime.Now.AddMinutes(30);
            if (rememberMe)
                expiresDate = expiresDate.AddDays(10);

            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                1,
                name,
                DateTime.Now,
                expiresDate,
                rememberMe,
                name);

            string encryptedTicket = FormsAuthentication.Encrypt(ticket);

            SetValue(AuthCookieName, encryptedTicket, expiresDate);

        }

        public void Logoff()
        {
            SetValue(AuthCookieName, null, DateTime.Now.AddYears(-1));

        }

        public static void SetValue(string cookieName, string cookieObject, DateTime dateStoreTo)
        {

            HttpCookie cookie = HttpContext.Current.Response.Cookies[cookieName];
            if (cookie == null)
            {
                cookie = new HttpCookie(cookieName);
                cookie.Path = "/";
            }

            cookie.Value = cookieObject;
            cookie.Expires = dateStoreTo;

            HttpContext.Current.Response.SetCookie(cookie);
        }
    }
}