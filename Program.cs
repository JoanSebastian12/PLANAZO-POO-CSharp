using Planazo.Excepciones;
using Planazo.Modelos;
using Planazo.Servicios;

namespace Planazo
{
    /// <summary>
    /// Punto de entrada principal de la aplicación PLANAZO.
    /// Contiene la interfaz de consola interactiva y captura global de excepciones
    /// para garantizar que el programa nunca termine inesperadamente ante errores del usuario.
    /// </summary>
    public class Program
    {
        private static readonly GestorReservas _gestor = new GestorReservas();

        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            if (args.Length > 0 && args[0].Equals("--test", StringComparison.OrdinalIgnoreCase))
            {
                SuitePruebas.EjecutarTodasLasPruebas();
                return;
            }

            bool continuar = true;

            while (continuar)
            {
                try
                {
                    MostrarMenuPrincipal();
                    Console.Write("Seleccione una opción: ");
                    string? opcion = Console.ReadLine()?.Trim();
                    Console.WriteLine();

                    switch (opcion)
                    {
                        case "1":
                            VerEventosDisponibles();
                            break;
                        case "2":
                            ConsultarEvento();
                            break;
                        case "3":
                            ReservarEntradas();
                            break;
                        case "4":
                            VerReservasRealizadas();
                            break;
                        case "5":
                            CancelarReserva();
                            break;
                        case "6":
                            VerResumenReservas();
                            break;
                        case "7":
                            ConsultarReservasPorUsuario(); // Reto 2
                            break;
                        case "8":
                            SuitePruebas.EjecutarTodasLasPruebas(); // Casos de prueba
                            break;
                        case "0":
                            continuar = false;
                            MostrarMensajeDespedida();
                            break;
                        default:
                            MostrarError("Opción no válida. Por favor seleccione un número del menú.");
                            break;
                    }
                }
                catch (PlanazoException ex)
                {
                    MostrarError($"[ERROR DE NEGOCIO] {ex.Message}");
                }
                catch (Exception ex)
                {
                    MostrarError($"[ERROR INESPERADO] {ex.Message}");
                }

                if (continuar)
                {
                    Pausa();
                }
            }
        }

        private static void LimpiarPantalla()
        {
            if (!Console.IsOutputRedirected)
            {
                try { Console.Clear(); } catch { }
            }
        }

        private static void Pausa()
        {
            Console.WriteLine("\nPresione ENTER para continuar...");
            if (Console.IsInputRedirected)
            {
                Console.ReadLine();
            }
            else
            {
                try { Console.ReadKey(true); } catch { Console.ReadLine(); }
            }
        }

        private static void MostrarMenuPrincipal()
        {
            LimpiarPantalla();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("================================");
            Console.WriteLine("        PLANAZO - EVENTOS       ");
            Console.WriteLine("================================");
            Console.ResetColor();
            Console.WriteLine("1. Ver eventos disponibles");
            Console.WriteLine("2. Consultar evento");
            Console.WriteLine("3. Reservar entradas");
            Console.WriteLine("4. Ver reservas realizadas");
            Console.WriteLine("5. Cancelar una reserva");
            Console.WriteLine("6. Ver resumen de reservas");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("7. Consultar reservas por usuario (Reto 2)");
            Console.WriteLine("8. Ejecutar casos de prueba automáticos");
            Console.ResetColor();
            Console.WriteLine("0. Salir");
            Console.WriteLine();
        }

        private static void VerEventosDisponibles()
        {
            MostrarTituloSeccion("CATÁLOGO DE EVENTOS DISPONIBLES");

            var eventos = _gestor.ObtenerEventos();
            if (!eventos.Any())
            {
                Console.WriteLine("No hay eventos registrados en este momento.");
                return;
            }

            Console.WriteLine(string.Format("{0,-4} | {1,-10} | {2,-36} | {3,-16} | {4,14} | {5,14} | {6,8}",
                "ID", "TIPO", "NOMBRE", "FECHA", "PRECIO BASE", "PRECIO FINAL", "CUPOS"));
            Console.WriteLine(new string('-', 112));

            foreach (var ev in eventos)
            {
                FormatearColorPorTipo(ev.TipoEvento);
                Console.WriteLine(string.Format("{0,-4} | {1,-10} | {2,-36} | {3,-16:dd/MM/yy HH:mm} | {4,14:C} | {5,14:C} | {6,5}/{7,-2}",
                    ev.Id,
                    ev.TipoEvento,
                    TruncarTexto(ev.Nombre, 36),
                    ev.Fecha,
                    ev.PrecioBase,
                    ev.CalcularPrecioFinal(),
                    ev.CuposDisponibles,
                    ev.CapacidadMaxima));
                Console.ResetColor();
            }

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("* Nota: Los precios finales reflejan las reglas polimórficas (Cine: Base | Concierto: +15% | Taller: -10% | Festival: +20%).");
            Console.ResetColor();
        }

