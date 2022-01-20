using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ReservaCine.Models;

namespace ReservaCine.Data
{
    public class ReservaCineContext : DbContext
    {
        public ReservaCineContext(DbContextOptions<ReservaCineContext> options)
            : base(options)
        {
        }

        public DbSet<ReservaCine.Models.Pelicula> Pelicula { get; set; }
        public DbSet<ReservaCine.Models.Genero> Genero { get; set; }

        public DbSet<ReservaCine.Models.Funcion> Funcion { get; set; }

        public DbSet<ReservaCine.Models.Usuario> Usuario { get; set; }

        public DbSet<ReservaCine.Models.Cliente> Cliente { get; set; }

        public DbSet<ReservaCine.Models.Empleado> Empleado { get; set; }
        public DbSet<ReservaCine.Models.Reserva> Reserva { get; set; }

        public DbSet<ReservaCine.Models.Sala> Sala { get; set; }

        public DbSet<ReservaCine.Models.TipoSala> TipoSala { get; set; }

        

         
    }
}
