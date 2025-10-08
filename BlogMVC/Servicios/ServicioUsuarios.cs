using BlogMVC.Entidades;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace BlogMVC.Servicios
{
    public interface IServicioUsuarios
    {
        string? ObtenerUsuarioId();
    }

    public class ServicioUsuarios : IServicioUsuarios
    {
        private readonly UserManager<Usuario> userManager;
        private readonly HttpContext httpContext;

        public ServicioUsuarios(IHttpContextAccessor httpContextAccessor,
            UserManager<Usuario> userManager)
        {
            this.userManager = userManager;
            httpContext = httpContextAccessor.HttpContext!;
        }

        public string? ObtenerUsuarioId()
        {
            var idClaim = httpContext.User.Claims // Obtiene los claims del usuario autenticado
                .Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault(); // Busca el claim que contiene el Id del usuario

            if (idClaim is null) // Si no se encuentra el claim, retorna null
            {
                return null;
            }

            return idClaim.Value; // Retorna el Id del usuario
        }
    }
}
