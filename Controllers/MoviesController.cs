using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Moviez_.Models;
using Moviez_.ViewModels;
using System.Data.Entity;

namespace Moviez_.Controllers
{
    public class MoviesController : Controller
    {

        private ApplicationDbContext _context;

        public MoviesController()
        {
            _context = new ApplicationDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }


        public ViewResult Index()
        {
            if (User.IsInRole(RoleName.CanManageMovies))
                return View("Index");
            
            return View("ReadOnly");
        }
        
        [AllowAnonymous]
        public ViewResult ReadOnly()
        {
            var movies = _context.Movies.Include(m => m.Genre).ToList();

            return View(movies);
        }

        [AllowAnonymous]
        public ActionResult Details(int id)
        {
            var movie = _context.Movies.Include(m => m.Genre).SingleOrDefault(c => c.Id == id);

            if (movie == null)
                return HttpNotFound();

            return View(movie);
        }
        [Authorize(Roles = RoleName.CanManageMovies)]
        public ViewResult New()
        {
            var genres = _context.Genres.ToList();

            var viewModel = new MovieFormViewModel
            {
                Movie = new Movie(),
                Genres = genres
            };

            return View("MovieForm", viewModel);
        }
        [Authorize(Roles = RoleName.CanManageMovies)]
        public ActionResult Edit(int id)
        {
            var movie = _context.Movies.SingleOrDefault(c => c.Id == id);

            if (movie == null)
                return HttpNotFound();

            var viewModel = new MovieFormViewModel
            {
                Movie = movie,
                Genres = _context.Genres.ToList()
            };

            return View("MovieForm", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleName.CanManageMovies)]
        public ActionResult Save(Movie movie)
        {
            if (!ModelState.IsValid)
            {
                var viewModel = new MovieFormViewModel
                {
                    Movie = movie,
                    Genres = _context.Genres.ToList()
                };
                return View("MovieForm", viewModel);
            }
            if (movie.Id == 0)
            {
                movie.DateAdded = DateTime.Now;
                _context.Movies.Add(movie);
            }
            else
            {
                var movieInDb = _context.Movies.Single(m => m.Id == movie.Id);
                movieInDb.Name = movie.Name;
                movieInDb.GenreId = movie.GenreId;
                movieInDb.InStock = movie.InStock;
                movieInDb.ReleaseDate = movie.ReleaseDate;
                movieInDb.Description = movie.Description;
            }

            _context.SaveChanges();

            return RedirectToAction("Index", "Movies");
        }

        private IEnumerable<Movie> GetMovies()
        {
            return new List<Movie>
            {
                new Movie { Id = 1, Name = "John Wick", Description = "With the untimely death of his beloved wife" +
                " still bitter in his mouth, John Wick, the expert former assassin, receives one final gift from her--a precious keepsake" +
                " to help John find a new meaning in life now that she is gone. But when the arrogant Russian mob prince, Iosef Tarasov," +
                " and his men pay Wick a rather unwelcome visit to rob him of his prized 1969 Mustang and his wife's present, the legendary" +
                " hitman will be forced to unearth his meticulously concealed identity. Blind with revenge, John will immediately unleash" +
                " a carefully orchestrated maelstrom of destruction against the sophisticated kingpin, Viggo Tarasov, and his family," +
                " who are fully aware of his lethal capacity. Now, only blood can quench the boogeyman's thirst for retribution. Written" +
                " by Nick Riganas"},
                new Movie { Id = 2, Name = "The Matrix", Description = "Thomas A. Anderson is a man living two lives." +
                " By day he is an average computer programmer and by night a hacker known as Neo. Neo has always questioned his reality, but" +
                " the truth is far beyond his imagination. Neo finds himself targeted by the police when he is contacted by Morpheus, a" +
                " legendary computer hacker branded a terrorist by the government. As a rebel against the machines, Neo must confront the" +
                " agents: super-powerful computer programs devoted to stopping Neo and the entire human rebellion. Written by redcommander27" }
            };
        }
    }
}