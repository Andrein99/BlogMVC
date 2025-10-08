using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlogMVC.Utilidades
{
    public static class Seeding
    {
        private static List<string> roles = new List<string>
        {
            Servicios.Constantes.RolAdmin,
            Servicios.Constantes.CRUDEntradas,
            Servicios.Constantes.BorraComentarios
        };

        public static void Aplicar(DbContext context, bool _) // Método síncrono para aplicar los roles
        {
            foreach (var rol in roles)
            {
                var rolDB = context.Set<IdentityRole>().FirstOrDefault(x => x.Name == rol); // Buscar el rol en la base de datos

                if (rolDB is null) // Si el rol no existe en la base de datos
                {
                    context.Set<IdentityRole>().Add(new IdentityRole
                    {
                        Name = rol,
                        NormalizedName = rol.ToUpper()
                    }); // Agregar el nuevo rol al contexto
                    context.SaveChanges(); // Guardar los cambios
                }
            }
        }

        public static async Task AplicarAsync(DbContext context, bool _, 
            CancellationToken cancellationToken) // Método asíncrono para aplicar los roles
        {
            foreach (var rol in roles)
            {
                var rolDB = await context.Set<IdentityRole>().FirstOrDefaultAsync(x => x.Name == rol); // Buscar el rol en la base de datos de forma asíncrona

                if (rolDB is null) // Si el rol no existe en la base de datos
                {
                    context.Set<IdentityRole>().Add(new IdentityRole
                    {
                        Name = rol,
                        NormalizedName = rol.ToUpper()
                    }); // Agregar el nuevo rol al contexto
                    await context.SaveChangesAsync(cancellationToken); // Guardar los cambios de forma asíncrona
                }
            }
        }
    }
}
