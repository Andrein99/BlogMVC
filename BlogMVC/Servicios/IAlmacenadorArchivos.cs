namespace BlogMVC.Servicios
{
    public interface IAlmacenadorArchivos
    {
        Task<string> Almacenar(string contenedor, IFormFile archivo); // Almacena un archivo y devuelve la ruta donde se almacenó
        Task Borrar(string? ruta, string contenedor); // Borra un archivo
        async Task<string> Editar(string? ruta, string contenedor, IFormFile archivo) 
        { 
            await Borrar(ruta, contenedor);
            return await Almacenar(contenedor, archivo);
        } // Edita un archivo: borra el anterior y almacena el nuevo
    }
}
