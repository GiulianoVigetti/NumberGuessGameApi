namespace NumberGuessGameApi.DataTransferObjects
{
    /// <summary>
    /// DTO para la respuesta de un intento de adivinanza.
    /// </summary>
    /// <remarks>
    /// Este DTO contiene la retroalimentación del sistema después de cada intento.
    /// 
    /// El campo más importante es Message, que contiene:
    /// 1. Pistas de Picas y Famas (si no adivinó)
    /// 2. Mensaje de felicitación (si adivinó correctamente)
    /// 
    /// Ejemplos de mensajes generados por ESCMB.GuessCore:
    /// - "Tu número tiene 1 fama y 2 pica"
    /// - "Tu número tiene 3 fama 0 pica"
    /// - "¡Felicidades! Has adivinado el número."
    /// 
    /// Flujo de interpretación de pistas:
    /// - FAMAS (4 máximo): Dígitos correctos en posición correcta
    ///   Número secreto: 5604
    ///   Intento: 5624
    ///   → 5, 6, 4 están correctos y en posición correcta = 3 famas
    /// 
    /// - PICAS: Dígitos correctos pero en posición incorrecta
    ///   Número secreto: 5604
    ///   Intento: 2564
    ///   → 5 en posición 2 (correcta posición) = 1 fama
    ///   → 6 existe pero en posición 3 en lugar de 2 = 1 pica
    ///   → 4 existe pero en posición 4 en lugar de 4 = 0 (está bien posicionado, ya contado en fama)
    ///   
    /// Cuando Famas = 4: El jugador adivinó completamente el número.
    /// El sistema marca automáticamente el juego como finalizado (IsFinished = true).
    /// 
    /// Ejemplo de respuesta exitosa:
    /// {
    ///   "gameid": 1,
    ///   "attemptedNumber": 5604,
    ///   "message": "¡Felicidades! Has adivinado el número."
    /// }
    /// 
    /// Ejemplo de respuesta con pistas:
    /// {
    ///   "gameid": 1,
    ///   "attemptedNumber": 5624,
    ///   "message": "Tu número tiene 3 fama 0 pica"
    /// }
    /// </remarks>
    public class GuessNumberResponse
    {
        /// <summary>
        /// Identificador del juego al que pertenece este intento.
        /// </summary>
        /// <remarks>
        /// Se devuelve como confirmación para que el cliente pueda:
        /// 1. Verificar que el intento se registró en el juego correcto
        /// 2. Mantener sincronización en aplicaciones multi-juego
        /// 3. Implementar logging y debugging del lado del cliente
        /// 4. Asociar la respuesta con el contexto correcto en la UI
        /// 
        /// Aunque el cliente envió este GameId en la solicitud,
        /// devolverlo confirma el procesamiento exitoso.
        /// </remarks>
        public int GameId { get; set; }

        /// <summary>
        /// El número que el jugador intentó adivinar.
        /// </summary>
        /// <remarks>
        /// Se devuelve como confirmación del número procesado.
        /// 
        /// Esto es útil para:
        /// 1. Confirmar al cliente qué número se procesó
        /// 2. Evitar confusiones en caso de múltiples solicitudes concurrentes
        /// 3. Mostrar el historial de intentos en la interfaz
        /// 4. Debugging y logging
        /// 
        /// Ejemplo de uso en la UI:
        /// "Has intentado: 1234"
        /// "Resultado: Tu número tiene 2 fama y 1 pica"
        /// </remarks>
        public int AttemptedNumber { get; set; }

        /// <summary>
        /// Mensaje con las pistas o la confirmación de éxito.
        /// </summary>
        /// <remarks>
        /// Este mensaje es generado por la librería ESCMB.GuessCore.
        /// 
        /// Tipos de mensajes posibles:
        /// 
        /// 1. PISTAS (cuando no adivinó):
        ///    - "Tu número tiene X fama y Y pica"
        ///    - X = cantidad de dígitos correctos en posición correcta (0-4)
        ///    - Y = cantidad de dígitos correctos en posición incorrecta (0-4)
        ///    
        /// 2. ÉXITO (cuando adivinó):
        ///    - "¡Felicidades! Has adivinado el número."
        ///    - Indica que Famas = 4
        ///    - El juego se marca automáticamente como IsFinished = true
        /// 
        /// Interpretación de pistas para el jugador:
        /// 
        /// - "0 fama 0 pica": Ningún dígito es correcto
        /// - "1 fama 0 pica": 1 dígito correcto en su posición, los otros 3 no están
        /// - "2 fama 1 pica": 2 dígitos bien posicionados, 1 existe pero mal ubicado
        /// - "3 fama 0 pica": 3 dígitos correctos, solo falta ajustar 1
        /// - "4 fama": ¡Ganaste! (no se muestran picas porque todas son famas)
        /// 
        /// El cliente debe mostrar este mensaje directamente al usuario,
        /// ya que contiene toda la información necesaria para continuar jugando.
        /// 
        /// El service registra cada mensaje en la tabla Attempts para:
        /// - Historial completo de intentos
        /// - Auditoría del juego
        /// - Análisis de estrategias de juego
        /// - Métricas en Power BI
        /// </remarks>
        public string Message { get; set; }
    }
}