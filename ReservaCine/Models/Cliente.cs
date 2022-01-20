using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservaCine.Models
{

    public class Cliente : Usuario
    {
        public IEnumerable<Reserva> Reservas { get; set; }

        public override Rol Rol => Rol.Cliente;

        public Cliente ()
        {

        }
    }
}