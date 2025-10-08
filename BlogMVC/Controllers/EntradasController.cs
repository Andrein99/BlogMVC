using BlogMVC.Datos;
using BlogMVC.Entidades;
using BlogMVC.Models;
using BlogMVC.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogMVC.Controllers
{
    public class EntradasController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IAlmacenadorArchivos almacenadorArchivos;
        private readonly IServicioUsuarios servicioUsuarios;
        private readonly string contenedor = "entradas"; // Nombre del contenedor donde se almacenan las imágenes de las entradas

        public EntradasController(ApplicationDbContext context,
            IAlmacenadorArchivos almacenadorArchivos,
            IServicioUsuarios servicioUsuarios)
        {
            this.context = context;
            this.almacenadorArchivos = almacenadorArchivos;
            this.servicioUsuarios = servicioUsuarios;
        }

        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = $"{Constantes.RolAdmin},{Constantes.CRUDEntradas}")] // Solo usuarios con rol Admin o permiso CRUDEntradas pueden acceder
        public async Task<IActionResult> Crear(EntradaCrearViewModel modelo) // Accion para crear una nueva entrada
        {
            if (!ModelState.IsValid)
            {
                return View(modelo); // Si el modelo no es válido, retorna la vista con el modelo para mostrar errores
            }

            string? portadaUrl = null; // Inicializa la URL de la portada como null

            if (modelo.ImagenPortada is not null) // Si se proporcionó una imagen de portada
            {
                portadaUrl = await almacenadorArchivos.Almacenar(contenedor, modelo.ImagenPortada); // Almacena la imagen de portada y obtiene la URL

            }

            string usuarioId = servicioUsuarios.ObtenerUsuarioId()!; // Obtiene el Id del usuario autenticado

            var entrada = new Entrada
            {
                Titulo = modelo.Titulo,
                Cuerpo = modelo.Cuerpo,
                FechaPublicacion = DateTime.UtcNow,
                PortadaUrl = portadaUrl,
                UsuarioCreacionId = usuarioId
            }; // Crea la nueva entrada

            context.Add(entrada); // Agrega la nueva entrada al contexto
            await context.SaveChangesAsync(); // Guarda los cambios en la base de datos

            return RedirectToAction("Detalle", new { id = entrada.Id }); // Redirige a la acción Detalle para ver la entrada creada
        }
    }
}
