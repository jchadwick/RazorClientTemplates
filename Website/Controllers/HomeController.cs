using System.Web.Mvc;
using RazorClientTemplates.Website.Models;

namespace RazorClientTemplates.Website.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var movies = new[] {
                    new Movie { 
                        Title = "The Big Lebowski",
                        ReleaseDate = "1998",
                        RunningTime = "117 mins",
                        Actors = new[] {
                                new Actor("Jeff Bridges"),
                                new Actor("John Goodman"),
                                new Actor("Steve Buscemi"),
                            }
                    },
                    new Movie { 
                        Title = "The Princess Bride",
                        ReleaseDate = "1987",
                        RunningTime = "98 mins",
                        Actors = new[] {
                                new Actor("Cary Elwes"),
                                new Actor("Many Patinkin"),
                                new Actor("Robin Wright"),
                            }
                    },
                };

            return View(movies);
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult MoreMovies()
        {
            var movies = new[] {
                    new Movie { 
                        Title = "Rounders",
                        ReleaseDate = "1998",
                        RunningTime = "121 mins",
                        Actors = new[] {
                                new Actor("Matt Damon"),
                                new Actor("Edward Norton"),
                                new Actor("John Malcovich"),
                            }
                    },
                    new Movie { 
                        Title = "Batman",
                        ReleaseDate = "1989",
                        RunningTime = "126 mins",
                        Actors = new[] {
                                new Actor("Michael Keaton"),
                                new Actor("Jack Nicholson"),
                            }
                    },
                };
            
            return Json(movies, JsonRequestBehavior.AllowGet);
        }
    }
}
