using BlogMVC.Entidades;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace BlogMVC.Servicios
{
    public interface IServicioUsuarios
    {
        string? ObtenerUsuarioId();
        Task<bool> PuedeUsuarioBorrarComentarios();
        Task<bool> PuedeUsuarioHacerCRUDEntradas();
    }

    public class ServicioUsuarios : IServicioUsuarios
    {
        private readonly UserManager<Usuario> userManager;
        private readonly HttpContext httpContext;
        private readonly Usuario usuarioActual;
        private static readonly string[] RolesCRUDEntradas = { Constantes.RolAdmin, Constantes.CRUDEntradas };
        private static readonly string[] RolesBorrarComentarios = { Constantes.RolAdmin, Constantes.BorraComentarios };

        public ServicioUsuarios(IHttpContextAccessor httpContextAccessor,
            UserManager<Usuario> userManager)
        {
            this.userManager = userManager;
            httpContext = httpContextAccessor.HttpContext!;
            usuarioActual = new Usuario { Id = ObtenerUsuarioId()! };
        }

        public async Task<bool> PuedeUsuarioHacerCRUDEntradas()
        {
            return await UsuarioEstaEnRol(RolesCRUDEntradas); // Verifica si el usuario está en alguno de los roles que permiten CRUD de entradas
        }

        public async Task<bool> PuedeUsuarioBorrarComentarios()
        {
            return await UsuarioEstaEnRol(RolesBorrarComentarios); // Verifica si el usuario está en alguno de los roles que permiten borrar comentarios
        }

        private async Task<bool> UsuarioEstaEnRol(IEnumerable<string> roles) // Verifica si el usuario actual está en alguno de los roles proporcionados
        {
            var rolesUsuario = await userManager.GetRolesAsync(usuarioActual); // Obtiene los roles del usuario actual
            return roles.Any(rolesUsuario.Contains); // Verifica si alguno de los roles proporcionados está en los roles del usuario
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
