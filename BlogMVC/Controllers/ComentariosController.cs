using BlogMVC.Datos;
using BlogMVC.Entidades;
using BlogMVC.Models;
using BlogMVC.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogMVC.Controllers
{
    public class ComentariosController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IServicioUsuarios servicioUsuarios;

        public ComentariosController(ApplicationDbContext context,
            IServicioUsuarios servicioUsuarios)
        {
            this.context = context;
            this.servicioUsuarios = servicioUsuarios;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Comentar(EntradasComentarViewModel modelo) // Acción para agregar un comentario a una entrada
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("detalle", "entradas", new { id = modelo.Id }); // Si el modelo no es válido, redirige al detalle de la entrada
            }
            var existeEntrada = await context.Entradas.AnyAsync(e => e.Id == modelo.Id);
            if (!existeEntrada)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            var usuarioId = servicioUsuarios.ObtenerUsuarioId()!;
            var comentario = new Comentario
            {
                Cuerpo = modelo.Cuerpo,
                EntradaId = modelo.Id,
                UsuarioId = usuarioId,
                FechaPublicacion = DateTime.UtcNow
            };
            context.Add(comentario); // Agrega el comentario a la base de datos
            await context.SaveChangesAsync();
            return RedirectToAction("detalle", "entradas", new { id = modelo.Id }); // Redirige al detalle de la entrada para ver el comentario agregado
        }
    }
}
