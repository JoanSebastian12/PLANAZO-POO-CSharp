namespace Planazo.Modelos
{
    /// <summary>
    /// Reto adicional 1 (+0,2): Nuevo tipo de evento.
    /// Representa un festival cultural o musical de gran formato.
    /// Hereda de Evento e implementa su propia regla de cálculo sin modificar
    /// las clases existentes (Principio Open/Closed de SOLID).
    /// </summary>
    public class Festival : Evento
    {
        public int NumeroEscenarios { get; private set; }
        public int DiasDuracion { get; private set; }
        public string PatrocinadorPrincipal { get; private set; }

        public override string TipoEvento => "Festival";

        public Festival(int id, string nombre, DateTime fecha, decimal precioBase, int capacidadMaxima,
                        int numeroEscenarios = 3, int diasDuracion = 2, string patrocinadorPrincipal = "PLANAZO Live")
            : base(id, nombre, fecha, precioBase, capacidadMaxima)
        {
            NumeroEscenarios = numeroEscenarios > 0 ? numeroEscenarios : 1;
            DiasDuracion = diasDuracion > 0 ? diasDuracion : 1;
            PatrocinadorPrincipal = string.IsNullOrWhiteSpace(patrocinadorPrincipal) ? "General" : patrocinadorPrincipal.Trim();
        }

        /// <summary>
        /// Aplica un recargo del 20 % sobre el precio base debido a la infraestructura multi-escenario
        /// y producción de eventos de gran escala.
        /// </summary>
        public override decimal CalcularPrecioFinal()
        {
            return Math.Round(PrecioBase * 1.20m, 2);
        }

        public override string MostrarDetalle()
        {
            return base.MostrarDetalle() + $"\n  Escenarios: {NumeroEscenarios} | Duración: {DiasDuracion} día(s) | Patrocinador: {PatrocinadorPrincipal} (+20% multi-escenario incluido)";
        }
    }
}
