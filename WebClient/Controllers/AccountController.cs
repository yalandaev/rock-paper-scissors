using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebClient.Infrastructure;
using WebClient.Models;
using WebClient.ViewModels;

namespace WebClient.Controllers
{
    public class AccountController : ControllerBase
    {
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public RedirectResult Login(LoginViewModel model)
        {
            var repository = new FakeAccountRepository();
            var service = new FormsAuthenticationService(repository);

            service.Login(model.Name, model.Password, model.RememberMe);

            if(IsAuthenticated)
            {
                return new RedirectResult("/home/index", false);
            }
            return new RedirectResult("/account/login", false);
        }
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public RedirectResult Register(LoginViewModel model)
        {
            var repository = new FakeAccountRepository();
            var service = new FormsAuthenticationService(repository);

            Account account = new Account() { Id = Guid.NewGuid(), Name = model.Name, Password = model.Password };
            repository.CreateAccount(account);

            service.Login(model.Name, model.Password, model.RememberMe);

            if (IsAuthenticated)
            {
                return new RedirectResult("/home/index", false);
            }
            return new RedirectResult("/account/login", false);
        }
    }
}