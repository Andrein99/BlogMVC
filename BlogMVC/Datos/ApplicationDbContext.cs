using BlogMVC.Entidades;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlogMVC.Datos
{
    public class ApplicationDbContext : IdentityDbContext<Usuario>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected ApplicationDbContext()
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Configuraciones adicionales del modelo pueden ir aquí
            builder.Entity<Comentario>().HasQueryFilter(c => !c.Borrado); // Filtro global para excluir comentarios borrados
            builder.Entity<Entrada>().HasQueryFilter(e => !e.Borrado);
        }

        public DbSet<Entrada> Entradas { get; set; }
        public DbSet<Comentario> Comentarios { get; set; }
        public DbSet<Lote> Lotes { get; set; }
    }
}
