namespace Planazo.Modelos
{
    /// <summary>
    /// Representa un taller educativo o formativo. Hereda de Evento y aplica un descuento del 10 %
    /// como estímulo educativo.
    /// </summary>
    public class Taller : Evento
    {
        public string Instructor { get; private set; }
        public bool IncluyeMateriales { get; private set; }
        public string Nivel { get; private set; }

        public override string TipoEvento => "Taller";

        public Taller(int id, string nombre, DateTime fecha, decimal precioBase, int capacidadMaxima,
                      string instructor, bool incluyeMateriales = true, string nivel = "Intermedio")
            : base(id, nombre, fecha, precioBase, capacidadMaxima)
        {
            Instructor = string.IsNullOrWhiteSpace(instructor) ? "Instructor Principal" : instructor.Trim();
            IncluyeMateriales = incluyeMateriales;
            Nivel = string.IsNullOrWhiteSpace(nivel) ? "General" : nivel.Trim();
        }

        /// <summary>
        /// Aplica un descuento del 10 % sobre el precio base (regla de negocio especificada).
        /// Ejemplo: $40.000 -> $36.000
        /// </summary>
        public override decimal CalcularPrecioFinal()
        {
            return Math.Round(PrecioBase * 0.90m, 2);
        }

        public override string MostrarDetalle()
        {
            return base.MostrarDetalle() + $"\n  Instructor: {Instructor} | Nivel: {Nivel} | Materiales incluidos: {(IncluyeMateriales ? "Sí" : "No")} (Descuento 10% aplicado)";
        }
    }
}
