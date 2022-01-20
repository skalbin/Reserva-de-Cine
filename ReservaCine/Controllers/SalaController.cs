using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ReservaCine.Data;
using ReservaCine.Models;

namespace ReservaCine.Controllers
{   [Authorize]
    public class SalaController : Controller
    {
        private readonly ReservaCineContext _context;

        public SalaController(ReservaCineContext context)
        {
            _context = context;
        }
        [Authorize(Roles = nameof(Rol.Administrador))]
        // GET: Sala
        public async Task<IActionResult> Index()
        {
            var reservaCineContext = _context.Sala.Include(s => s.TipoSala);
            return View(await reservaCineContext.ToListAsync());
        }
        [Authorize(Roles = nameof(Rol.Administrador))]
        // GET: Sala/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sala = await _context.Sala
                .Include(s => s.TipoSala)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (sala == null)
            {
                return NotFound();
            }

            return View(sala);
        }

        [Authorize(Roles = nameof(Rol.Administrador))]
        // GET: Sala/Create
        public IActionResult Create()
        {
            ViewData["TipoSalaId"] = new SelectList(_context.TipoSala, "Id", "Nombre");
            return View();
        }

        [Authorize(Roles = nameof(Rol.Administrador))]
        // POST: Sala/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Numero,CapacidadButacas,TipoSalaId")] Sala sala)
        {
            if (ModelState.IsValid)
            {
                sala.Id = Guid.NewGuid();
                _context.Add(sala);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TipoSalaId"] = new SelectList(_context.TipoSala, "Id", "Nombre", sala.TipoSalaId);
            return View(sala);
        }
        [Authorize(Roles = nameof(Rol.Administrador))]
        // GET: Sala/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sala = await _context.Sala.FindAsync(id);
            if (sala == null)
            {
                return NotFound();
            }
            ViewData["TipoSalaId"] = new SelectList(_context.TipoSala, "Id", "Nombre", sala.TipoSalaId);
            return View(sala);
        }

        [Authorize(Roles = nameof(Rol.Administrador))]
        //me muestra un listado de los tipo de salas
        private async void mostrarTipoSalas()
        {
            var tipoSalas = await _context.TipoSala
                                        .Select(t => new SelectListItem(t.Nombre, t.Id.ToString()))
                                        .ToListAsync();

            ViewBag.TipoSalas = tipoSalas;
        }

        [Authorize(Roles = nameof(Rol.Administrador))]
        // POST: Sala/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Sala sala)
        {
            if (id != sala.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //solo se puede editar el tipo de sala - sino el modelo deja de ser valido
                    var salaNueva = _context.Sala.FirstOrDefault(s => s.Id == id);
                   salaNueva.TipoSalaId = sala.TipoSalaId;
                    _context.Update(salaNueva);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SalaExists(sala.Id))
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
            ViewData["TipoSalaId"] = new SelectList(_context.TipoSala, "Id", "Nombre", sala.TipoSalaId);
            return View(sala);
        }
        [Authorize(Roles = nameof(Rol.Administrador))]
        //GET: Sala/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sala = await _context.Sala
                .Include(s => s.TipoSala)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sala == null)
            {
                return NotFound();
            }

            return View(sala);
        }
        [Authorize(Roles = nameof(Rol.Administrador))]
        // POST: Sala/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var sala = await _context.Sala.FindAsync(id);
            _context.Sala.Remove(sala);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private bool SalaExists(Guid id)
        {
            return _context.Sala.Any(e => e.Id == id);
        }
    }
}
