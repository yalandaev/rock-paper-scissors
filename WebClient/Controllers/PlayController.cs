using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebClient.Infrastructure;
using WebClient.ViewModels;

namespace WebClient.Controllers
{
    public class PlayController : ControllerBase
    {
        // GET: Play
        [HttpPost]
        [Authenticate]
        public ActionResult Index(PlayViewModel model)
        {
            if (ModelState.IsValid)
            {
                Guid gameId = GameEngine.Engine.Instance.CreateGame(model.Name, CurrentUser.Name, model.TurnDuration, model.VictoryLimit);
                return RedirectToAction("Index", new { id = gameId });
            }
            return View(model);//WARN!
        }
        [Authenticate]
        public ActionResult Index(Guid id)
        {
            if(GameEngine.Engine.Instance.ConnectToGame(id, CurrentUser.Name))
            {
                var model = GameEngine.Engine.Instance.Games.Find(x => x.Id == id);
                ViewBag.GameName = model.Name;
                ViewBag.PlayerName = CurrentUser.Name;
                ViewBag.HintUsed = model.Players.Find(x => x.Name == CurrentUser.Name).HintUsed;
                ViewBag.Player2Name = model.Players.Count == 2 ? model.Players[1].Name : "None";
                ViewBag.Player2Points = model.Players.Count == 2 ? model.Players[1].Points.ToString() : "None";
                return View(model);
            }
            return RedirectToAction("Error");
        }

        public ActionResult Error()
        {
            ViewBag.ErrorMessage = "Connection error: maximum player limit";
            return View();
        }
    }
}