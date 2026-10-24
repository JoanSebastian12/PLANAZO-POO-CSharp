using Planazo.Excepciones;
using Planazo.Modelos;

namespace Planazo.Servicios
{
    /// <summary>
    /// Suite de verificación automatizada que ejecuta y valida los 7 casos de prueba
    /// exigidos por la rúbrica del parcial, además de los retos adicionales.
    /// </summary>
    public static class SuitePruebas
    {
        public static void EjecutarTodasLasPruebas()
        {
            if (!Console.IsOutputRedirected)
            {
                try { Console.Clear(); } catch { }
            }
            ImprimirEncabezado("SUITE DE PRUEBAS DE CALIDAD - PLANAZO POO C#");
            Console.WriteLine("Ejecutando los 7 escenarios de prueba solicitados en el parcial...\n");

            int exitosas = 0;
            int total = 7;

            if (ProbarCaso1_ReservaValida()) exitosas++;
            if (ProbarCaso2_ReservaMasCupos()) exitosas++;
            if (ProbarCaso3_ReservaCantidadNegativa()) exitosas++;
            if (ProbarCaso4_CancelarYDevolverCupos()) exitosas++;
            if (ProbarCaso5_CancelarDosVeces()) exitosas++;
            if (ProbarCaso6_ConsultarEventoInexistente()) exitosas++;
            if (ProbarCaso7_CalculoPrecioPolimorfismo()) exitosas++;

            Console.WriteLine(new string('=', 65));
            if (exitosas == total)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n RESULTADO FINAL: {exitosas}/{total} PRUEBAS SUPERADAS CON ÉXITO (100%)");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n RESULTADO FINAL: {exitosas}/{total} PRUEBAS SUPERADAS.");
                Console.ResetColor();
            }
            Console.WriteLine(new string('=', 65));
        }

        private static bool ProbarCaso1_ReservaValida()
        {
            ImprimirSeparador("Caso 1: Registrar una reserva válida");
            try
            {
                var gestor = new GestorReservas();
                var evento = gestor.ObtenerEventoPorIdOExcepcion(1); // Cine: Interestelar, 50 cupos
                int cuposIniciales = evento.CuposDisponibles;

                var usuario = gestor.RegistrarOBuscarUsuario("1001", "Prueba Valida", "prueba@test.com", "3000000000");
                var reserva = gestor.RealizarReserva(1, usuario, 3);

                bool cuposDisminuyeron = (evento.CuposDisponibles == cuposIniciales - 3);
                bool totalCorrecto = (reserva.ValorTotal == evento.CalcularPrecioFinal() * 3);
                bool estadoActivo = (reserva.Estado == EstadoReserva.Activa);

                if (cuposDisminuyeron && totalCorrecto && estadoActivo)
                {
                    ImprimirExito($"Reserva #{reserva.Id} generada exitosamente. Total: {reserva.ValorTotal:C}. Cupos: {cuposIniciales} -> {evento.CuposDisponibles}");
                    return true;
                }
                ImprimirFallo("Los cupos o valores no coincidieron con lo esperado.");
                return false;
            }
            catch (Exception ex)
            {
                ImprimirFallo($"Excepción inesperada: {ex.Message}");
                return false;
            }
        }

        private static bool ProbarCaso2_ReservaMasCupos()
        {
            ImprimirSeparador("Caso 2: Intentar reservar más entradas que los cupos disponibles");
            try
            {
                var gestor = new GestorReservas();
                var evento = gestor.ObtenerEventoPorIdOExcepcion(5); // Taller: 25 cupos
                var usuario = gestor.RegistrarOBuscarUsuario("1002", "Prueba Cupos", "cupos@test.com", "3000000001");

                // Intentar reservar 30 entradas cuando solo hay 25
                gestor.RealizarReserva(5, usuario, 30);

                ImprimirFallo("El sistema permitió reservar más entradas que los cupos disponibles.");
                return false;
            }
            catch (CuposInsuficientesException ex)
            {
                ImprimirExito($"Excepción capturada correctamente ({ex.GetType().Name}): {ex.Message}");
                return true;
            }
            catch (Exception ex)
            {
                ImprimirFallo($"Se esperaba CuposInsuficientesException pero se lanzó: {ex.GetType().Name} - {ex.Message}");
                return false;
            }
        }

        private static bool ProbarCaso3_ReservaCantidadNegativa()
        {
            ImprimirSeparador("Caso 3: Intentar realizar una reserva con cantidad negativa o cero");
            try
            {
                var gestor = new GestorReservas();
                var usuario = gestor.RegistrarOBuscarUsuario("1003", "Prueba Negativo", "neg@test.com", "3000000002");

                // Intentar reservar -5 entradas
                gestor.RealizarReserva(1, usuario, -5);

                ImprimirFallo("El sistema permitió una reserva con cantidad negativa.");
                return false;
            }
            catch (CantidadInvalidaException ex)
            {
                ImprimirExito($"Excepción capturada correctamente ({ex.GetType().Name}): {ex.Message}");
                return true;
            }
            catch (Exception ex)
            {
                ImprimirFallo($"Se esperaba CantidadInvalidaException pero se lanzó: {ex.GetType().Name} - {ex.Message}");
                return false;
            }
        }

        private static bool ProbarCaso4_CancelarYDevolverCupos()
        {
            ImprimirSeparador("Caso 4: Cancelar una reserva y comprobar que se devuelven los cupos");
            try
            {
                var gestor = new GestorReservas();
                var evento = gestor.ObtenerEventoPorIdOExcepcion(3); // Concierto Coldplay, 80 cupos
                int cuposAntesReserva = evento.CuposDisponibles;

                var usuario = gestor.RegistrarOBuscarUsuario("1004", "Prueba Cancelar", "cancel@test.com", "3000000003");
                var reserva = gestor.RealizarReserva(3, usuario, 4);
                int cuposDespuesReserva = evento.CuposDisponibles;

                gestor.CancelarReserva(reserva.Id);
                int cuposDespuesCancelacion = evento.CuposDisponibles;

                if (cuposDespuesReserva == cuposAntesReserva - 4 && cuposDespuesCancelacion == cuposAntesReserva && reserva.Estado == EstadoReserva.Cancelada)
                {
                    ImprimirExito($"Reserva #{reserva.Id} cancelada. Cupos: {cuposAntesReserva} -> {cuposDespuesReserva} -> {cuposDespuesCancelacion}. ¡Cupos devueltos exitosamente!");
                    return true;
                }
                ImprimirFallo("Los cupos no se restauraron adecuadamente tras la cancelación.");
                return false;
            }
            catch (Exception ex)
            {
                ImprimirFallo($"Excepción inesperada: {ex.Message}");
                return false;
            }
        }

        private static bool ProbarCaso5_CancelarDosVeces()
        {
            ImprimirSeparador("Caso 5: Intentar cancelar dos veces la misma reserva");
            try
            {
                var gestor = new GestorReservas();
                var usuario = gestor.RegistrarOBuscarUsuario("1005", "Prueba Doble", "doble@test.com", "3000000004");
                var reserva = gestor.RealizarReserva(2, usuario, 2);

                // Primera cancelación (debe funcionar)
                gestor.CancelarReserva(reserva.Id);

                // Segunda cancelación (debe lanzar excepción)
                gestor.CancelarReserva(reserva.Id);

                ImprimirFallo("El sistema permitió cancelar dos veces la misma reserva.");
                return false;
            }
            catch (ReservaYaCanceladaException ex)
            {
                ImprimirExito($"Excepción capturada correctamente ({ex.GetType().Name}): {ex.Message}");
                return true;
            }
            catch (Exception ex)
            {
                ImprimirFallo($"Se esperaba ReservaYaCanceladaException pero se lanzó: {ex.GetType().Name} - {ex.Message}");
                return false;
            }
        }

        private static bool ProbarCaso6_ConsultarEventoInexistente()
        {
            ImprimirSeparador("Caso 6: Consultar un evento inexistente");
            try
            {
                var gestor = new GestorReservas();
                int idInexistente = 9999;

                // Debe lanzar EventoNoEncontradoException
                gestor.ObtenerEventoPorIdOExcepcion(idInexistente);

                ImprimirFallo("El sistema no detectó el evento inexistente.");
                return false;
            }
            catch (EventoNoEncontradoException ex)
            {
                ImprimirExito($"Excepción capturada correctamente ({ex.GetType().Name}): {ex.Message}");
                return true;
            }
            catch (Exception ex)
            {
                ImprimirFallo($"Se esperaba EventoNoEncontradoException pero se lanzó: {ex.GetType().Name} - {ex.Message}");
                return false;
            }
        }

        private static bool ProbarCaso7_CalculoPrecioPolimorfismo()
        {
            ImprimirSeparador("Caso 7: Comprobar que cada tipo de evento calcula correctamente su precio final");
            try
            {
                // Cine: mantiene precio base
                var cine = new Cine(101, "Cine Test", DateTime.Today, 20000m, 50);
                decimal precioCine = cine.CalcularPrecioFinal();
                bool cineOk = (precioCine == 20000m);

                // Concierto: aumenta 15% (Ejemplo del enunciado: $50.000 -> $57.500)
                var concierto = new Concierto(102, "Concierto Test", DateTime.Today, 50000m, 50, "Artista");
                decimal precioConcierto = concierto.CalcularPrecioFinal();
                bool conciertoOk = (precioConcierto == 57500m);

                // Taller: descuento 10% ($40.000 -> $36.000)
                var taller = new Taller(103, "Taller Test", DateTime.Today, 40000m, 30, "Instructor");
                decimal precioTaller = taller.CalcularPrecioFinal();
                bool tallerOk = (precioTaller == 36000m);

                // Festival (Reto 1): aumenta 20% ($100.000 -> $120.000)
                var festival = new Festival(104, "Festival Test", DateTime.Today, 100000m, 100);
                decimal precioFestival = festival.CalcularPrecioFinal();
                bool festivalOk = (precioFestival == 120000m);

                if (cineOk && conciertoOk && tallerOk && festivalOk)
                {
                    ImprimirExito($"Cine: Base $20.000 -> Final {precioCine:C} (Exacto)");
                    ImprimirExito($"Concierto: Base $50.000 -> Final {precioConcierto:C} (+15% Exacto)");
                    ImprimirExito($"Taller: Base $40.000 -> Final {precioTaller:C} (-10% Exacto)");
                    ImprimirExito($"Festival (Reto 1): Base $100.000 -> Final {precioFestival:C} (+20% Exacto)");
                    return true;
                }
                ImprimirFallo("Uno o más cálculos polimórficos arrojaron resultados incorrectos.");
                return false;
            }
            catch (Exception ex)
            {
                ImprimirFallo($"Excepción inesperada: {ex.Message}");
                return false;
            }
        }

        #region Helpers de Salida por Consola

        private static void ImprimirEncabezado(string titulo)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(new string('=', 65));
            Console.WriteLine($"  {titulo}");
            Console.WriteLine(new string('=', 65));
            Console.ResetColor();
        }

        private static void ImprimirSeparador(string subtitulo)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"\n>> {subtitulo}");
            Console.ResetColor();
        }

        private static void ImprimirExito(string mensaje)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("  [PASÓ] ");
            Console.ResetColor();
            Console.WriteLine(mensaje);
        }

        private static void ImprimirFallo(string mensaje)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("  [FALLÓ] ");
            Console.ResetColor();
            Console.WriteLine(mensaje);
        }

        #endregion
    }
}
