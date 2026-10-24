namespace Planazo.Excepciones
{
    /// <summary>
    /// Excepción lanzada cuando se intenta consultar o cancelar una reserva inexistente.
    /// </summary>
    public class ReservaNoEncontradaException : PlanazoException
    {
        public int ReservaId { get; }

        public ReservaNoEncontradaException(int reservaId)
            : base($"No se encontró ninguna reserva registrada con el identificador #{reservaId}.")
        {
            ReservaId = reservaId;
        }
    }
}
