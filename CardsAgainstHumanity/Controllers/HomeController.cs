using System.Linq;
using System.Web.Mvc;
using CAH.Database;

namespace CardsAgainstHumanity.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DbTest()
        {
            var db = new CardsReposiitory();

            var questions = db.GetQuestionCards();

            return null;
        }
    }
}