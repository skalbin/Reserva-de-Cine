using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ReservaCine.Models
{
    public class TipoSala
    {   
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Ingrese un nombre")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Ingrese un nombre valido")]
        [DisplayName("Nombre de la sala")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Ingrese un precio valido")]
        public double Precio { get; set; }

        public IEnumerable<Sala> Salas { get; set; }
    }
}
