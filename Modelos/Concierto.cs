namespace Planazo.Modelos
{
    /// <summary>
    /// Representa un concierto musical. Hereda de Evento y aumenta el precio base en un 15 %
    /// debido a costos de producción técnica, artistas y logística.
    /// </summary>
    public class Concierto : Evento
    {
        public string ArtistaOAgrupacion { get; private set; }
        public string Zona { get; private set; }
        public bool TieneAsientosNumerados { get; private set; }

        public override string TipoEvento => "Concierto";

        public Concierto(int id, string nombre, DateTime fecha, decimal precioBase, int capacidadMaxima,
                         string artistaOAgrupacion, string zona = "General", bool tieneAsientosNumerados = false)
            : base(id, nombre, fecha, precioBase, capacidadMaxima)
        {
            ArtistaOAgrupacion = string.IsNullOrWhiteSpace(artistaOAgrupacion) ? "Varios Artistas" : artistaOAgrupacion.Trim();
            Zona = string.IsNullOrWhiteSpace(zona) ? "General" : zona.Trim();
            TieneAsientosNumerados = tieneAsientosNumerados;
        }

        /// <summary>
        /// Aplica un aumento del 15 % sobre el precio base (regla de negocio especificada).
        /// Ejemplo: $50.000 -> $57.500
        /// </summary>
        public override decimal CalcularPrecioFinal()
        {
            return Math.Round(PrecioBase * 1.15m, 2);
        }

        public override string MostrarDetalle()
        {
            return base.MostrarDetalle() + $"\n  Artista: {ArtistaOAgrupacion} | Zona: {Zona} | Numerado: {(TieneAsientosNumerados ? "Sí" : "No")} (+15% producción incluido)";
        }
    }
}
