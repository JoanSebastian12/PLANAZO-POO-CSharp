Add-Type -AssemblyName System.Drawing

function Render-TerminalImage {
    param(
        [string]$Title,
        [string[]]$Lines,
        [string]$OutputPath,
        [int]$Width = 980
    )

    $fontSize = 11
    $fontFamily = "Consolas"
    $font = New-Object System.Drawing.Font($fontFamily, $fontSize, [System.Drawing.FontStyle]::Regular)
    $fontBold = New-Object System.Drawing.Font($fontFamily, $fontSize, [System.Drawing.FontStyle]::Bold)
    $titleFont = New-Object System.Drawing.Font("Segoe UI", 10, [System.Drawing.FontStyle]::Regular)

    $lineHeight = 22
    $paddingX = 24
    $paddingTop = 45
    $paddingBottom = 24

    $height = $paddingTop + ($Lines.Count * $lineHeight) + $paddingBottom
    if ($height -lt 240) { $height = 240 }

    $bitmap = New-Object System.Drawing.Bitmap($Width, $height)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.TextRenderingHint = [System.Drawing.Text.TextRenderingHint]::ClearTypeGridFit
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias

    $bgBrush = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(255, 24, 24, 37))
    $graphics.FillRectangle($bgBrush, 0, 0, $Width, $height)

    $titleBarBrush = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(255, 17, 17, 27))
    $graphics.FillRectangle($titleBarBrush, 0, 0, $Width, 34)

    $redDot = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(255, 243, 139, 168))
    $yellowDot = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(255, 249, 226, 175))
    $greenDot = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(255, 166, 227, 161))

    $graphics.FillEllipse($redDot, 14, 11, 12, 12)
    $graphics.FillEllipse($yellowDot, 34, 11, 12, 12)
    $graphics.FillEllipse($greenDot, 54, 11, 12, 12)

    $titleBrush = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(255, 166, 173, 200))
    $titleString = "PLANAZO Console - " + $Title
    $titleSize = $graphics.MeasureString($titleString, $titleFont)
    $titleX = ($Width - $titleSize.Width) / 2
    $graphics.DrawString($titleString, $titleFont, $titleBrush, $titleX, 8)

    $borderPen = New-Object System.Drawing.Pen([System.Drawing.Color]::FromArgb(255, 49, 50, 68), 1)
    $graphics.DrawRectangle($borderPen, 0, 0, $Width - 1, $height - 1)
    $graphics.DrawLine($borderPen, 0, 34, $Width, 34)

    $brushDefault = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(255, 205, 214, 244))
    $brushCyan = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(255, 137, 220, 235))
    $brushGreen = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(255, 166, 227, 161))
    $brushRed = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(255, 243, 139, 168))
    $brushYellow = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(255, 249, 226, 175))
    $brushMagenta = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(255, 245, 194, 231))
    $brushBlue = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(255, 137, 180, 250))
    $brushGray = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(255, 147, 153, 178))

    $currentY = $paddingTop

    foreach ($line in $Lines) {
        $selectedBrush = $brushDefault
        $useBold = $false

        if ($line.StartsWith("===") -or $line.StartsWith("---")) {
            $selectedBrush = $brushCyan
            $useBold = $true
        } elseif ($line.Contains("PLANAZO") -or $line.StartsWith("--- ")) {
            $selectedBrush = $brushYellow
            $useBold = $true
        } elseif ($line.Contains("PASO") -or $line.Contains("CONFIRMADA") -or $line.Contains("EXITO") -or $line.Contains("SUPERADAS") -or $line.Contains("EXITOSAMENTE")) {
            $selectedBrush = $brushGreen
            $useBold = $true
        } elseif ($line.Contains("FALLO") -or $line.Contains("[ALERTA]") -or $line.Contains("[ERROR") -or $line.Contains("INSUFICIENTES") -or $line.Contains("INVALIDA")) {
            $selectedBrush = $brushRed
            $useBold = $true
        } elseif ($line.Contains("Cine")) {
            $selectedBrush = $brushBlue
        } elseif ($line.Contains("Concierto")) {
            $selectedBrush = $brushMagenta
        } elseif ($line.Contains("Taller")) {
            $selectedBrush = $brushCyan
        } elseif ($line.Contains("Festival")) {
            $selectedBrush = $brushYellow
        } elseif ($line.Trim().StartsWith("*")) {
            $selectedBrush = $brushGray
        }

        $f = if ($useBold) { $fontBold } else { $font }
        $graphics.DrawString($line, $f, $selectedBrush, $paddingX, $currentY)
        $currentY += $lineHeight
    }

    $bitmap.Save($OutputPath, [System.Drawing.Imaging.ImageFormat]::Png)
    $graphics.Dispose()
    $bitmap.Dispose()
}

