using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ReservaCine.Models
{
    public class Reserva
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey(nameof(Funcion))]
        public Guid FuncionId { get; set; }
        [DisplayName("Función")]
        public Funcion Funcion { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [DisplayName("Fecha de Reserva")]
        public DateTime FechaAlta { get; set; }

        [DisplayName("Cliente")]
        [Required]
        [ForeignKey (nameof(Cliente))]
        public Guid ClienteId { get; set; }
        public Cliente Cliente { get; set; }

        [Required (ErrorMessage = "Ingrese cantidad válida de butacas")]
        [Range(1, 12, ErrorMessage = "Cantidad fuera de rango disponible")]
        [DisplayName("Cantidad de butacas")]
        public int CantidadButacas { get; set; }

        public bool Activa { get; set; }


     }

}