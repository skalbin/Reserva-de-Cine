using ReservaCine.Controllers;
using ReservaCine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ReservaCine.Data
{
	public class InicializacionDeDatos
	{
		private static readonly ISeguridad seguridad = new SeguridadBasica();
		public static void Inicializar(ReservaCineContext context)
		{		
			context.Database.EnsureCreated();

			if (context.Cliente.Any())
			{
				// Si ya hay datos aqui, significa que ya los hemos creado previamente
				return;
			}

			var newCliente = new Cliente();
			newCliente.Id = Guid.NewGuid();
			newCliente.Nombre = "San";
			newCliente.Apellido = "Albín";
			newCliente.DNI = 1111111111;
			newCliente.Email = "san.a@hotmail.com";
			newCliente.Domicilio = "as 1245";
			newCliente.Telefono = 3234235245;
			newCliente.NombreUsuario = "assa";
			newCliente.Password = seguridad.EncriptarPass("4564");



			newCliente.FechaAlta = DateTime.Now;
			context.Cliente.Add(newCliente);
			context.SaveChanges();

			var Cliente = context.Cliente.First();

			if (context.Empleado.Any())
			{
				return;
			}

			var newEmpleado = new Empleado();
			newEmpleado.Id = Guid.NewGuid();
			newEmpleado.Nombre = "Patricio";
			newEmpleado.Apellido = "Castellano";
			newEmpleado.DNI = 41623687;
			newEmpleado.Email = "patriciocastell@hotmail.com";
			newEmpleado.Domicilio = "Av etc. 245";
			newEmpleado.Telefono = 1130659588;
			newEmpleado.Legajo = 11564665;
			newEmpleado.NombreUsuario = "Patokpo123";
			newEmpleado.Password = seguridad.EncriptarPass("pato1998");
			newEmpleado.FechaAlta = DateTime.Now;
			context.Empleado.Add(newEmpleado);
			context.SaveChanges();

			var Empleado = context.Empleado.First();

			

			if (context.Genero.Any())
			{
				return;
			}

			var nuevoGenero = new Genero();
			nuevoGenero.Nombre = "Acción";
			nuevoGenero.Id = Guid.NewGuid();
			nuevoGenero.Peliculas = new List<Pelicula>();
			context.Genero.Add(nuevoGenero);
			context.SaveChanges();

			var genero = context.Genero.First();

			if (context.Pelicula.Any())
			{
				return;
			}

			var nuevaPelicula = new Pelicula();
			nuevaPelicula.FechaLanzamiento = DateTime.Now.Date;
			nuevaPelicula.Titulo = "La viuda Negra";
			nuevaPelicula.Id = Guid.NewGuid();
			nuevaPelicula.Descripcion = "Una de las preferidas de las series de Marvel, viene a vengarse de su pasado";
			nuevaPelicula.GeneroId = nuevoGenero.Id;
			nuevaPelicula.Duracion = 180;
			context.Pelicula.Add(nuevaPelicula);
			context.SaveChanges();

			var pelicula = context.Pelicula.First();

			if (context.TipoSala.Any())
			{
				return;
			}

			var newTipoSala = new TipoSala
			{
				Id = Guid.NewGuid(),
				Nombre = "Sala 3D",
				Precio = 2000,


			};
			context.TipoSala.Add(newTipoSala);
			context.SaveChanges();

			var TipoSala = context.TipoSala.First();



			if (context.Sala.Any())
			{
				return;
			}

			var newSala = new Sala();
			newSala.Id = Guid.NewGuid();
			newSala.CapacidadButacas = 125;
			newSala.Numero = 1;
			newSala.TipoSalaId = newTipoSala.Id;

			context.Sala.Add(newSala);
			context.SaveChanges();

			var Sala = context.Sala.First();

			if (context.Funcion.Any())
			{
				// Si ya hay datos aqui, significa que ya los hemos creado previamente
				return;
			}

			var nuevaFuncion = new Funcion();
			nuevaFuncion.Id = Guid.NewGuid();
			nuevaFuncion.CantButacasDisponibles = 20;
			nuevaFuncion.Confirmar = true;
			nuevaFuncion.Descripcion = "Película ATP";
			nuevaFuncion.PeliculaId = nuevaPelicula.Id;
			nuevaFuncion.SalaId = newSala.Id;
			nuevaFuncion.Fecha = DateTime.Now.AddDays(14);
			nuevaFuncion.Hora = new DateTime(1,1,1, 16, 40,0);

			context.Funcion.Add(nuevaFuncion);
			context.SaveChanges();

			var Funcion = context.Funcion.First();

			if (context.Reserva.Any())
			{
				// Si ya hay datos aqui, significa que ya los hemos creado previamente
				return;
			}

			var nuevaReserva = new Reserva();
			nuevaReserva.Id = Guid.NewGuid();
			nuevaReserva.CantidadButacas = 4;
			nuevaReserva.ClienteId = newCliente.Id;
			nuevaReserva.Activa = true;
			nuevaReserva.FechaAlta = DateTime.Now;
			nuevaReserva.FuncionId = nuevaFuncion.Id;
		
			context.Reserva.Add(nuevaReserva);
			context.SaveChanges();

			var Reserva = context.Reserva.First();
						

			
		}



		}

	}

		

