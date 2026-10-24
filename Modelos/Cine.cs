namespace Planazo.Modelos
{
    /// <summary>
    /// Representa una función de cine. Hereda de Evento y mantiene el precio base sin recargo ni descuento.
    /// </summary>
    public class Cine : Evento
    {
        public string Sala { get; private set; }
        public string Formato { get; private set; }
        public int DuracionMinutos { get; private set; }

        public override string TipoEvento => "Cine";

        public Cine(int id, string nombre, DateTime fecha, decimal precioBase, int capacidadMaxima,
                    string sala = "Sala 1", string formato = "2D Digital", int duracionMinutos = 120)
            : base(id, nombre, fecha, precioBase, capacidadMaxima)
        {
            Sala = string.IsNullOrWhiteSpace(sala) ? "Sala General" : sala.Trim();
            Formato = string.IsNullOrWhiteSpace(formato) ? "2D Estándar" : formato.Trim();
            DuracionMinutos = duracionMinutos > 0 ? duracionMinutos : 120;
        }

        /// <summary>
        /// El cine mantiene el precio base (regla de negocio especificada).
        /// </summary>
        public override decimal CalcularPrecioFinal()
        {
            return PrecioBase;
        }

        public override string MostrarDetalle()
        {
            return base.MostrarDetalle() + $"\n  Sala: {Sala} | Formato: {Formato} | Duración: {DuracionMinutos} min";
        }
    }
}