        private static void ConsultarEvento()
        {
            MostrarTituloSeccion("CONSULTA DETALLADA DE EVENTO");

            Console.Write("Ingrese el identificador (ID) del evento a consultar: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                MostrarError("El identificador debe ser un número entero válido.");
                return;
            }

            try
            {
                var evento = _gestor.ObtenerEventoPorIdOExcepcion(id);

                Console.WriteLine();
                FormatearColorPorTipo(evento.TipoEvento);
                Console.WriteLine(evento.MostrarDetalle());
                Console.ResetColor();
            }
            catch (EventoNoEncontradoException ex)
            {
                MostrarError(ex.Message);
            }
        }

        private static void ReservarEntradas()
        {
            MostrarTituloSeccion("RESERVA DE ENTRADAS");

            // Paso 1: Selección del evento
            Console.Write("Ingrese el ID del evento que desea reservar: ");
            if (!int.TryParse(Console.ReadLine(), out int eventoId))
            {
                MostrarError("El ID del evento debe ser numérico.");
                return;
            }

            var evento = _gestor.BuscarEventoPorId(eventoId);
            if (evento == null)
            {
                MostrarError($"El evento con ID #{eventoId} no existe en el catálogo.");
                return;
            }

            Console.WriteLine($"\n>> Evento seleccionado: [{evento.TipoEvento}] \"{evento.Nombre}\"");
            Console.WriteLine($"   Precio por entrada: {evento.CalcularPrecioFinal():C} | Cupos disponibles: {evento.CuposDisponibles}");

            if (evento.CuposDisponibles == 0)
            {
                MostrarError("Lo sentimos, este evento no cuenta con cupos disponibles.");
                return;
            }

            // Paso 2: Identificación del usuario (Reto 2)
            Console.WriteLine("\n--- Identificación del Comprador ---");
            Console.Write("Ingrese su documento de identidad / cédula: ");
            string? documento = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(documento))
            {
                MostrarError("El documento de identidad es obligatorio.");
                return;
            }

