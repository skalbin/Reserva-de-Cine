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
{
    public class PeliculaController : Controller
    {
        private readonly ReservaCineContext _context;

        public PeliculaController(ReservaCineContext context)
        {
            _context = context;
        }

        // GET: Pelicula
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var peliculas = await _context.Pelicula.ToListAsync();

            foreach(var p in peliculas){
               var genero = await _context.Genero
                .FirstOrDefaultAsync(g => g.Id == p.GeneroId);

                p.Genero.Nombre = genero.Nombre;
                p.Genero.Id = p.GeneroId;
            }

            return View(peliculas);
        }

        [AllowAnonymous]
        // GET: Pelicula/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            Pelicula pelicula = await _context.Pelicula
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pelicula == null)
            {
                return NotFound();
            }

            buscarGenero(pelicula.GeneroId);
            return View(pelicula);
        }

        [Authorize(Roles = nameof(Rol.Administrador))]
        // GET: Pelicula/Create
        public IActionResult Create()
        {
            completarGeneros();
            return View();
        }

        [Authorize(Roles = nameof(Rol.Administrador))]
        // POST: Pelicula/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = nameof(Rol.Administrador))]
        public async Task<IActionResult> Create([Bind("Id,FechaLanzamiento,Titulo,Descripcion,GeneroId,Duracion")] Pelicula pelicula)
        {
            if (ModelState.IsValid)
            {
                pelicula.Id = Guid.NewGuid();
                _context.Add(pelicula);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            

            return View(pelicula);
        }

        [Authorize(Roles = nameof(Rol.Administrador))]
        // GET: Pelicula/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pelicula = await _context.Pelicula.FindAsync(id);
            if (pelicula == null)
            {
                return NotFound();
            }
            completarGeneros();
            return View(pelicula);
        }

        [Authorize(Roles = nameof(Rol.Administrador))]
        // POST: Pelicula/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,FechaLanzamiento,Titulo,Descripcion, GeneroId, Duracion")] Pelicula pelicula)
        {
            if (id != pelicula.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pelicula);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PeliculaExists(pelicula.Id))
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
            return View(pelicula);
        }

        [Authorize(Roles = nameof(Rol.Administrador))]
        // GET: Pelicula/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pelicula = await _context.Pelicula
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pelicula == null)
            {
                return NotFound();
            }

            buscarGenero(pelicula.GeneroId);
            

            return View(pelicula);
        }

        [Authorize(Roles = nameof(Rol.Administrador))]
        // POST: Pelicula/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var pelicula = await _context.Pelicula.FindAsync(id);
            _context.Pelicula.Remove(pelicula);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool PeliculaExists(Guid id)
        {
            return _context.Pelicula.Any(p => p.Id == id);
        }

        public IActionResult Funciones(Guid id)
        {
            if (!PeliculaExists(id))
            {
                return NotFound();
            }

            var peliculaFuncion = _context.Pelicula
                                            .Include(pelicula => pelicula.Funciones)
                                                .ThenInclude(funcion => funcion.Sala)
                                            .FirstOrDefault(p => p.Id == id);

            var funciones = peliculaFuncion.Funciones.Select(funPel => funPel);            

            return View(funciones);
        }

        //completar lista de generos a seleccionar
        private async void completarGeneros()
        {
            ViewBag.GeneroId = await _context.Genero.ToListAsync();
        }

        //para buscar genero y llamar al metodo en el details,delete
        private async void buscarGenero(Guid id)
        {
            ViewBag.Genero= await _context.Genero
                .FirstOrDefaultAsync(g => g.Id == id);
        }


    }
}


