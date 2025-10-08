using System.ComponentModel.DataAnnotations;

namespace BlogMVC.Entidades
{
    public class Comentario
    {
        public int Id { get; set; } // Id del comentario
        public int EntradaId { get; set; } // Id de la entrada a la que pertenece el comentario
        public Entrada? Entrada { get; set; } // Entrada a la que pertenece el comentario
        [Required]
        public string Cuerpo { get; set; } = string.Empty; // Contenido del comentario
        public DateTime FechaPublicacion { get; set; } // Fecha en que se publicó el comentario
        public string? UsuarioId { get; set; } // Id del usuario que hizo el comentario
        public Usuario? Usuario { get; set; } // Usuario que hizo el comentario
        public bool Borrado { get; set; } // Indica si el comentario ha sido borrado (Para saber cuál mostrar. Se mantiene el comentario para temas de auditoría.)

    }
}
