using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ReservaCine.Models
{
    public class Pelicula
    {
        [Key]
        public Guid Id { get; set; }

        [DisplayName("Fecha de estreno")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime FechaLanzamiento { get; set; }

        [Required(ErrorMessage = "Ingrese un Nombre de película válido")]
        [StringLength(50, MinimumLength = 2,ErrorMessage ="El nombre de la película deber contener al menos 2 caracteres")]
        [DisplayName("Nombre de la película")]
        public String Titulo { get; set; }

        [Required(ErrorMessage = "Ingrese una Descripción")]
        [StringLength(6000, MinimumLength = 10, ErrorMessage ="La descripción debe tener un mínimo de 10 caracteres")]
        [DisplayName("Descripción")]
        public String Descripcion { get; set; }

        [Required(ErrorMessage = "Ingrese una Duración")]
        [Range(60,200, ErrorMessage = "La duración deber ser entre 60 y 200 minutos")]
        [DisplayName("Duración")]
        public int Duracion { get; set; }

      
       public Pelicula ()
        {

        }

        [Required(ErrorMessage = "Seleccione un Genero")]
        [ForeignKey(nameof(Genero))]
        public Guid GeneroId { get; set; }
        public Genero Genero { get; set; }

       public IEnumerable<Funcion> Funciones { get; set; } 
    }

      

}
