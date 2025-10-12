namespace BlogMVC.Utilidades
{
    public static class HttpContextExtensions
    {
        public static string ObtenerUrlRetorno(this HttpContext httpContext)
        {
            ArgumentNullException.ThrowIfNull(httpContext); // Verifica que el contexto HTTP no sea nulo
            return httpContext.Request.Path + httpContext.Request.QueryString; // Construye la URL de retorno combinando la ruta y la cadena de consulta
        }
    }
}
