using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ReservaCine.Data;
using ReservaCine.Models;

namespace ReservaCine.Controllers
{
    [AllowAnonymous]
    public class UsuarioController : Controller
    {
        private readonly ReservaCineContext _context;
        private readonly ISeguridad seguridad = new SeguridadBasica();

        public UsuarioController(ReservaCineContext context)
        {
            _context = context;
        }

        // GET: Usuario
        public IActionResult Ingresar(string returnUrl)
        {
            TempData["UrlIngreso"] = returnUrl;


            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Ingresar(string NombreUsuario, string Password)
        {
           // Guardamos la URL a la que debemos redirigir al usuario
            var urlIngreso = TempData["UrlIngreso"] as string;

            // Verificamos que ambos esten informados
            if (!string.IsNullOrEmpty(NombreUsuario) && !string.IsNullOrEmpty(Password))
            {

                // Verificamos que exista el usuario
                var user = await _context.Usuario.FirstOrDefaultAsync(u => u.NombreUsuario == NombreUsuario);
                if (user != null)
                {

                    // Verificamos que coincida la contraseña
                    var contraseña = seguridad.EncriptarPass(Password);
                    if (contraseña.SequenceEqual(user.Password))
                    {
                        // Creamos los Claims (credencial de acceso con informacion del usuario)-- cookies
                        ClaimsIdentity identidad = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);

                        // Agregamos a la credencial el nombre de usuario
                        identidad.AddClaim(new Claim(ClaimTypes.Name, NombreUsuario));
                        // Agregamos a la credencial el nombre del estudiante/administrador
                        identidad.AddClaim(new Claim(ClaimTypes.GivenName, user.Nombre));
                        // Agregamos a la credencial el Rol
                        identidad.AddClaim(new Claim(ClaimTypes.Role, user.Rol.ToString()));
                        // Agregamos el Id de Usuario
                        identidad.AddClaim(new Claim("IdDeUsuario", user.Id.ToString()));

                        ClaimsPrincipal principal = new ClaimsPrincipal(identidad);

                        // Ejecutamos el Login
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                        if (!string.IsNullOrEmpty(urlIngreso))
                        {
                            return Redirect(urlIngreso);
                        }
                        else
                        {
                            // Redirigimos a la pagina principal
                            return RedirectToAction("Index", "Home");
                        }
                    }
                }
            }

            ViewBag.ErrorEnLogin = "Verifique el usuario y contraseña";
            TempData["UrlIngreso"] = urlIngreso; // Volvemos a enviarla en el TempData para no perderla
            return View();

        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Salir()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public IActionResult AccesoDenegado()
        {
            return View();
        }


        public IActionResult Registrarse()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registrarse(Cliente usuario, string pass)
        {
            if (ModelState.IsValid)
            {
                if (seguridad.ValidarPass(pass))
                {
                         var existeUser = _context.Cliente
                           .Any(c => usuario.NombreUsuario == c.NombreUsuario);
                    if (!existeUser)
                    {
                        var nuevoCliente = new Cliente
                        {
                            Nombre = usuario.Nombre,
                            Apellido = usuario.Apellido,
                            DNI = usuario.DNI,
                            Domicilio = usuario.Domicilio,
                            Email = usuario.Email,
                            FechaAlta = DateTime.Today,
                            Password = seguridad.EncriptarPass(pass),
                            Id = Guid.NewGuid(),
                            NombreUsuario = usuario.NombreUsuario,
                            Telefono = usuario.Telefono,



                        };


                        _context.Cliente.Add(nuevoCliente);




                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Ingresar));
                    }
                    else
                    {
                        ModelState.AddModelError(nameof(Usuario.NombreUsuario), "Ya existe ese nombre usuario.");
                    }
                }
                else
                {
                    ModelState.AddModelError(nameof(Usuario.Password), "La contraseña no cumple los requisitos");
                }


            }
            return View(usuario);
        }

        


    }
}

