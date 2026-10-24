namespace Planazo.Excepciones
{
    /// <summary>
    /// Excepción lanzada cuando la cantidad de entradas solicitadas supera los cupos disponibles de un evento.
    /// </summary>
    public class CuposInsuficientesException : PlanazoException
    {
        public int CuposDisponibles { get; }
        public int EntradasSolicitadas { get; }

        public CuposInsuficientesException(string nombreEvento, int cuposDisponibles, int entradasSolicitadas)
            : base($"No hay cupos suficientes para el evento '{nombreEvento}'. Cupos disponibles: {cuposDisponibles}, solicitados: {entradasSolicitadas}.")
        {
            CuposDisponibles = cuposDisponibles;
            EntradasSolicitadas = entradasSolicitadas;
        }
    }
}
