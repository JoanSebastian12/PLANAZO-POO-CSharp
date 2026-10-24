namespace Planazo.Excepciones
{
    /// <summary>
    /// Excepción lanzada cuando se intenta realizar una reserva con una cantidad de entradas menor o igual a cero.
    /// </summary>
    public class CantidadInvalidaException : PlanazoException
    {
        public int Cantidad { get; }

        public CantidadInvalidaException(int cantidad)
            : base($"La cantidad de entradas debe ser un número entero mayor a cero. Valor ingresado: {cantidad}.")
        {
            Cantidad = cantidad;
        }
    }
}
