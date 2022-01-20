using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ReservaCine.Models
{
    public abstract class Usuario
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Ingrese un nombre válido")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre debe tener un mínimo de 3 letras.")]
        public String Nombre { get; set; }

        [Required(ErrorMessage = "Ingrese un apellido válido")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "El apellido debe tener un mínimo de 3 letras")]
        public String Apellido { get; set; }

        [Required(ErrorMessage = "Ingrese un DNI válido")]
        [Range(10000000, 99999999999, ErrorMessage = "Ingrese DNI válido")]
        public long DNI { get; set; }

        [Required(ErrorMessage = "Ingrese un Email válido")]
        [EmailAddress(ErrorMessage = "Formato de Email inválido")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "El email es inválido")]
        public String Email { get; set; }

        [Required(ErrorMessage = "Ingrese un domicilio válido")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Domicilio inválido.")]
        public String Domicilio { get; set; }

        [Required(ErrorMessage = "Ingrese un número de Teléfono")]
        [Range(000000000, 99999999999, ErrorMessage = "Teléfono incorrecto")]
        [DisplayName("Teléfono")]
       
        public long Telefono { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [DisplayName("Fecha de Alta")]
        public DateTime FechaAlta { get; set; }

        
        [DisplayName("Nombre de Usuario")]
        public String NombreUsuario { get; set; }

        
        [DisplayName("Contraseña")]
        [ScaffoldColumn(false)] //para ocultar en columna
        public byte[] Password { get; set; }

       [Required]
       public abstract Rol Rol { get; }
        
        public Usuario()
        {
            FechaAlta = DateTime.Now;
        }



    }
}
         