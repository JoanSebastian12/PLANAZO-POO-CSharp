using Planazo.Excepciones;

namespace Planazo.Modelos
{
    /// <summary>
    /// Clase abstracta base que representa cualquier evento dentro de la plataforma PLANAZO.
    /// Demuestra el principio de abstracción, encapsulamiento riguroso y provee la base para el polimorfismo.
    /// </summary>
    public abstract class Evento
    {
        // Encapsulamiento: propiedades con acceso de solo lectura externa o setters privados/protegidos
        public int Id { get; private set; }
        public string Nombre { get; private set; }
        public DateTime Fecha { get; private set; }
        public decimal PrecioBase { get; private set; }
        public int CapacidadMaxima { get; private set; }
        public int CuposDisponibles { get; private set; }

        /// <summary>
        /// Propiedad polimórfica que describe el tipo de evento concreto.
        /// </summary>
        public abstract string TipoEvento { get; }

        /// <summary>
        /// Constructor base que inicializa y valida el estado consistente del evento.
        /// </summary>
        protected Evento(int id, string nombre, DateTime fecha, decimal precioBase, int capacidadMaxima)
        {
            if (id <= 0)
                throw new ArgumentException("El identificador del evento debe ser mayor a cero.", nameof(id));

            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre del evento no puede ser nulo o estar vacío.", nameof(nombre));

            if (precioBase < 0)
                throw new ArgumentException("El precio base no puede ser un valor negativo.", nameof(precioBase));

            if (capacidadMaxima <= 0)
                throw new ArgumentException("La capacidad máxima debe ser mayor a cero.", nameof(capacidadMaxima));

            Id = id;
            Nombre = nombre.Trim();
            Fecha = fecha;
            PrecioBase = precioBase;
            CapacidadMaxima = capacidadMaxima;
            CuposDisponibles = capacidadMaxima; // Inicialmente todos los cupos están disponibles
        }

        /// <summary>
        /// Método abstracto para calcular el precio final de la entrada.
        /// Cada clase hija implementará su propia regla de negocio aplicando polimorfismo.
        /// </summary>
        public abstract decimal CalcularPrecioFinal();

        /// <summary>
        /// Verifica si hay suficientes cupos disponibles para una cantidad solicitada.
        /// </summary>
        public bool TieneCuposDisponibles(int cantidad)
        {
            return cantidad > 0 && CuposDisponibles >= cantidad;
        }

        /// <summary>
        /// Reduce la cantidad de cupos disponibles al realizar una reserva válida.
        /// Protege la invariante de estado del evento mediante encapsulamiento.
        /// </summary>
        public void ReducirCupos(int cantidad)
        {
            if (cantidad <= 0)
                throw new CantidadInvalidaException(cantidad);

            if (cantidad > CuposDisponibles)
                throw new CuposInsuficientesException(Nombre, CuposDisponibles, cantidad);

            CuposDisponibles -= cantidad;
        }

        /// <summary>
        /// Restaura la cantidad de cupos disponibles al cancelar una reserva.
        /// </summary>
        public void RestaurarCupos(int cantidad)
        {
            if (cantidad <= 0)
                throw new CantidadInvalidaException(cantidad);

            if (CuposDisponibles + cantidad > CapacidadMaxima)
                throw new PlanazoException($"Inconsistencia al restaurar cupos: los cupos superarían la capacidad máxima ({CapacidadMaxima}).");

            CuposDisponibles += cantidad;
        }

        /// <summary>
        /// Muestra información detallada del evento incluyendo precio final y cupos.
        /// </summary>
        public virtual string MostrarDetalle()
        {
            return $"ID: #{Id} | [{TipoEvento.ToUpper()}] \"{Nombre}\"\n" +
                   $"  Fecha: {Fecha:dd/MM/yyyy HH:mm} | Precio Base: {PrecioBase:C} | Precio Final: {CalcularPrecioFinal():C}\n" +
                   $"  Capacidad: {CapacidadMaxima} | Cupos Disponibles: {CuposDisponibles}";
        }

        public override string ToString()
        {
            return $"#{Id} - [{TipoEvento}] {Nombre} ({Fecha:dd/MM/yyyy}) - {CalcularPrecioFinal():C} [Cupos: {CuposDisponibles}/{CapacidadMaxima}]";
        }
    }
}
