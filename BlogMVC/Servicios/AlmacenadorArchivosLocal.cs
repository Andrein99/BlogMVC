
namespace BlogMVC.Servicios
{
    public class AlmacenadorArchivosLocal : IAlmacenadorArchivos
    {
        private readonly IWebHostEnvironment env;
        private readonly IHttpContextAccessor httpContextAccessor;

        public AlmacenadorArchivosLocal(IWebHostEnvironment env,
            IHttpContextAccessor httpContextAccessor)
        {
            this.env = env;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> Almacenar(string contenedor, IFormFile archivo)
        {
            var extension = Path.GetExtension(archivo.FileName);
            var nombreArchivo = $"{Guid.NewGuid()}{extension}";
            string folder = Path.Combine(env.WebRootPath, contenedor);

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            string ruta = Path.Combine(folder, nombreArchivo);

            using (var ms = new MemoryStream()) 
            {
                await archivo.CopyToAsync(ms); // Copiar el archivo al MemoryStream
                var contenido = ms.ToArray(); // Leer el archivo como un arreglo de bytes
                await File.WriteAllBytesAsync(ruta, contenido); // Escribir el archivo en el disco
            }

            var request = httpContextAccessor.HttpContext!.Request; // Obtener la URL del servidor
            var url = $"${request.Scheme}://{request.Host}";
            var urlArchivo = Path.Combine(url, contenedor, nombreArchivo).Replace("\\", "/"); // Reemplazar las barras invertidas por diagonales para que funcione en URL
            return urlArchivo;
        }

        public Task Borrar(string? ruta, string contenedor)
        {
            if (string.IsNullOrEmpty(ruta)) // Si la ruta es nula o vacía, no hacer nada
            {
                return Task.CompletedTask;
            }

            var nombreArchivo = Path.GetFileName(ruta); // Obtener solo el nombre del archivo
            var directorioArchivo = Path.Combine(env.WebRootPath, contenedor, nombreArchivo); // Obtener la ruta completa del archivo

            if (File.Exists(directorioArchivo)) // Si el archivo existe, borrarlo
            {
                File.Delete(directorioArchivo); // Borrar el archivo
            }
            return Task.CompletedTask;
        }
    }
}
