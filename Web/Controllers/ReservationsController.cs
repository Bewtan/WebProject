using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Data;
using Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace Web.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly HotelDbContext _context;
        protected UserManager<User> _userManager { get; set; }


        public ReservationsController(HotelDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Reservations
        public async Task<IActionResult> Index()
        {
            var hotelDbContext = _context.Reservations.Include(r => r.Room).Include(r => r.User).Include(r => r.Clients);
            return View(await hotelDbContext.ToListAsync());
        }

        // GET: Reservations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.Room)
                .Include(r => r.User)
                .Include(r => r.Clients)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // GET: Reservations/Create
        public IActionResult Create()
        {
            ViewData["Room"] = new List<SelectListItem>();
            foreach(var room in _context.Rooms)
            {
                ViewBag.Room.Add(new SelectListItem { Text = room.Number.ToString(), Value = room.Id.ToString() });
            }
            ViewData["User"] = new List<SelectListItem>();
            foreach (var user in _context.Users)
            {
                ViewBag.User.Add(new SelectListItem { Text = user.UserName, Value = user.Id});
            }
            ViewData["Client"] = new List<SelectListItem>();
            foreach (var client in _context.Clients)
            {
                ViewBag.Client.Add(new SelectListItem { Text = client.FirstName + client.LastName, Value = client.Id.ToString() });
            }
            return View();
        }

        // POST: Reservations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,RoomId,ClientsIDs,DateOfArrival,DateOfLeaving,IsBreakfastIncluded,IsAllInclusive")] Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                var current_User = _userManager.GetUserAsync(HttpContext.User).Result;
                reservation.UserId = current_User.Id;
                foreach(var id in reservation.ClientsIDs)
                {
                    reservation.Clients.Add(_context.Clients.Find(id));
                }
                reservation.Cost = CalculateCost(reservation);
                _context.Add(reservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["Room"] = new List<SelectListItem>();
            foreach (var room in _context.Rooms)
            {
                ViewBag.Room.Add(new SelectListItem { Text = room.Number.ToString(), Value = room.Id.ToString() });
            }
            ViewData["User"] = new List<SelectListItem>();
            foreach (var user in _context.Users)
            {
                ViewBag.User.Add(new SelectListItem { Text = user.UserName, Value = user.Id });
            }
            ViewData["Client"] = new List<SelectListItem>();
            foreach (var client in _context.Clients)
            {
                ViewBag.Client.Add(new SelectListItem { Text = client.FirstName + client.LastName, Value = client.Id.ToString() });
            }
            return View(reservation);
        }

        // GET: Reservations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }

            ViewData["Room"] = new List<SelectListItem>();
            foreach (var room in _context.Rooms)
            {
                ViewBag.Room.Add(new SelectListItem { Text = room.Number.ToString(), Value = room.Id.ToString() });
            }
            ViewData["User"] = new List<SelectListItem>();
            foreach (var user in _context.Users)
            {
                ViewBag.User.Add(new SelectListItem { Text = user.UserName, Value = user.Id });
            }
            ViewData["Client"] = new List<SelectListItem>();
            foreach (var client in _context.Clients)
            {
                ViewBag.Client.Add(new SelectListItem { Text = client.FirstName + client.LastName, Value = client.Id.ToString() });
            }
            
            return View(reservation);
        }

        // POST: Reservations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,RoomId,ClientsIDs,DateOfArrival,DateOfLeaving,IsBreakfastIncluded,IsAllInclusive")] Reservation reservation)
        {
            if (id != reservation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var current_User = _userManager.GetUserAsync(HttpContext.User).Result;
                    reservation.UserId = current_User.Id;
                    List<int> ids = reservation.ClientsIDs.ToList<int>();
                    _context.Update(reservation);
                    _context.SaveChanges();
                    reservation = _context.Reservations.Include(r => r.Clients).First(i => i.Id == reservation.Id);
                    reservation.Clients.Clear();
                    _context.SaveChanges(); //pain
                    foreach (var ID in ids)
                    {
                        reservation.Clients.Add(_context.Clients.Find(ID));
                    }
                    
                    reservation.Cost = CalculateCost(reservation);
                    _context.Update(reservation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservationExists(reservation.Id))
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

            ViewData["Room"] = new List<SelectListItem>();
            foreach (var room in _context.Rooms)
            {
                ViewBag.Room.Add(new SelectListItem { Text = room.Number.ToString(), Value = room.Id.ToString() });
            }
            ViewData["User"] = new List<SelectListItem>();
            foreach (var user in _context.Users)
            {
                ViewBag.User.Add(new SelectListItem { Text = user.UserName, Value = user.Id });
            }
            ViewData["Client"] = new List<SelectListItem>();
            foreach (var client in _context.Clients)
            {
                ViewBag.Client.Add(new SelectListItem { Text = client.FirstName + client.LastName, Value = client.Id.ToString() });
            }
            return View(reservation);
        }

        // GET: Reservations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.Room)
                .Include(r => r.User)
                .Include(r => r.Clients)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservationExists(int id)
        {
            return _context.Reservations.Any(e => e.Id == id);
        }
        private decimal CalculateCost(Reservation reservation) {
            int Adults = 0;
            int Kids = 0;
            decimal Cost = 0;
            foreach(var client in reservation.Clients)
            {
                if (client.IsAdult == true)
                    Adults++;
                else
                    Kids++;
            }
            reservation.Room = _context.Rooms.Find(reservation.RoomId);
            Cost = (Adults * reservation.Room.PriceForAdult + Kids * reservation.Room.PriceForKid) * (decimal)(reservation.DateOfLeaving - reservation.DateOfArrival).TotalDays;
            return Cost;
        }
    }
}
