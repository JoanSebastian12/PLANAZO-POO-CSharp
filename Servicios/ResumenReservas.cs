namespace Planazo.Servicios
{
    /// <summary>
    /// Objeto de transferencia que consolida el resumen de reservas activas y canceladas.
    /// </summary>
    public class ResumenReservas
    {
        public int TotalReservasActivas { get; set; }
        public int TotalReservasCanceladas { get; set; }
        public int TotalEntradasReservadasActivas { get; set; }
        public decimal ValorTotalReservasActivas { get; set; }

        public override string ToString()
        {
            return $"Reservas Activas: {TotalReservasActivas} | Entradas Activas: {TotalEntradasReservadasActivas} | Valor Total: {ValorTotalReservasActivas:C} | Canceladas: {TotalReservasCanceladas}";
        }
    }
}
