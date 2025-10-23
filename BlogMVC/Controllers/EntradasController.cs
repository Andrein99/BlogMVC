using BlogMVC.Datos;
using BlogMVC.Entidades;
using BlogMVC.Models;
using BlogMVC.Servicios;
using BlogMVC.Utilidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogMVC.Controllers
{
    public class EntradasController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IAlmacenadorArchivos almacenadorArchivos;
        private readonly IServicioUsuarios servicioUsuarios;
        private readonly IServicioChat servicioChat;
        private readonly IServicioImagenes servicioImagenes;
        private readonly IWebHostEnvironment env;
        private readonly string contenedor = "entradas"; // Nombre del contenedor donde se almacenan las imágenes de las entradas

        public EntradasController(ApplicationDbContext context,
            IAlmacenadorArchivos almacenadorArchivos,
            IServicioUsuarios servicioUsuarios,
            IServicioChat servicioChat,
            IServicioImagenes servicioImagenes,
            IWebHostEnvironment env)
        {
            this.context = context;
            this.almacenadorArchivos = almacenadorArchivos;
            this.servicioUsuarios = servicioUsuarios;
            this.servicioChat = servicioChat;
            this.servicioImagenes = servicioImagenes;
            this.env = env;
        }

        [HttpGet]
        public async Task<IActionResult> Detalle(int id) // Acción para ver el detalle de una entrada
        {
            var entrada = await context.Entradas // Consulta la base de datos para obtener la entrada con el Id proporcionado
                .IgnoreQueryFilters() // Ignorar los filtros globales (por ejemplo, para incluir entradas borradas)
                .Include(x => x.UsuarioCreacion) // Incluir el usuario que creó la entrada
                .Include(x => x.Comentarios) // Incluir los comentarios asociados a la entrada
                    .ThenInclude(x => x.Usuario) // Incluir el usuario que creó cada comentario
                .FirstOrDefaultAsync(x => x.Id == id); // Buscar la entrada por Id

            if (entrada is null)
            {
                return RedirectToAction("NoEncontrado", "Home"); // Si no se encuentra la entrada, redirige a una página de "No Encontrado"
            }

            var puedeEditarEntradas = await servicioUsuarios.PuedeUsuarioHacerCRUDEntradas(); // Verifica si el usuario autenticado puede hacer CRUD de entradas

            if (entrada.Borrado && !puedeEditarEntradas) // Si la entrada está borrada y el usuario no tiene permisos para editar entradas
            {
                var urlRetorno = HttpContext.ObtenerUrlRetorno(); // Obtiene la URL de retorno
                return RedirectToAction("Login", "Usuarios", new { urlRetorno }); // Redirige a la página de login
            }

            var puedeBorrarComentarios = await servicioUsuarios.PuedeUsuarioBorrarComentarios(); // Verifica si el usuario autenticado puede borrar comentarios

            var usuarioId = servicioUsuarios.ObtenerUsuarioId(); // Obtiene el Id del usuario autenticado

            var modelo = new EntradaDetalleViewModel
            {
                Id = entrada.Id,
                Titulo = entrada.Titulo,
                Cuerpo = entrada.Cuerpo,
                FechaPublicacion = entrada.FechaPublicacion,
                PortadaUrl = entrada.PortadaUrl,
                EscritoPor = entrada.UsuarioCreacion!.Nombre,
                MostrarBotonEdicion = puedeEditarEntradas,
                EntradaBorrada = entrada.Borrado,
                Comentarios = entrada.Comentarios.Select(c => new ComentarioViewModel
                {
                    Id = c.Id,
                    Cuerpo = c.Cuerpo,
                    EscritoPor = c.Usuario!.Nombre,
                    FechaPublicacion = c.FechaPublicacion,
                    MostrarBotonBorrar = puedeBorrarComentarios || usuarioId == c.UsuarioId
                })
            }; // Crea el modelo para la vista con los datos de la entrada

            return View(modelo); // Retorna la vista con el modelo de la entrada
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

            } else if (modelo.ImagenPortadaIA is not null)
            {
                var archivo = Base64AIFormFile(modelo.ImagenPortadaIA);
                portadaUrl = await almacenadorArchivos.Almacenar(contenedor, archivo);
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

        [HttpGet]
        [Authorize(Roles = $"{Constantes.RolAdmin},{Constantes.CRUDEntradas}")] // Solo usuarios con rol Admin o permiso CRUDEntradas pueden acceder
        public async Task<IActionResult> Editar(int id) // Acción para editar una entrada existente
        {
            var entrada = await context.Entradas // Consulta la base de datos para obtener la entrada con el Id proporcionado
                .IgnoreQueryFilters() // Ignorar los filtros globales (por ejemplo, para incluir entradas borradas)
                .FirstOrDefaultAsync(e => e.Id == id); // Buscar la entrada por Id
            if (entrada is null)
            {
                return RedirectToAction("NoEncontrado", "Home"); // Si no se encuentra la entrada, redirige a una página de "No Encontrado"
            }
            var modelo = new EntradaEditarViewModel
            {
                Id = entrada.Id,
                Titulo = entrada.Titulo,
                Cuerpo = entrada.Cuerpo,
                ImagenPortadaActual = entrada.PortadaUrl
            }; // Crea el modelo para la vista con los datos de la entrada
            return View(modelo); // Retorna la vista con el modelo de la entrada
        }

        [HttpPost]
        [Authorize(Roles = $"{Constantes.RolAdmin},{Constantes.CRUDEntradas}")] // Solo usuarios con rol Admin o permiso CRUDEntradas pueden acceder
        public async Task<IActionResult> Editar(EntradaEditarViewModel modelo) // Acción para editar una entrada existente
        {
            if (!ModelState.IsValid)
            {
                return View(modelo); // Si el modelo no es válido, retorna la vista con el modelo para mostrar errores
            }
            var entradaDB = await context.Entradas // Consulta la base de datos para obtener la entrada con el Id proporcionado
                .IgnoreQueryFilters() // Ignorar los filtros globales (por ejemplo, para incluir entradas borradas)
                .FirstOrDefaultAsync(x => x.Id == modelo.Id); // Buscar la entrada por Id
            if (entradaDB is null)
            {
                return RedirectToAction("NoEncontrado", "Home"); // Si no se encuentra la entrada, redirige a una página de "No Encontrado"
            }
            string? portadaUrl = null; // Inicializa la URL de la portada como null
            if (modelo.ImagenPortada is not null) // Si se proporcionó una nueva imagen de portada
            {
                portadaUrl = await almacenadorArchivos.Editar(modelo.ImagenPortadaActual,
                    contenedor, modelo.ImagenPortada); // Almacena la nueva imagen de portada y obtiene la URL
            } else if (modelo.ImagenPortadaIA is not null)
            {
                var archivo = Base64AIFormFile(modelo.ImagenPortadaIA);
                portadaUrl = await almacenadorArchivos.Editar(modelo.ImagenPortadaActual, contenedor, archivo);
            }
            else if (modelo.ImagenRemovida) // Si se indicó que se quiere remover la imagen de portada
            {
                await almacenadorArchivos.Borrar(modelo.ImagenPortadaActual, contenedor); // Borra la imagen de portada actual
            }
            else // Si no se proporcionó una nueva imagen ni se indicó que se quiere remover la imagen
            {
                portadaUrl = entradaDB.PortadaUrl; // Mantiene la URL de la portada actual
            }
            
            string usuarioId = servicioUsuarios.ObtenerUsuarioId()!; // Obtiene el Id del usuario autenticado
            entradaDB.Titulo = modelo.Titulo; // Actualiza el título de la entrada
            entradaDB.Cuerpo = modelo.Cuerpo; // Actualiza el cuerpo de la entrada
            entradaDB.PortadaUrl = portadaUrl; // Actualiza la URL de la portada
            entradaDB.UsuarioActualizacionId = usuarioId; // Actualiza el Id del usuario que actualizó la entrada
            await context.SaveChangesAsync(); // Guarda los cambios en la base de datos
            return RedirectToAction("Detalle", new { id = entradaDB.Id }); // Redirige a la acción Detalle para ver la entrada editada
        }

        [HttpPost]
        [Authorize(Roles = $"{Constantes.RolAdmin}, {Constantes.CRUDEntradas}")]
        public async Task<IActionResult> Borrar(int id, bool borrado)
        {
            var entradaDB = await context.Entradas
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(x => x.Id == id);
            if (entradaDB is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            entradaDB.Borrado = borrado;
            await context.SaveChangesAsync();
            return RedirectToAction("Detalle", new { id = entradaDB.Id });
        }

        [HttpGet]
        public async Task GenerarCuerpo([FromQuery] string titulo)
        {
            if (string.IsNullOrWhiteSpace(titulo))
            {
                Response.StatusCode = StatusCodes.Status400BadRequest;
                await Response.WriteAsync("El título es obligatorio.");
            }

            await foreach (var segmento in servicioChat.GenerarCuerpoStream(titulo))
            {
                await Response.WriteAsync(segmento);
                await Response.Body.FlushAsync();
            }
        }

        [HttpGet]
        public async Task<IActionResult> GenerarImagen([FromQuery] string titulo)
        {
            if (string.IsNullOrWhiteSpace(titulo))
            {
                return BadRequest("El título es obligatorio.");
            }

            // Se utiliza este código para usar una imagen como placeholder durante el desarrollo para no llamar a la API y gastar dinero.
            //if (env.IsDevelopment())
            //{
            //    var rutaImagen = Path.Combine(env.WebRootPath, "img", "IA-NoImage.png");
            //    var imagenBytes = await System.IO.File.ReadAllBytesAsync(rutaImagen);
            //    return File(imagenBytes, "image/png");
            //}

            var bytes = await servicioImagenes.GenerarPortadaEntrada(titulo);

            return File(bytes, "image/png");
        }

        private IFormFile Base64AIFormFile(string base64)
        {
            byte[] bytes = Convert.FromBase64String(base64);
            var stream = new MemoryStream(bytes);
            IFormFile archivo = new FormFile(stream, 0, bytes.Length, "imagen", "imagen.png");
            return archivo;
        }
    }
}
