using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Final.Data;
using Final.Models;
using System.Security.Claims;

namespace Final.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TicketsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private string getUserId()
        {
            string userId = null;
            var claimsIdentity = User.Identity as ClaimsIdentity;
            if (claimsIdentity != null)
            {
                // the principal identity is a claims identity.
                // now we need to find the NameIdentifier claim
                var userIdClaim = claimsIdentity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

                if (userIdClaim != null)
                {
                    userId = userIdClaim.Value;
                }
            }

            return userId;
        }

        // GET: Tickets
        public async Task<IActionResult> Index()
        {
            var tickets = await _context.Ticket.Where(t => t.Owner.Equals(getUserId())).ToListAsync();
            return View(tickets);
        }

        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Ticket.FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }
            else if (!ticket.Owner.Equals(getUserId()))
            {
                return NotFound();
            }
            return View(ticket);
        }

        // GET: Tickets/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Topic,Description")] Ticket ticket)
        {
            if (ModelState.IsValid && getUserId() != null)
            {
                ticket.Owner = getUserId();
                ticket.CreatedAt = DateTime.Now;
                ticket.Status = TicketStatus.Pending;

                _context.Add(ticket);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(ticket);
        }

        public async Task<IActionResult> Close(int id)
        {
            var ticket = await _context.Ticket.FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }
            else if (!ticket.Owner.Equals(getUserId()))
            {
                return NotFound();
            }
            return View(ticket);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmClose(int id)
        {
            Ticket ticket = await _context.Ticket.FindAsync(id);

            if (ticket != null)
            {
                ticket.Status = TicketStatus.Closed;
                ticket.UpdatedAt = DateTime.Now;

                _context.Update(ticket);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}
