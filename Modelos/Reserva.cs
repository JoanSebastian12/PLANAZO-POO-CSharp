using Planazo.Excepciones;

namespace Planazo.Modelos
{
    /// <summary>
    /// Representa una reserva de entradas para un evento asociado a un usuario.
    /// Demuestra composición, encapsulamiento y control de transiciones de estado.
    /// </summary>
    public class Reserva
    {
        public int Id { get; private set; }
        public Usuario Usuario { get; private set; }
        public string NombrePersona => Usuario.Nombre;
        public Evento EventoReservado { get; private set; }
        public int CantidadEntradas { get; private set; }
        public decimal ValorUnitario { get; private set; }
        public decimal ValorTotal { get; private set; }
        public EstadoReserva Estado { get; private set; }
        public DateTime FechaCreacion { get; private set; }

        public Reserva(int id, Usuario usuario, Evento evento, int cantidadEntradas)
        {
            if (id <= 0)
                throw new ArgumentException("El identificador de la reserva debe ser mayor a cero.", nameof(id));

            Usuario = usuario ?? throw new ArgumentNullException(nameof(usuario), "El usuario es obligatorio para la reserva.");
            EventoReservado = evento ?? throw new ArgumentNullException(nameof(evento), "El evento es obligatorio para la reserva.");

            // Regla 1: No permitir reservas de cantidades negativas o iguales a cero
            if (cantidadEntradas <= 0)
                throw new CantidadInvalidaException(cantidadEntradas);

            // Regla 2: No permitir reservar más entradas que los cupos disponibles
            if (!evento.TieneCuposDisponibles(cantidadEntradas))
                throw new CuposInsuficientesException(evento.Nombre, evento.CuposDisponibles, cantidadEntradas);

            Id = id;
            CantidadEntradas = cantidadEntradas;
            Estado = EstadoReserva.Activa;
            FechaCreacion = DateTime.Now;

            // Regla de cálculo polimórfico: se utiliza el método propio de cada tipo de evento
            ValorUnitario = evento.CalcularPrecioFinal();
            ValorTotal = ValorUnitario * cantidadEntradas;

            // Regla 3: Disminuir los cupos al confirmar la reserva
            evento.ReducirCupos(cantidadEntradas);
        }

        /// <summary>
        /// Cancela la reserva, restaura los cupos al evento e impide dobles cancelaciones.
        /// </summary>
        public void Cancelar()
        {
            // Regla 5: Impedir que una misma reserva sea cancelada dos veces
            if (Estado == EstadoReserva.Cancelada)
                throw new ReservaYaCanceladaException(Id);

            // Cambiar estado a cancelada
            Estado = EstadoReserva.Cancelada;

            // Regla 4: Devolver los cupos cuando se cancela una reserva
            EventoReservado.RestaurarCupos(CantidadEntradas);
        }

        public string MostrarDetalle()
        {
            return $"====================================================\n" +
                   $"RESERVA #{Id} - [{Estado.ToString().ToUpper()}]\n" +
                   $"====================================================\n" +
                   $"  Cliente:          {Usuario.Nombre} (Doc: {Usuario.Documento})\n" +
                   $"  Email:            {Usuario.Email} | Tel: {Usuario.Telefono}\n" +
                   $"  Evento:           [{EventoReservado.TipoEvento}] {EventoReservado.Nombre}\n" +
                   $"  Fecha Evento:     {EventoReservado.Fecha:dd/MM/yyyy HH:mm}\n" +
                   $"  Entradas:         {CantidadEntradas}\n" +
                   $"  Precio Unitario:  {ValorUnitario:C}\n" +
                   $"  Total Reserva:    {ValorTotal:C}\n" +
                   $"  Fecha Emisión:    {FechaCreacion:dd/MM/yyyy HH:mm:ss}\n" +
                   $"====================================================";
        }

        public override string ToString()
        {
            return $"Reserva #{Id} | {Usuario.Nombre} | [{EventoReservado.TipoEvento}] {EventoReservado.Nombre} | " +
                   $"Cant: {CantidadEntradas} | Total: {ValorTotal:C} | Estado: {Estado}";
        }
    }
}
