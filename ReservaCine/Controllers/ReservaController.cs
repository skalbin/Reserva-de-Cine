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
    [Authorize]
    public class ReservaController : Controller
    {
        private readonly ReservaCineContext _context;

        public ReservaController(ReservaCineContext context)
        {
            _context = context;
        }

        [Authorize(Roles = nameof(Rol.Administrador) + "," + nameof(Rol.Cliente))]
        // GET: Reserva
        public async Task<IActionResult> Index()//reservas historicas
        {
            var IdDeCliente = Guid.Parse(User.FindFirst("IdDeUsuario").Value);

           return View(await _context.Reserva.Where(r => r.ClienteId == IdDeCliente && r.Activa).OrderByDescending(x => x.FechaAlta).ToListAsync());
        }

        public async Task<IActionResult> ReservasHistoricas()
        {
            var IdDeCliente = Guid.Parse(User.FindFirst("IdDeUsuario").Value);

            return View(await _context.Reserva.Where(r => r.ClienteId == IdDeCliente && !r.Activa).OrderByDescending(x => x.FechaAlta).ToListAsync());
        }

        [Authorize(Roles = nameof(Rol.Administrador) + "," + nameof(Rol.Cliente))]
        // GET: Reserva/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            //que en los detalles de la reserva se muestre la funcion y la pelicula
            var reserva = await _context.Reserva
                .Include(r => r.Funcion)
                .ThenInclude(f => f.Pelicula)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (reserva == null)
            {
                return NotFound();
            }

            return View(reserva);
        }
        [Authorize(Roles = nameof(Rol.Cliente))]
        // GET: Reserva/Create
        public IActionResult Create()
        {
            completarFuncion();
            return View();
        }
        [Authorize(Roles = nameof(Rol.Cliente))]
        // POST: Reserva/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FechaAlta,CantidadButacas")] Reserva reserva)
        {
            if (ModelState.IsValid)
            {
                //se verifica que el cliente que realizo la reserva coinicida con el id del cliente loggeado
                reserva.Id = Guid.NewGuid();
                var clienteId = Guid.Parse(User.FindFirst("IdDeUsuario").Value);
                reserva.ClienteId = clienteId;

                _context.Add(reserva);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(reserva);
        }

        [Authorize(Roles = nameof(Rol.Cliente))]
        // GET: Reserva/Edit/5

        //[Authorize(Roles = nameof(Rol.Administrador) + "," + nameof(Rol.Cliente))]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reserva.FindAsync(id);
            if (reserva == null)
            {
                return NotFound();
            }
            return View(reserva);
        }
        [Authorize(Roles = nameof(Rol.Cliente))]
        // POST: Reserva/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,FechaAlta,CantidadButacas")] Reserva reserva)
        {
            if (id != reserva.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reserva);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservaExists(reserva.Id))
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
            return View(reserva);
        }
        [Authorize(Roles = nameof(Rol.Cliente))]
        // GET: Reserva/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reserva
                 .Include(r => r.Funcion)
                 .ThenInclude(r => r.Pelicula)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (reserva == null)
            {
                return NotFound();
            }

            return View(reserva);
        }
        [Authorize(Roles = nameof(Rol.Cliente))]
        // POST: Reserva/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
           
           
            var reserva = await _context.Reserva
                                                .Include(r=>r.Funcion)
                                                .ThenInclude(f => f.Pelicula)
                                                .FirstOrDefaultAsync(r => r.Id == id);

            //cuando se borra la reserva se vuelve a actualizar la cantidad de butacas disponibles
            reserva.Funcion.CantButacasDisponibles = reserva.Funcion.CantButacasDisponibles + reserva.CantidadButacas;
           
            _context.Reserva.Remove(reserva);
            _context.Update(reserva.Funcion);

            await _context.SaveChangesAsync();
          
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = nameof(Rol.Cliente))]
        public async Task<IActionResult> SeleccionarPelicula()
        {
            var clienteId = Guid.Parse(User.FindFirst("IdDeUsuario").Value);
            //validamos que el cliente con ese ID no tenga una reserva activa,
            //si tiene una reserva activa lo manda a los detalles de la reserva que tiene 
            if (ClienteTieneReservaActiva(clienteId, out Guid? reservaId))
            {
                return RedirectToAction(nameof(Details), new {id = reservaId});
            }

            // peliculas con funciones confirmadas y butacas disponibles
            var peliculasDisponibles = await _context.Funcion
                                                        .Include(f => f.Pelicula)
                                                         .ThenInclude(p => p.Genero)
                                                        .Where(f => f.CantButacasDisponibles > 0 && f.Confirmar)
                                                       .Select(f => f.Pelicula)
                                                      .Distinct()   //para que no se repita 2 veces el mismo nombre
                                                       .ToListAsync();

            return View(peliculasDisponibles); //(muestra la vista que creamos seleccionar pelicula)         
        }
        [Authorize(Roles = nameof(Rol.Cliente))]
        public async Task<IActionResult> SeleccionarFuncion(Guid peliculaId, int butacas)
        {
            var clienteId = Guid.Parse(User.FindFirst("IdDeUsuario").Value);
            if (ClienteTieneReservaActiva(clienteId, out Guid? reservaId))
            {
                return RedirectToAction(nameof(Details), new { id = reservaId });
            }

            if (butacas <= 0)
            {
                
                return RedirectToAction(nameof(SeleccionarPelicula));
            }
            // peliculas con funciones confirmadas y butacas disponibles
            var funcionesDisponibles = await _context.Funcion
                                                       .Where(f => f.CantButacasDisponibles >= butacas && f.Confirmar && f.PeliculaId == peliculaId)
                                                       .Include(f=> f.Sala)
                                                       .ThenInclude(f=> f.TipoSala)
                                                       .Include(f=> f.Pelicula)
                                                       .ToListAsync();
            ViewBag.Butacas = butacas;
            return View(funcionesDisponibles);
        }
        [Authorize(Roles = nameof(Rol.Cliente))]
        public async Task<IActionResult> ConfirmarReserva(Guid FuncionId, int butacas)
        {
                      
            var peliculaReservada = await _context.Funcion
                                                        .Include(f => f.Sala)
                                                       .ThenInclude(f => f.TipoSala)
                                                       .Include(f => f.Pelicula)
                                                       .FirstOrDefaultAsync(f => f.Id == FuncionId);
            ViewBag.Butacas = butacas;

            return View(peliculaReservada);
        }
        [Authorize(Roles = nameof(Rol.Cliente))]
        [HttpPost]
        public async Task<IActionResult> ReservaConfirmada(Guid FuncionId, int butacas)
        {
            var reserva = new Reserva();
            reserva.Id = Guid.NewGuid();
            reserva.FuncionId = FuncionId;
            reserva.ClienteId = Guid.Parse(User.FindFirst("IdDeUsuario").Value);
            reserva.Activa = true;
            reserva.CantidadButacas = butacas;
            reserva.FechaAlta = DateTime.Now;

            _context.Reserva.Add(reserva);

            //traigo la funcion que coincida con la funcion pasada por parametro y 
            //resto de la cantidad de butacas disponibles, las reservadas

            var funcionReservada = await _context.Funcion
                                                .FirstOrDefaultAsync(f => f.Id == FuncionId);
            funcionReservada.CantButacasDisponibles = funcionReservada.CantButacasDisponibles - butacas;

            _context.Update(funcionReservada);

            await _context.SaveChangesAsync();

            //una vez que confirma la reserva y se descuenta las butacas se muestran los detalles de la reserva
                return RedirectToAction(nameof(Details), new { id = reserva.Id });
            }

        [Authorize(Roles = nameof(Rol.Cliente))]
        private bool ClienteTieneReservaActiva(Guid clienteId, out Guid? reservaId)
        {
            //me trae desde el contexto de reserva el primero que cumpla con la condicion de tener reserva activa y que coincida el id del cliente
            var reserva = _context.Reserva.FirstOrDefault(r => r.Activa && r.ClienteId == clienteId);
            reservaId = reserva?.Id;
            return reserva != null;
        }
        [Authorize(Roles = nameof(Rol.Cliente))]
        private bool ReservaExists(Guid id)
        {
            return _context.Reserva.Any(r => r.Id == id);
        }
        [Authorize(Roles = nameof(Rol.Cliente))]
        private async void completarFuncion()
        {
            ViewBag.FuncionId = await _context.Funcion.ToListAsync();
        }
    }
}
