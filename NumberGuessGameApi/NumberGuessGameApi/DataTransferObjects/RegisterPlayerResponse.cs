namespace NumberGuessGameApi.DataTransferObjects
{
    /// <summary>
    /// DTO para la respuesta del endpoint de registro de jugador.
    /// </summary>
    /// <remarks>
    /// Este DTO define qué información se devuelve al cliente después de un registro exitoso.
    /// 
    /// En lugar de devolver toda la entidad Player (con información sensible o innecesaria),
    /// solo devolvemos el PlayerId que el cliente necesita para interactuar con la API.
    /// 
    /// Ventajas de este enfoque:
    /// 1. MINIMALISMO: Solo devuelve lo necesario (el ID del jugador)
    /// 2. SEGURIDAD: No expone otros datos del jugador innecesariamente
    /// 3. CONSISTENCIA: Define un contrato claro de respuesta
    /// 4. VERSIONADO: Facilita cambios futuros en la API sin romper clientes
    /// 
    /// Ejemplo de respuesta JSON:
    /// {
    ///   "playerid": 42
    /// }
    /// </remarks>
    public class RegisterPlayerResponse
    {
        /// <summary>
        /// Identificador único del jugador recién registrado.
        /// </summary>
        /// <remarks>
        /// Este ID es generado automáticamente por la base de datos durante el registro.
        /// El cliente debe almacenar este ID para:
        /// 1. Iniciar nuevos juegos (enviándolo en StartGameRequest)
        /// 2. Identificarse en futuras interacciones con la API
        /// 
        /// El nombre de la propiedad es "PlayerId" (con mayúscula P) en C#,
        /// pero ASP.NET Core lo serializa como "playerid" (minúsculas) en JSON
        /// por la configuración predeterminada de camelCase.
        /// 
        /// Si se requiere exactamente "playerid" en minúsculas (como indica el documento),
        /// se puede usar el atributo [JsonPropertyName("playerid")] de System.Text.Json.
        /// </remarks>
        public int PlayerId { get; set; }
    }
}