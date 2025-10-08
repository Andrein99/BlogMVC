using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BlogMVC.Entidades
{
    public class Entrada
    {
        public int Id { get; set; } // Id de la entrada
        public required string Titulo { get; set; } // Título de la entrada
        public required string Cuerpo { get; set; } // Contenido de la entrada
        [Unicode(false)] // Indica que el campo no debe usar Unicode (ahorra espacio si solo se usan caracteres ASCII) - En la base de datos se crea como VARCHAR en lugar de NVARCHAR
        public string? PortadaUrl { get; set; } // URL de la imagen de portada
        public DateTime FechaPublicacion { get; set; } // Fecha en que se publicó la entrada
        [Required]
        public string UsuarioCreacionId { get; set; } = null!; // Id del usuario que creó la entrada
        public Usuario? UsuarioCreacion { get; set; } // Usuario que creó la entrada
        public string? UsuarioActualizacionId { get; set; } // Id del usuario que actualizó la entrada por última vez
        public Usuario? UsuarioActualizacion { get; set; } // Usuario que actualizó la entrada por última vez
        public bool Borrado { get; set; } // Indica si el comentario ha sido borrado (Para saber cuál mostrar. Se mantiene el comentario para temas de auditoría.)
        public List<Comentario> Comentarios { get; set; } = []; // Comentarios asociados a la entrada. Propiedad de navegación.
    }
}