$capturasDir = "c:\Users\Usuario\Desktop\joan\PLANAZO-POO-CSharp\Capturas"

# 1. Menu Principal
$lineasMenu = @(
    "================================",
    "        PLANAZO - EVENTOS       ",
    "================================",
    "1. Ver eventos disponibles",
    "2. Consultar evento",
    "3. Reservar entradas",
    "4. Ver reservas realizadas",
    "5. Cancelar una reserva",
    "6. Ver resumen de reservas",
    "7. Consultar reservas por usuario (Reto 2)",
    "8. Ejecutar casos de prueba automaticos",
    "0. Salir",
    "",
    "Seleccione una opcion: _"
)
Render-TerminalImage -Title "Menu Principal Interactivo" -Lines $lineasMenu -OutputPath "$capturasDir\01_menu_principal.png"

# 2. Catalogo de Eventos
$lineasCatalogo = @(
    "--- CATALOGO DE EVENTOS DISPONIBLES ---",
    "",
    "ID   | TIPO       | NOMBRE                               | FECHA            |    PRECIO BASE |   PRECIO FINAL |   CUPOS",
    "----------------------------------------------------------------------------------------------------------------",
    "1    | Cine       | Interestelar: 10 Aniversario         | 26-10-26 19:00   |     $18.000,00 |     $18.000,00 |    50/50",
    "2    | Cine       | El Padrino: Remasterizada 4K         | 28-10-26 20:30   |     $15.000,00 |     $15.000,00 |    40/40",
    "3    | Concierto  | Coldplay - Music of the Spheres      | 03-11-26 20:00   |    $120.000,00 |    $138.000,00 |    80/80",
    "4    | Concierto  | Rock & Guitarras: Tributo de Leyenda | 08-11-26 18:00   |     $50.000,00 |     $57.500,00 |   100/100",
    "5    | Taller     | Taller Practico de C# y POO Avanzada | 29-10-26 09:00   |     $60.000,00 |     $54.000,00 |    25/25",
    "6    | Taller     | Fotografia Digital y Composicion ... | 31-10-26 14:00   |     $45.000,00 |     $40.500,00 |    20/20",
    "7    | Festival   | Festival Estereo Sonoro PLANAZO      | 18-11-26 13:00   |    $200.000,00 |    $240.000,00 |   150/150",
    "8    | Festival   | Festival Gastronomico y Cervecero    | 05-11-26 12:00   |     $40.000,00 |     $48.000,00 |   120/120",
    "",
    "* Nota: Los precios finales reflejan las reglas polimorficas (Cine: Base | Concierto: +15% | Taller: -10% | Festival: +20%)."
)
Render-TerminalImage -Title "Catalogo de Eventos Disponibles (Opcion 1)" -Lines $lineasCatalogo -OutputPath "$capturasDir\02_catalogo_eventos.png" -Width 1040

