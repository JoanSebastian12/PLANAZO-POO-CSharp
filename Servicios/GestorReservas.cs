using Planazo.Excepciones;
using Planazo.Modelos;

namespace Planazo.Servicios
{
    /// <summary>
    /// Servicio central del dominio que gestiona eventos, reservas y usuarios.
    /// Aplica principios de encapsulamiento, composición de objetos y manejo de colecciones con List&lt;T&gt;.
    /// </summary>
    public class GestorReservas
    {
        private readonly List<Evento> _eventos;
        private readonly List<Reserva> _reservas;
        private readonly List<Usuario> _usuarios;
        private int _consecutivoReserva;
        private int _consecutivoUsuario;

        public GestorReservas()
        {
            _eventos = new List<Evento>();
            _reservas = new List<Reserva>();
            _usuarios = new List<Usuario>();
            _consecutivoReserva = 1000;
            _consecutivoUsuario = 1;

            CargarDatosSemilla();
        }

        /// <summary>
        /// Inicializa el sistema con datos de prueba realistas (mínimo 6 eventos según el enunciado).
        /// Incluye 2 Cines, 2 Conciertos, 2 Talleres y 2 Festivales (Reto 1), más usuarios iniciales (Reto 2).
        /// </summary>
        private void CargarDatosSemilla()
        {
            // Usuarios semilla
            var u1 = new Usuario(_consecutivoUsuario++, "Joan Sebastián Gómez", "1017254890", "joan.gomez@planazo.com", "3001234567");
            var u2 = new Usuario(_consecutivoUsuario++, "María Camila Rivas", "1020304050", "camila.rivas@gmail.com", "3119876543");
            var u3 = new Usuario(_consecutivoUsuario++, "David Fernando Ruiz", "1033445566", "david.ruiz@hotmail.com", "3204567890");
            _usuarios.AddRange(new[] { u1, u2, u3 });

            // 1 & 2: CINE (Mantiene precio base)
            _eventos.Add(new Cine(1, "Interestelar: 10 Aniversario", DateTime.Today.AddDays(2).AddHours(19), 18000m, 50, "Sala IMAX 1", "IMAX Laser 3D", 169));
            _eventos.Add(new Cine(2, "El Padrino: Remasterizada 4K", DateTime.Today.AddDays(4).AddHours(20).AddMinutes(30), 15000m, 40, "Sala Premier 2", "2D Digital", 175));

            // 3 & 4: CONCIERTO (+15 % sobre el precio base)
            _eventos.Add(new Concierto(3, "Coldplay - Music of the Spheres", DateTime.Today.AddDays(10).AddHours(20), 120000m, 80, "Coldplay", "Platino VIP", true));
            _eventos.Add(new Concierto(4, "Rock & Guitarras: Tributo de Leyenda", DateTime.Today.AddDays(15).AddHours(18), 50000m, 100, "Varios Artistas", "General", false));

            // 5 & 6: TALLER (-10 % sobre el precio base)
            _eventos.Add(new Taller(5, "Taller Práctico de C# y POO Avanzada", DateTime.Today.AddDays(5).AddHours(9), 60000m, 25, "Ing. Joan Sebastián", true, "Avanzado"));
            _eventos.Add(new Taller(6, "Fotografía Digital y Composición Urbana", DateTime.Today.AddDays(7).AddHours(14), 45000m, 20, "Laura Marcela Gómez", true, "Intermedio"));

            // 7 & 8: FESTIVAL (Reto 1: +20 % sobre el precio base)
            _eventos.Add(new Festival(7, "Festival Estéreo Sonoro PLANAZO", DateTime.Today.AddDays(25).AddHours(13), 200000m, 150, 4, 3, "PLANAZO & LiveMusic"));
            _eventos.Add(new Festival(8, "Festival Gastronómico y Cervecero", DateTime.Today.AddDays(12).AddHours(12), 40000m, 120, 2, 2, "Cervezas Artesanales"));
        }

        #region Métodos de Consulta y Búsqueda

        /// <summary>
        /// Retorna una lista inmutable de todos los eventos registrados.
        /// </summary>
        public IReadOnlyList<Evento> ObtenerEventos() => _eventos.AsReadOnly();

        /// <summary>
        /// Retorna los eventos que aún tienen cupos disponibles.
        /// </summary>
        public IEnumerable<Evento> ObtenerEventosConCupo() => _eventos.Where(e => e.CuposDisponibles > 0);

        /// <summary>
        /// Busca un evento por su identificador. Retorna null si no existe.
        /// </summary>
        public Evento? BuscarEventoPorId(int id)
        {
            return _eventos.FirstOrDefault(e => e.Id == id);
        }

