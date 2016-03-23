using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebClient.Models;

namespace WebClient.Infrastructure
{
    public class AuthenticateAttribute : AuthorizeAttribute
    {
        public bool AllowAnonymus { get; set; }

        public AuthenticateAttribute()
        {
        }

        public AuthenticateAttribute(bool allowAnonymus)
        {
            AllowAnonymus = allowAnonymus;
        }

        protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
        {
            if (AllowAnonymus)
                return true;

            var container = new UnityContainer();
            //Account user = container.Resolve<IAuthenticationService>().CurrentUser;
            var repository = new FakeAccountRepository();
            var service = new FormsAuthenticationService(repository);
            Account user = service.CurrentUser;

            if (user == null)
                return false;

            return true;
        }

        protected override void HandleUnauthorizedRequest(System.Web.Mvc.AuthorizationContext filterContext)
        {
            filterContext.Result = new System.Web.Mvc.RedirectResult("/account/login", false);
        }
    }
}