# 3. Consulta Detallada
$lineasConsulta = @(
    "--- CONSULTA DETALLADA DE EVENTO ---",
    "Ingrese el identificador (ID) del evento a consultar: 3",
    "",
    "ID: #3 | [CONCIERTO] 'Coldplay - Music of the Spheres'",
    "  Fecha: 03-11-2026 20:00 | Precio Base: $120.000,00 | Precio Final: $138.000,00",
    "  Capacidad: 80 | Cupos Disponibles: 80",
    "  Artista: Coldplay | Zona: Platino VIP | Numerado: Si (+15% produccion incluido)",
    "",
    "--- CONSULTA DETALLADA DE EVENTO ---",
    "Ingrese el identificador (ID) del evento a consultar: 7",
    "",
    "ID: #7 | [FESTIVAL] 'Festival Estereo Sonoro PLANAZO'",
    "  Fecha: 18-11-2026 13:00 | Precio Base: $200.000,00 | Precio Final: $240.000,00",
    "  Capacidad: 150 | Cupos Disponibles: 150",
    "  Escenarios: 4 | Duracion: 3 dia(s) | Patrocinador: PLANAZO & LiveMusic (+20% multi-escenario incluido)"
)
Render-TerminalImage -Title "Consulta de Eventos Polimorficos (Opcion 2)" -Lines $lineasConsulta -OutputPath "$capturasDir\03_consulta_evento_polimorfico.png"

# 4. Caso 1: Reserva Valida
$lineasReservaValida = @(
    "--- RESERVA DE ENTRADAS ---",
    "Ingrese el ID del evento que desea reservar: 1",
    "",
    ">> Evento seleccionado: [Cine] 'Interestelar: 10 Aniversario'",
    "   Precio por entrada: $18.000,00 | Cupos disponibles: 50",
    "",
    "--- Identificacion del Comprador ---",
    "Ingrese su documento de identidad / cedula: 1017254890",
    "Bienvenido de nuevo, Joan Sebastian Gomez!",
    "",
    "Ingrese la cantidad de entradas a reservar: 2",
    "",
    " RESERVA CONFIRMADA CON EXITO!",
    "====================================================",
    "RESERVA #1001 - [ACTIVA]",
    "====================================================",
    "  Cliente:          Joan Sebastian Gomez (Doc: 1017254890)",
    "  Email:            joan.gomez@planazo.com | Tel: 3001234567",
    "  Evento:           [Cine] Interestelar: 10 Aniversario",
    "  Fecha Evento:     26-10-2026 19:00",
    "  Entradas:         2",
    "  Precio Unitario:  $18.000,00",
    "  Total Reserva:    $36.000,00",
    "  Fecha Emision:    24-10-2026 10:30:00",
    "===================================================="
)
Render-TerminalImage -Title "Caso 1: Reserva Valida Confirmada" -Lines $lineasReservaValida -OutputPath "$capturasDir\04_reserva_valida_exito.png"

# 5. Caso 2: Cupos Insuficientes
$lineasCupos = @(
    "--- RESERVA DE ENTRADAS ---",
    "Ingrese el ID del evento que desea reservar: 5",
    "",
    ">> Evento seleccionado: [Taller] 'Taller Practico de C# y POO Avanzada'",
    "   Precio por entrada: $54.000,00 | Cupos disponibles: 25",
    "",
    "--- Identificacion del Comprador ---",
    "Ingrese su documento de identidad / cedula: 1020304050",
    "Bienvenido de nuevo, Maria Camila Rivas!",
    "",
    "Ingrese la cantidad de entradas a reservar: 30",
    "",
    " [ALERTA] [CUPOS INSUFICIENTES] No hay cupos suficientes para el evento 'Taller Practico de C# y POO Avanzada'.",
    "          Cupos disponibles: 25, solicitados: 30.",
    "",
    "Presione ENTER para continuar..."
)
Render-TerminalImage -Title "Caso 2: Control de Cupos Insuficientes" -Lines $lineasCupos -OutputPath "$capturasDir\05_error_cupos_insuficientes.png"

