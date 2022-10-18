using Microsoft.AspNetCore.Mvc;

namespace DataProcessingServer.Controllers
{
    public class HomeController : Controller
    {

        #region Dependencies

        #endregion

        #region Constructor

        public HomeController()
        {

        }

        #endregion


        #region Actions

        public IActionResult Index()
        {
            return Redirect(UrlSelector.IndexPage);
        }

        public IActionResult About()
        {
            return View(UrlSelector.AboutPage);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return Redirect(UrlSelector.ErrorPage);
        }

        #endregion

    }
}