            var usuario = _gestor.ObtenerUsuarios().FirstOrDefault(u => u.Documento.Equals(documento, StringComparison.OrdinalIgnoreCase));
            if (usuario != null)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"¡Bienvenido de nuevo, {usuario.Nombre}!");
                Console.ResetColor();
            }
            else
            {
                Console.Write("Ingrese su nombre completo: ");
                string? nombre = Console.ReadLine()?.Trim();
                if (string.IsNullOrWhiteSpace(nombre))
                {
                    MostrarError("El nombre no puede estar vacío.");
                    return;
                }

                Console.Write("Ingrese su correo electrónico: ");
                string? email = Console.ReadLine()?.Trim();

                Console.Write("Ingrese su número de teléfono: ");
                string? telefono = Console.ReadLine()?.Trim();

                usuario = _gestor.RegistrarOBuscarUsuario(documento, nombre, email ?? "", telefono ?? "");
            }

            // Paso 3: Cantidad de entradas
            Console.Write("\nIngrese la cantidad de entradas a reservar: ");
            if (!int.TryParse(Console.ReadLine(), out int cantidad))
            {
                MostrarError("La cantidad ingresada debe ser un número entero.");
                return;
            }

            try
            {
                var reserva = _gestor.RealizarReserva(eventoId, usuario, cantidad);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n ¡RESERVA CONFIRMADA CON ÉXITO!");
                Console.ResetColor();
                Console.WriteLine(reserva.MostrarDetalle());
            }
            catch (CantidadInvalidaException ex)
            {
                MostrarError($"[CANTIDAD INVÁLIDA] {ex.Message}");
            }
            catch (CuposInsuficientesException ex)
            {
                MostrarError($"[CUPOS INSUFICIENTES] {ex.Message}");
            }
            catch (Exception ex)
            {
                MostrarError($"No fue posible completar la reserva: {ex.Message}");
            }
        }

        private static void VerReservasRealizadas()
        {
            MostrarTituloSeccion("HISTORIAL DE RESERVAS REALIZADAS");

            var reservas = _gestor.ObtenerReservas();
            if (!reservas.Any())
            {
                Console.WriteLine("Aún no se ha realizado ninguna reserva en el sistema.");
                return;
            }

            Console.WriteLine(string.Format("{0,-8} | {1,-20} | {2,-28} | {3,5} | {4,14} | {5,-10}",
                "ID", "CLIENTE", "EVENTO", "CANT", "TOTAL", "ESTADO"));
            Console.WriteLine(new string('-', 98));

            foreach (var r in reservas)
            {
                if (r.Estado == EstadoReserva.Activa)
                    Console.ForegroundColor = ConsoleColor.Green;
                else
                    Console.ForegroundColor = ConsoleColor.DarkGray;

                Console.WriteLine(string.Format("#{0,-7} | {1,-20} | {2,-28} | {3,5} | {4,14:C} | {5,-10}",
                    r.Id,
                    TruncarTexto(r.Usuario.Nombre, 20),
                    TruncarTexto(r.EventoReservado.Nombre, 28),
                    r.CantidadEntradas,
                    r.ValorTotal,
                    r.Estado));
                Console.ResetColor();
            }
        }

        private static void CancelarReserva()
        {
            MostrarTituloSeccion("CANCELACIÓN DE RESERVA");

            Console.Write("Ingrese el identificador (ID) de la reserva a cancelar: ");
            if (!int.TryParse(Console.ReadLine(), out int reservaId))
            {
                MostrarError("El ID de la reserva debe ser numérico.");
                return;
            }

            try
            {
                var reserva = _gestor.ObtenerReservaPorIdOExcepcion(reservaId);
                int cuposAntes = reserva.EventoReservado.CuposDisponibles;

                _gestor.CancelarReserva(reservaId);

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"\n ¡RESERVA #{reservaId} CANCELADA EXITOSAMENTE!");
                Console.ResetColor();
                Console.WriteLine($"  Evento:               {reserva.EventoReservado.Nombre}");
                Console.WriteLine($"  Entradas devueltas:   {reserva.CantidadEntradas}");
                Console.WriteLine($"  Cupos restaurados:    {cuposAntes} -> {reserva.EventoReservado.CuposDisponibles}");
                Console.WriteLine($"  Estado actual:        {reserva.Estado}");
            }
            catch (ReservaYaCanceladaException ex)
            {
                MostrarError($"[OPERACIÓN INVÁLIDA] {ex.Message}");
            }
            catch (ReservaNoEncontradaException ex)
            {
                MostrarError(ex.Message);
            }
        }

        private static void VerResumenReservas()
        {
            MostrarTituloSeccion("RESUMEN GENERAL DE RESERVAS");

            var resumen = _gestor.ObtenerResumen();

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("=================================================");
            Console.WriteLine("            CONSOLIDADO PLANAZO                  ");
            Console.WriteLine("=================================================");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"  • Cantidad de reservas activas:        {resumen.TotalReservasActivas}");
            Console.WriteLine($"  • Total de entradas reservadas:        {resumen.TotalEntradasReservadasActivas}");
            Console.WriteLine($"  • Valor total de reservas activas:     {resumen.ValorTotalReservasActivas:C}");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"  • Cantidad de reservas canceladas:     {resumen.TotalReservasCanceladas}");
            Console.ResetColor();

            Console.WriteLine("=================================================");
        }

        private static void ConsultarReservasPorUsuario()
        {
            MostrarTituloSeccion("RETO 2: CONSULTA DE RESERVAS POR USUARIO");

            Console.Write("Ingrese el documento de identidad o nombre del usuario: ");
            string? criterio = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(criterio))
            {
                MostrarError("Debe ingresar un documento o nombre válido.");
                return;
            }

            var reservasUsuario = _gestor.ConsultarReservasPorUsuario(criterio);

            if (!reservasUsuario.Any())
            {
                Console.WriteLine($"No se encontraron reservas para el usuario con criterio: \"{criterio}\".");
                return;
            }

            Console.WriteLine($"\nSe encontraron {reservasUsuario.Count} reserva(s) para \"{criterio}\":\n");

            foreach (var r in reservasUsuario)
            {
                Console.WriteLine(r.MostrarDetalle());
                Console.WriteLine();
            }
        }

        private static void MostrarMensajeDespedida()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n¡Gracias por utilizar el sistema de gestión PLANAZO!");
            Console.WriteLine("Desarrollado con Programación Orientada a Objetos en C# y .NET 8.\n");
            Console.ResetColor();
        }

        #region Helpers de Interfaz

        private static void MostrarTituloSeccion(string titulo)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\n--- {titulo} ---");
            Console.ResetColor();
        }

        private static void MostrarError(string mensaje)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n [ALERTA] {mensaje}");
            Console.ResetColor();
        }

        private static void FormatearColorPorTipo(string tipo)
        {
            switch (tipo)
            {
                case "Cine":
                    Console.ForegroundColor = ConsoleColor.Blue;
                    break;
                case "Concierto":
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    break;
                case "Taller":
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;
                case "Festival":
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                default:
                    Console.ResetColor();
                    break;
            }
        }

        private static string TruncarTexto(string texto, int longitud)
        {
            if (string.IsNullOrEmpty(texto)) return "";
            return texto.Length <= longitud ? texto : texto.Substring(0, longitud - 3) + "...";
        }

        #endregion
    }
}