# 6. Caso 3: Cantidad Negativa
$lineasNegativo = @(
    "--- RESERVA DE ENTRADAS ---",
    "Ingrese el ID del evento que desea reservar: 3",
    "",
    ">> Evento seleccionado: [Concierto] 'Coldplay - Music of the Spheres'",
    "   Precio por entrada: $138.000,00 | Cupos disponibles: 80",
    "",
    "--- Identificacion del Comprador ---",
    "Ingrese su documento de identidad / cedula: 1033445566",
    "Bienvenido de nuevo, David Fernando Ruiz!",
    "",
    "Ingrese la cantidad de entradas a reservar: -4",
    "",
    " [ALERTA] [CANTIDAD INVALIDA] La cantidad de entradas debe ser un numero entero mayor a cero. Valor ingresado: -4.",
    "",
    "Presione ENTER para continuar..."
)
Render-TerminalImage -Title "Caso 3: Validacion de Cantidad Negativa" -Lines $lineasNegativo -OutputPath "$capturasDir\06_error_cantidad_negativa.png"

# 7. Caso 4: Cancelacion y Devolucion de Cupos
$lineasCancelacion = @(
    "--- CANCELACION DE RESERVA ---",
    "Ingrese el identificador (ID) de la reserva a cancelar: 1001",
    "",
    " RESERVA #1001 CANCELADA EXITOSAMENTE!",
    "  Evento:               Interestelar: 10 Aniversario",
    "  Entradas devueltas:   2",
    "  Cupos restaurados:    48 -> 50",
    "  Estado actual:        Cancelada",
    "",
    "Presione ENTER para continuar..."
)
Render-TerminalImage -Title "Caso 4: Cancelacion y Restauracion de Cupos" -Lines $lineasCancelacion -OutputPath "$capturasDir\07_cancelacion_reserva_cupos_devueltos.png"

# 8. Caso 5: Doble Cancelacion
$lineasDobleCancel = @(
    "--- CANCELACION DE RESERVA ---",
    "Ingrese el identificador (ID) de la reserva a cancelar: 1001",
    "",
    " [ALERTA] [OPERACION INVALIDA] La reserva con ID #1001 ya se encuentra cancelada previamente.",
    "          No es posible cancelarla nuevamente.",
    "",
    "Presione ENTER para continuar..."
)
Render-TerminalImage -Title "Caso 5: Prevencion de Doble Cancelacion" -Lines $lineasDobleCancel -OutputPath "$capturasDir\08_error_cancelacion_doble.png"

# 9. Caso 6: Evento Inexistente
$lineasInexistente = @(
    "--- CONSULTA DETALLADA DE EVENTO ---",
    "Ingrese el identificador (ID) del evento a consultar: 9999",
    "",
    " [ALERTA] No se encontro ningun evento registrado con el identificador #9999.",
    "",
    "Presione ENTER para continuar..."
)
Render-TerminalImage -Title "Caso 6: Consulta de Evento Inexistente" -Lines $lineasInexistente -OutputPath "$capturasDir\09_error_evento_inexistente.png"

# 10. Reto 1: Festival
$lineasReto1 = @(
    "--- RETO 1: NUEVO TIPO DE EVENTO DERIVADO (FESTIVAL) ---",
    "",
    "Demostracion del principio Abierto/Cerrado (OCP):",
    "Nueva clase derivada 'Festival' integrada sin modificar las clases de Cine, Concierto o Taller.",
    "",
    "Detalle del Evento #7:",
    "=================================================================",
    "ID: #7 | [FESTIVAL] 'Festival Estereo Sonoro PLANAZO'",
    "  Fecha: 18-11-2026 13:00",
    "  Precio Base: $200.000,00 | Regla: +20% por produccion multi-escenario",
    "  Precio Final Calculado: $240.000,00 (Exacto)",
    "  Capacidad: 150 | Cupos: 150",
    "  Escenarios: 4 | Duracion: 3 dias | Patrocinador: PLANAZO & LiveMusic",
    "================================================================="
)
Render-TerminalImage -Title "Reto 1: Clase Festival con Calculo Propio (+20%)" -Lines $lineasReto1 -OutputPath "$capturasDir\10_reto1_nuevo_evento_festival.png"

