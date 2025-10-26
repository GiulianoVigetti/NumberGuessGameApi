using System.ComponentModel.DataAnnotations;

namespace NumberGuessGameApi.DataTransferObjects
{
    /// <summary>
    /// DTO para la solicitud de intento de adivinanza del número secreto.
    /// </summary>
    /// <remarks>
    /// Este DTO representa cada intento que un jugador hace para adivinar el número.
    /// 
    /// Flujo de uso:
    /// 1. Usuario tiene un juego activo (recibió GameId al iniciar)
    /// 2. Usuario piensa un número de 4 dígitos sin repetir
    /// 3. Usuario envía { "gameid": 1, "attemptedNumber": 1234 }
    /// 4. Sistema valida el número y devuelve pistas (Picas/Famas)
    /// 5. Usuario repite hasta adivinar el número correcto
    /// 
    /// Validaciones que se realizan:
    /// - Nivel DTO (aquí): Formato básico del número
    /// - Nivel Service: Validación avanzada (4 dígitos sin repetir)
    /// - Nivel Service: El juego existe y está activo
    /// - Nivel Service: Comparación con el número secreto
    /// 
    /// Ejemplo de JSON válido:
    /// {
    ///   "gameid": 1,
    ///   "attemptedNumber": 5734
    /// }
    /// 
    /// Ejemplo de JSON inválido:
    /// {
    ///   "gameid": 1,
    ///   "attemptedNumber": 1233  // ← Tiene dígitos repetidos (dos 3)
    /// }
    /// </remarks>
    public class GuessNumberRequest
    {
        /// <summary>
        /// Identificador del juego en el que se está realizando el intento.
        /// </summary>
        /// <remarks>
        /// [Required] - Valida que el campo esté presente y tenga valor.
        /// 
        /// Este GameId permite al sistema:
        /// 1. Localizar el juego específico en la base de datos
        /// 2. Verificar que el juego exista
        /// 3. Verificar que el juego esté activo (IsFinished = false)
        /// 4. Obtener el número secreto para comparar
        /// 5. Asociar el intento con el juego correcto en la tabla Attempts
        /// 
        /// [Range(1, int.MaxValue)] - Valida que sea un ID positivo válido.
        /// Esto previene:
        /// - Valores negativos o cero (IDs inválidos)
        /// - Consultas innecesarias a la base de datos
        /// - Errores confusos para el usuario
        /// 
        /// El service validará posteriormente si este GameId:
        /// - Existe en la tabla Games
        /// - Pertenece a un juego activo (no finalizado)
        /// </remarks>
        [Required(ErrorMessage = "El ID del juego es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID del juego debe ser un número positivo")]
        public int GameId { get; set; }

        /// <summary>
        /// Número de 4 dígitos que el jugador intenta como adivinanza.
        /// </summary>
        /// <remarks>
        /// [Required] - Campo obligatorio.
        /// 
        /// Este campo se valida en múltiples niveles:
        /// 
        /// NIVEL 1 - Validación DTO (aquí):
        /// No hay validaciones complejas aquí porque queremos delegar
        /// la lógica al service para mejor manejo de errores y flexibilidad.
        /// 
        /// NIVEL 2 - Validación Service:
        /// - Debe tener exactamente 4 dígitos
        /// - No debe tener dígitos repetidos (1234 ✓, 1123 ✗)
        /// - Cada carácter debe ser un dígito (0-9)
        /// 
        /// ¿Por qué int en lugar de string?
        /// Se usa int porque:
        /// 1. Los números de 4 dígitos se representan naturalmente como enteros
        /// 2. JSON maneja enteros nativamente sin comillas
        /// 3. La librería ESCMB.GuessCore probablemente espera enteros
        /// 
        /// Consideraciones importantes:
        /// - El rango de un número de 4 dígitos es 0-9999
        /// - Números como 0123 se interpretan como 123 (pierde el cero inicial)
        /// - El service debe manejar esto y validar que tenga 4 dígitos
        /// 
        /// Ejemplo de validación en el service:
        /// string numberStr = attemptedNumber.ToString("D4"); // Fuerza 4 dígitos
        /// if (numberStr.Distinct().Count() != 4) // Verifica sin repetición
        /// {
        ///     return error;
        /// }
        /// 
        /// Ejemplos:
        /// - 1234 → válido (4 dígitos diferentes)
        /// - 5678 → válido
        /// - 123 → inválido (solo 3 dígitos)
        /// - 1233 → inválido (dígito 3 repetido)
        /// - 0123 → se interpreta como 123, el service lo rechazará
        /// </remarks>
        [Required(ErrorMessage = "El número a adivinar es requerido")]
        public int AttemptedNumber { get; set; }
    }
}