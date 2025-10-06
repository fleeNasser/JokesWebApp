using JokesWebApp.Data;
using JokesWebApp.Models;
using JokesWebApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JokesWebApp.Controllers
{
    public class JokesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;


        public JokesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Jokes
        public async Task<IActionResult> Index()
        {
            return View(await _context.Joke.Include(j => j.User).ToListAsync());
        }

        // GET: Jokes/ShowSearchForm
        public async Task<IActionResult> ShowSearchForm()
        {
            return View();
        }

        // POST: Jokes/ShowSearchResults
        public async Task<IActionResult> ShowSearchResults(String SearchPhrase)
        {
            return View("Index", await _context.Joke.Where(j => j.JokeQuestion.Contains
            (SearchPhrase)).Include(j => j.User).ToListAsync());
        }

        // GET: Jokes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            /*var joke = await _context.Joke
                .FirstOrDefaultAsync(m => m.JokeID == id);
            */
            var joke = await _context.Joke
                .Include(j => j.User)
                .FirstOrDefaultAsync(m => m.JokeID == id);
            if (joke == null)
            {
                return NotFound();
            }

            return View(joke);
        }

        // GET: Jokes/Create

        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Jokes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(JokeCreateViewModel model)
        {
            IdentityUser currentUser = await _userManager.GetUserAsync(HttpContext.User);
            if (ModelState.IsValid)
            {
                var joke = new Joke
                {
                    JokeQuestion = model.JokeQuestion,
                    JokeAnswer = model.JokeAnswer,
                    UserId = currentUser.Id
                };
                _context.Add(joke);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                ModelState.AddModelError("", error.ErrorMessage);
            }
            return View(model);
        }

        // GET: Jokes/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var joke = await _context.Joke.FindAsync(id);
            if (joke == null)
            {
                return NotFound();
            }

            var model = new JokeEditViewModel
            {
                JokeID = joke.JokeID,
                JokeQuestion = joke.JokeQuestion,
                JokeAnswer = joke.JokeAnswer
            };

            return View(model);
        }

        // POST: Jokes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> Edit(int id, JokeEditViewModel model)
        {
            if (id != model.JokeID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var joke = await _context.Joke.FindAsync(id);
                if (joke == null)
                {
                    return NotFound();
                }

                joke.JokeQuestion = model.JokeQuestion;
                joke.JokeAnswer = model.JokeAnswer;

                // Optionally, update UserId to current user if you want to track who edited
                // IdentityUser currentUser = await _userManager.GetUserAsync(HttpContext.User);
                // joke.UserId = currentUser.Id;

                _context.Update(joke);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
        /* OLD FUNCTION
         * public async Task<IActionResult> Edit(int id, [Bind("JokeID,JokeQuestion,JokeAnswer")] Joke joke)
        {
            if (id != joke.JokeID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(joke);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JokeExists(joke.JokeID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(joke);
        }
         */

        // GET: Jokes/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var joke = await _context.Joke
                .FirstOrDefaultAsync(m => m.JokeID == id);
            if (joke == null)
            {
                return NotFound();
            }

            return View(joke);
        }

        // POST: Jokes/Delete/5
        [Authorize ]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var joke = await _context.Joke.FindAsync(id);
            if (joke != null)
            {
                _context.Joke.Remove(joke);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JokeExists(int id)
        {
            return _context.Joke.Any(e => e.JokeID == id);
        }
    }
}