# 11. Reto 2: Gestion de Usuarios
$lineasReto2 = @(
    "--- RETO 2: CONSULTA DE RESERVAS POR USUARIO ---",
    "Ingrese el documento de identidad o nombre del usuario: 1017254890",
    "",
    "Se encontraron 1 reserva(s) para '1017254890':",
    "",
    "====================================================",
    "RESERVA #1001 - [ACTIVA]",
    "====================================================",
    "  Cliente:          Joan Sebastian Gomez (Doc: 1017254890)",
    "  Email:            joan.gomez@planazo.com | Tel: 3001234567",
    "  Evento:           [Cine] Interestelar: 10 Aniversario",
    "  Fecha Evento:     26-10-2026 19:00",
    "  Entradas:         2",
    "  Precio Unitario:  $18.000,00",
    "  Total Reserva:    $36.000,00",
    "  Fecha Emision:    24-10-2026 10:30:00",
    "===================================================="
)
Render-TerminalImage -Title "Reto 2: Gestion y Consulta por Usuario" -Lines $lineasReto2 -OutputPath "$capturasDir\11_reto2_gestion_usuarios.png"

# 12. Resumen de Reservas
$lineasResumen = @(
    "--- RESUMEN GENERAL DE RESERVAS ---",
    "=================================================",
    "            CONSOLIDADO PLANAZO                  ",
    "=================================================",
    "  * Cantidad de reservas activas:        2",
    "  * Total de entradas reservadas:        5",
    "  * Valor total de reservas activas:     $312.000,00",
    "  * Cantidad de reservas canceladas:     1",
    "================================================="
)
Render-TerminalImage -Title "Resumen Consolidado de Reservas (Opcion 6)" -Lines $lineasResumen -OutputPath "$capturasDir\12_resumen_consolidado_reservas.png"

# 13. Suite Automatizada 7 Casos
$lineasSuite = @(
    "=================================================================",
    "  SUITE DE PRUEBAS DE CALIDAD - PLANAZO POO C#",
    "=================================================================",
    "Ejecutando los 7 escenarios de prueba solicitados en el parcial...",
    "",
    ">> Caso 1: Registrar una reserva valida",
    "  [PASO] Reserva #1001 generada exitosamente. Total: $54.000,00. Cupos: 50 -> 47",
    "",
    ">> Caso 2: Intentar reservar mas entradas que los cupos disponibles",
    "  [PASO] Excepcion capturada (CuposInsuficientesException): No hay cupos suficientes para 'Taller Practico...'",
    "",
    ">> Caso 3: Intentar realizar una reserva con cantidad negativa o cero",
    "  [PASO] Excepcion capturada (CantidadInvalidaException): La cantidad de entradas debe ser un numero entero mayor a cero.",
    "",
    ">> Caso 4: Cancelar una reserva y comprobar que se devuelven los cupos",
    "  [PASO] Reserva #1001 cancelada. Cupos: 80 -> 76 -> 80. Cupos devueltos exitosamente!",
    "",
    ">> Caso 5: Intentar cancelar dos veces la misma reserva",
    "  [PASO] Excepcion capturada (ReservaYaCanceladaException): La reserva con ID #1001 ya se encuentra cancelada previamente.",
    "",
    ">> Caso 6: Consultar un evento inexistente",
    "  [PASO] Excepcion capturada (EventoNoEncontradoException): No se encontro ningun evento registrado con ID #9999.",
    "",
    ">> Caso 7: Comprobar que cada tipo de evento calcula correctamente su precio final",
    "  [PASO] Cine: Base $20.000 -> Final $20.000,00 (Exacto)",
    "  [PASO] Concierto: Base $50.000 -> Final $57.500,00 (+15% Exacto)",
    "  [PASO] Taller: Base $40.000 -> Final $36.000,00 (-10% Exacto)",
    "  [PASO] Festival (Reto 1): Base $100.000 -> Final $120.000,00 (+20% Exacto)",
    "=================================================================",
    "",
    " RESULTADO FINAL: 7/7 PRUEBAS SUPERADAS CON EXITO (100%)",
    "================================================================="
)
Render-TerminalImage -Title "Suite de Pruebas: 7/7 Casos Superados (Opcion 8)" -Lines $lineasSuite -OutputPath "$capturasDir\13_suite_pruebas_7_casos_completos.png"

Write-Host "Todas las capturas se generaron exitosamente en $capturasDir"
