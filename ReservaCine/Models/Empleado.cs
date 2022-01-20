using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ReservaCine.Models
{
    public class Empleado : Usuario
    {
        public override Rol Rol => Rol.Administrador;

        [Required(ErrorMessage ="Ingrese el legajo")]
        [Range(0, 99999999999, ErrorMessage = "Ingrese un número válido")]
        public int Legajo { get; set; }
         

        public Empleado ()
        {

        }
    }
}
        
    
