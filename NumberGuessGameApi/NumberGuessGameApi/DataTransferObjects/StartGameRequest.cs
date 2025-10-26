using System.ComponentModel.DataAnnotations;

namespace NumberGuessGameApi.DataTransferObjects
{
    /// <summary>
    /// DTO para la solicitud de inicio de un nuevo juego.
    /// </summary>
    /// <remarks>
    /// Este DTO representa los datos que un jugador debe enviar para iniciar una nueva partida.
    /// 
    /// Solo requiere el PlayerId porque:
    /// 1. El sistema genera automáticamente el número secreto
    /// 2. El sistema asigna automáticamente el GameId
    /// 3. El sistema registra automáticamente la fecha de creación
    /// 
    /// El endpoint validará:
    /// - Que el PlayerId exista en la base de datos
    /// - Que el jugador no tenga un juego activo (IsFinished = false)
    /// - Que el PlayerId tenga el formato correcto
    /// 
    /// Flujo típico:
    /// 1. Usuario se registra y obtiene PlayerId = 5
    /// 2. Usuario envía { "playerid": 5 } a /api/game/v1/start
    /// 3. Sistema crea juego con número secreto aleatorio
    /// 4. Sistema devuelve el GameId para que el usuario empiece a jugar
    /// 
    /// Ejemplo de JSON:
    /// {
    ///   "playerid": 5
    /// }
    /// </remarks>
    public class StartGameRequest
    {
        /// <summary>
        /// Identificador del jugador que desea iniciar un nuevo juego.
        /// </summary>
        /// <remarks>
        /// [Required] - Valida que este campo no sea nulo.
        /// 
        /// Para un tipo int, Required valida que:
        /// 1. El campo esté presente en el JSON
        /// 2. Tenga un valor numérico válido
        /// 
        /// Nota: int es un tipo valor (no nullable), por lo que nunca será null por defecto.
        /// Sin embargo, Required sigue siendo útil porque:
        /// - Valida que el campo esté presente en el JSON
        /// - Genera un error claro si falta el campo
        /// - Documenta que el campo es obligatorio
        /// 
        /// [Range(1, int.MaxValue)] - Valida que el ID sea positivo.
        /// Esta validación adicional:
        /// 1. Previene valores cero o negativos (que no son IDs válidos)
        /// 2. Mejora la claridad del error para el cliente
        /// 3. Evita consultas innecesarias a la base de datos con IDs inválidos
        /// 
        /// ErrorMessage proporciona retroalimentación específica al usuario.
        /// 
        /// int.MaxValue es el valor máximo de un entero (2,147,483,647),
        /// permitiendo cualquier ID positivo válido.
        /// 
        /// Validaciones posteriores en el service verificarán:
        /// - Si el PlayerId existe en la tabla Players
        /// - Si el jugador tiene un juego activo
        /// </remarks>
        [Required(ErrorMessage = "El ID del jugador es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID del jugador debe ser un número positivo")]
        public int PlayerId { get; set; }
    }
}
