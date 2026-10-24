namespace Planazo.Excepciones
{
    /// <summary>
    /// Excepción lanzada cuando se intenta cancelar una reserva que ya se encontraba cancelada.
    /// </summary>
    public class ReservaYaCanceladaException : PlanazoException
    {
        public int ReservaId { get; }

        public ReservaYaCanceladaException(int reservaId)
            : base($"La reserva con ID #{reservaId} ya se encuentra cancelada previamente. No es posible cancelarla nuevamente.")
        {
            ReservaId = reservaId;
        }
    }
}
