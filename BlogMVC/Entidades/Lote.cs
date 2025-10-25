namespace BlogMVC.Entidades
{
    public class Lote
    {
        public required string Id { get; set; }
        public required string Status { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
