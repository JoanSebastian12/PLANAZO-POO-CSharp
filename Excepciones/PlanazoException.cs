namespace Planazo.Excepciones
{
    /// <summary>
    /// Excepción base para los errores de dominio de la aplicación PLANAZO.
    /// Demuestra el manejo de excepciones personalizadas en Programación Orientada a Objetos.
    /// </summary>
    public class PlanazoException : Exception
    {
        public PlanazoException(string mensaje) : base(mensaje)
        {
        }

        public PlanazoException(string mensaje, Exception innerException) : base(mensaje, innerException)
        {
        }
    }
}
