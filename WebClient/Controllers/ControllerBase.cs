using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebClient.App_Start;
using WebClient.Infrastructure;
using WebClient.Models;
using Microsoft.Practices.Unity;

namespace WebClient.Controllers
{
    public class ControllerBase : Controller
    {
        protected IAuthenticationService authenticationService;
        public ControllerBase()
        {
            IAuthenticationService authenticationService = UnityConfig.GetConfiguredContainer().Resolve<IAuthenticationService>();
            this.authenticationService = authenticationService;
        }

        public Account CurrentUser
        {
            get
            {
                return authenticationService.CurrentUser;
            }
        }

        protected bool IsAuthenticated
        {
            get { return CurrentUser != null; }
        }
    }
}