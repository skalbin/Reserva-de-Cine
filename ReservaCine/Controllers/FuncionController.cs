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
    public class FuncionController : Controller
    {
        private readonly ReservaCineContext _context;

        public FuncionController(ReservaCineContext context)
        {
            _context = context;
        }

        // GET: Funcion
        public async Task<IActionResult> Index()
        {
            //que me muestre las funciones que esten confirmadas,incluyendo la peli,la sala y el tipo de sala
            var reservaCineContext = _context.Funcion
                                       .Where(f => f.Confirmar) //donde las funciones estén confirmadas
                                    .Include(f => f.Pelicula)
                                    .Include(f => f.Sala)
                                    .ThenInclude(s => s.TipoSala);
            return View(await reservaCineContext.ToListAsync());
        }

        // GET: Funcion/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var funcion = await _context.Funcion
                .Include(f => f.Pelicula)
                .Include(f => f.Sala)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (funcion == null)
            {
                return NotFound();
            }

            return View(funcion);
        }

        [Authorize(Roles = nameof(Rol.Administrador))]
        // GET: Funcion/Create
        public IActionResult Create()
        {
            completarPeliculas();
            completarSalas();
            return View();
        }

        // POST: Funcion/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = nameof(Rol.Administrador))]
        public async Task<IActionResult> Create(Funcion funcion)
        {
            if (ModelState.IsValid)
            {
                funcion.Id = Guid.NewGuid();
                funcion.Hora = new DateTime(1, 1, 1, funcion.Hora.Hour, funcion.Hora.Minute, funcion.Hora.Second);



                var butacas = await _context.Sala
                                         .Where(s => s.Id == funcion.SalaId)
                                         .FirstOrDefaultAsync(s => s.Id == funcion.SalaId);

                funcion.CantButacasDisponibles = butacas.CapacidadButacas;


                _context.Add(funcion);
                await _context.SaveChangesAsync();

               
                return RedirectToAction(nameof(Index));
            }


            ViewData["PeliculaId"] = new SelectList(_context.Pelicula, "Id", "Titulo", funcion.PeliculaId);
            ViewData["SalaId"] = new SelectList(_context.Sala, "Id", "Numero", funcion.SalaId);
          
            return View(funcion);
        }

        [Authorize(Roles = nameof(Rol.Administrador))]
        // GET: Funcion/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var funcion = await _context.Funcion.FindAsync(id);
            if (funcion == null)
            {
                return NotFound();
            }
            buscarSala(funcion.SalaId);
            buscarPelicula(funcion.PeliculaId);
            completarPeliculas();
            completarSalas();
            obtenerTodasLasSalas();



            return View(funcion);
        }
        private async void obtenerTodasLasSalas()
        {

            var salas = await _context.Sala
                                        .Include(s => s.TipoSala)
                                        .Select(s => new SelectListItem(string.Concat(s.Numero, " - ", s.TipoSala.Nombre, " - ", s.CapacidadButacas), s.Id.ToString()))
                                        .ToListAsync();

            ViewBag.Salas = salas;
        }

        [Authorize(Roles = nameof(Rol.Administrador))]
        // POST: Funcion/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Funcion funcion)
        {
            if (id != funcion.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                
              
                try
                {
                    //si cambia el estado a no confirmada de la funcion, 
                    if (!funcion.Confirmar)
                    {
                        //se verifica si la funcion tiene reservas activas
                        var tieneReservas = _context.Reserva
                                        .Any(r => r.FuncionId == funcion.Id && r.Activa);
                          
                        //si es true, el modelo deja de ser valido y muestra el msj de error
                        if (tieneReservas)
                        {


                            ModelState.AddModelError(nameof(Funcion.Confirmar), "No sé puede cancelar la función, ya que posee reservas activas.");

                            ViewData["PeliculaId"] = new SelectList(_context.Pelicula, "Id", "Titulo", funcion.PeliculaId);
                            ViewData["SalaId"] = new SelectList(_context.Sala, "Id", "Numero", funcion.SalaId);
                            completarPeliculas();
                            completarSalas();
                            obtenerTodasLasSalas();

                            return View(funcion);
                        }


                    }
                    _context.Update(funcion);
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FuncionExists(funcion.Id))
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
            ViewData["PeliculaId"] = new SelectList(_context.Pelicula, "Id", "Titulo", funcion.PeliculaId);
            ViewData["SalaId"] = new SelectList(_context.Sala, "Id", "Numero", funcion.SalaId);
            return View(funcion);
        }

        [Authorize(Roles = nameof(Rol.Administrador))]
        // GET: Funcion/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var funcion = await _context.Funcion
                .Include(f => f.Pelicula)
                .Include(f => f.Sala)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (funcion == null)
            {
                return NotFound();
            }

            return View(funcion);
        }

        [Authorize(Roles = nameof(Rol.Administrador))]
        // POST: Funcion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var funcion = await _context.Funcion.FindAsync(id);
            _context.Funcion.Remove(funcion);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FuncionExists(Guid id)
        {
            return _context.Funcion.Any(f => f.Id == id);
        }

        //me trae todas las peliculas
        private async void completarPeliculas()
        {
            ViewBag.PeliculaId = await _context.Pelicula.ToListAsync();
        }

        //para buscar pelicula y llamar al metodo en el details,delete
        private async void buscarPelicula(Guid id)
        {
            ViewBag.PeliculaTitulo = await _context.Pelicula
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        //completar lista de salas a seleccionar
        private async void completarSalas()
        {
            ViewBag.SalaId = await _context.Sala.ToListAsync();
        }

        //para buscar  y llamar al metodo en el details,delete
        private async void buscarSala(Guid id)
        {
            ViewBag.SalaNumero = await _context.Sala
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        [Authorize(Roles = nameof(Rol.Administrador))]
        public IActionResult Reservas(Guid id)
        {
            if (!FuncionExists(id))
            {
                return NotFound();
            }

            //al contexto funcion, le pido que me incluya las reservas, a esas reservas le pido las funciones y a esas funcion la peli
            var reservaFuncion = _context.Funcion
                                            .Include(funcion => funcion.Reservas)
                                                .ThenInclude(reserva => reserva.Funcion)
                                                .ThenInclude(funcion => funcion.Pelicula)
                                            .Include(funcion => funcion.Reservas)
                                                .ThenInclude(reserva => reserva.Cliente)
                                            .FirstOrDefault(f => f.Id == id);


            return View(reservaFuncion);
        }

    }
}
