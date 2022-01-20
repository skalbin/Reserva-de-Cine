using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReservaCine.Models
{

    public class Genero
    {
        [Key]
        public Guid Id { get; set; }
        public string Nombre { get; set; }


        public Genero()
        {
        }


        public IEnumerable<Pelicula> Peliculas { get; set; }
    }
}