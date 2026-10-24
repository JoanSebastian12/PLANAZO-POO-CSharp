namespace Planazo.Excepciones
{
    /// <summary>
    /// Excepción lanzada cuando se intenta consultar o reservar un evento inexistente.
    /// </summary>
    public class EventoNoEncontradoException : PlanazoException
    {
        public int EventoId { get; }

        public EventoNoEncontradoException(int eventoId)
            : base($"No se encontró ningún evento registrado con el identificador #{eventoId}.")
        {
            EventoId = eventoId;
        }
    }
}
