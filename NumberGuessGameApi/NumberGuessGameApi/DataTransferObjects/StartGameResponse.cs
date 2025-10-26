using System;

namespace NumberGuessGameApi.DataTransferObjects
{
    /// <summary>
    /// DTO para la respuesta exitosa del endpoint de inicio de juego.
    /// </summary>
    /// <remarks>
    /// Este DTO contiene la información que el cliente necesita después de iniciar un juego.
    /// 
    /// ¿Por qué se devuelven estos campos específicos?
    /// 
    /// 1. GameId: El cliente DEBE conocer este ID para hacer intentos de adivinanza.
    ///    Cada solicitud a /api/game/v1/guess requerirá este GameId.
    /// 
    /// 2. PlayerId: Se devuelve como confirmación de qué jugador inició el juego.
    ///    Útil para logging del lado del cliente y validación.
    /// 
    /// 3. CreatedAt: Permite al cliente mostrar cuándo comenzó el juego,
    ///    calcular tiempo transcurrido, o implementar límites de tiempo.
    /// 
    /// Nota importante: NO se devuelve el número secreto (SecretNumber).
    /// Esto sería hacer trampa y arruinar el juego.
    /// 
    /// Ejemplo de respuesta JSON:
    /// {
    ///   "gameid": 34,
    ///   "playerid": 1,
    ///   "createat": "2025-10-08T19:01:23"
    /// }
    /// 
    /// Nota sobre el nombre "createat": Según el documento, debe ser "createat"
    /// (sin 'd' al final), aunque gramaticalmente sería "createdat".
    /// Respetamos la especificación del documento.
    /// </remarks>
    public class StartGameResponse
    {
        /// <summary>
        /// Identificador único del juego recién creado.
        /// </summary>
        /// <remarks>
        /// Este ID se genera automáticamente por la base de datos al crear el juego.
        /// Es la clave primaria de la tabla Games.
        /// 
        /// El cliente debe almacenar este ID para:
        /// 1. Enviar intentos de adivinanza (GuessNumberRequest requiere GameId)
        /// 2. Consultar el estado del juego
        /// 3. Identificar el juego en su interfaz de usuario
        /// 
        /// Este ID es único en todo el sistema, no solo por jugador.
        /// </remarks>
        public int GameId { get; set; }

        /// <summary>
        /// Identificador del jugador que inició este juego.
        /// </summary>
        /// <remarks>
        /// Se devuelve como confirmación y para facilitar el seguimiento del cliente.
        /// 
        /// Aunque el cliente ya conoce su propio PlayerId (lo envió en la solicitud),
        /// devolverlo en la respuesta:
        /// 1. Confirma que la solicitud se procesó correctamente
        /// 2. Permite al cliente validar que el juego se creó para el jugador correcto
        /// 3. Facilita el logging y debugging
        /// 4. Mantiene consistencia en el formato de respuestas
        /// </remarks>
        public int PlayerId { get; set; }

        /// <summary>
        /// Fecha y hora en que se creó el juego.
        /// </summary>
        /// <remarks>
        /// DateTime representa tanto la fecha como la hora con precisión de milisegundos.
        /// 
        /// Este campo se utiliza para:
        /// 1. Mostrar al usuario cuándo comenzó el juego
        /// 2. Calcular el tiempo transcurrido desde el inicio
        /// 3. Ordenar juegos cronológicamente
        /// 4. Implementar límites de tiempo si es necesario
        /// 5. Auditoría y análisis de patrones de juego
        /// 
        /// ASP.NET Core serializa automáticamente DateTime a formato ISO 8601:
        /// "2025-10-08T19:01:23.1234567" (con zona horaria si se usa DateTimeOffset)
        /// 
        /// El nombre de la propiedad es "CreatedAt" en C#, pero se serializa como
        /// "createat" en JSON para coincidir con la especificación del documento.
        /// Esto se puede configurar con JsonPropertyName si es necesario.
        /// </remarks>
        public DateTime CreatedAt { get; set; }
    }
}