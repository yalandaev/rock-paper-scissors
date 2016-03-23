using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebClient.Infrastructure;
using WebClient.ViewModels;

namespace WebClient.Controllers
{
    public class HomeController : ControllerBase
    {
        [Authenticate]
        public ActionResult Index()
        {
            ViewBag.PlayerName = CurrentUser.Name;

            GameEngine.Engine.Instance.AddPlayer(CurrentUser.Name);

            return View(new HomeViewModel());
        }
    }
}