        /// <summary>
        /// Busca un evento por su ID o lanza una excepción especializada si no existe.
        /// </summary>
        public Evento ObtenerEventoPorIdOExcepcion(int id)
        {
            var evento = BuscarEventoPorId(id);
            if (evento == null)
                throw new EventoNoEncontradoException(id);
            return evento;
        }

        /// <summary>
        /// Retorna una lista inmutable de todas las reservas registradas.
        /// </summary>
        public IReadOnlyList<Reserva> ObtenerReservas() => _reservas.AsReadOnly();

        /// <summary>
        /// Busca una reserva por su ID. Retorna null si no existe.
        /// </summary>
        public Reserva? BuscarReservaPorId(int id)
        {
            return _reservas.FirstOrDefault(r => r.Id == id);
        }

        /// <summary>
        /// Retorna una reserva por su ID o lanza una excepción especializada si no existe.
        /// </summary>
        public Reserva ObtenerReservaPorIdOExcepcion(int id)
        {
            var reserva = BuscarReservaPorId(id);
            if (reserva == null)
                throw new ReservaNoEncontradaException(id);
            return reserva;
        }

        /// <summary>
        /// Retorna una lista de usuarios registrados.
        /// </summary>
        public IReadOnlyList<Usuario> ObtenerUsuarios() => _usuarios.AsReadOnly();

        /// <summary>
        /// Reto adicional 2: Permite consultar todas las reservas asociadas a un usuario determinado.
        /// Se puede buscar por documento o nombre.
        /// </summary>
        public List<Reserva> ConsultarReservasPorUsuario(string criterio)
        {
            if (string.IsNullOrWhiteSpace(criterio))
                return new List<Reserva>();

            string filtro = criterio.Trim().ToLower();

            return _reservas
                .Where(r => r.Usuario.Documento.ToLower() == filtro ||
                            r.Usuario.Nombre.ToLower().Contains(filtro))
                .ToList();
        }

        #endregion

        #region Operaciones de Negocio

        /// <summary>
        /// Registra una nueva reserva validando eventos, cupos y cantidades.
        /// </summary>
        public Reserva RealizarReserva(int eventoId, Usuario usuario, int cantidadEntradas)
        {
            var evento = ObtenerEventoPorIdOExcepcion(eventoId);

            int nuevoId = ++_consecutivoReserva;
            var reserva = new Reserva(nuevoId, usuario, evento, cantidadEntradas);
            _reservas.Add(reserva);

            return reserva;
        }

        /// <summary>
        /// Cancela una reserva existente devolviendo los cupos al evento e impidiendo doble cancelación.
        /// </summary>
        public Reserva CancelarReserva(int reservaId)
        {
            var reserva = ObtenerReservaPorIdOExcepcion(reservaId);
            reserva.Cancelar();
            return reserva;
        }

        /// <summary>
        /// Genera el resumen consolidado de las reservas del sistema.
        /// </summary>
        public ResumenReservas ObtenerResumen()
        {
            var activas = _reservas.Where(r => r.Estado == EstadoReserva.Activa).ToList();
            var canceladas = _reservas.Where(r => r.Estado == EstadoReserva.Cancelada).ToList();

            return new ResumenReservas
            {
                TotalReservasActivas = activas.Count,
                TotalReservasCanceladas = canceladas.Count,
                TotalEntradasReservadasActivas = activas.Sum(r => r.CantidadEntradas),
                ValorTotalReservasActivas = activas.Sum(r => r.ValorTotal)
            };
        }

        /// <summary>
        /// Reto adicional 2: Registra o recupera un usuario existente por su documento.
        /// </summary>
        public Usuario RegistrarOBuscarUsuario(string documento, string nombre, string email, string telefono)
        {
            if (string.IsNullOrWhiteSpace(documento))
                throw new ArgumentException("El documento no puede estar vacío.", nameof(documento));

            var existente = _usuarios.FirstOrDefault(u => u.Documento.Equals(documento.Trim(), StringComparison.OrdinalIgnoreCase));
            if (existente != null)
                return existente;

            int nuevoId = ++_consecutivoUsuario;
            var nuevoUsuario = new Usuario(nuevoId, nombre, documento, email, telefono);
            _usuarios.Add(nuevoUsuario);
            return nuevoUsuario;
        }

        /// <summary>
        /// Permite registrar nuevos eventos en tiempo de ejecución.
        /// </summary>
        public void RegistrarEvento(Evento evento)
        {
            if (evento == null)
                throw new ArgumentNullException(nameof(evento));

            if (_eventos.Any(e => e.Id == evento.Id))
                throw new PlanazoException($"Ya existe un evento con el ID #{evento.Id}.");

            _eventos.Add(evento);
        }

        #endregion
    }
}
