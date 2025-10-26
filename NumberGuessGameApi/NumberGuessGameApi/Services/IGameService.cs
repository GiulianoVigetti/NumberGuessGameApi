using System.Threading.Tasks;
using NumberGuessGameApi.DataTransferObjects;

namespace NumberGuessGameApi.Services
{
    /// <summary>
    /// Interfaz que define el contrato para el servicio de lógica de negocio del juego.
    /// </summary>
    public interface IGameService
    {
        /// <summary>
        /// Registra un nuevo jugador en el sistema.
        /// </summary>
        /// <param name="request">DTO con los datos del jugador (nombre, apellido, edad)</param>
        /// <returns>
        /// Tupla con:
        /// - success: true si el registro fue exitoso
        /// - message: Mensaje descriptivo del resultado
        /// - response: DTO con el PlayerId generado (puede ser null si falló)
        /// </returns>
        Task<(bool success, string message, RegisterPlayerResponse response)> RegisterPlayerAsync(RegisterPlayerRequest request);

        /// <summary>
        /// Inicia un nuevo juego para un jugador.
        /// </summary>
        /// <param name="request">DTO con el PlayerId del jugador</param>
        /// <returns>
        /// Tupla con:
        /// - success: true si el juego se inició correctamente
        /// - message: Mensaje descriptivo del resultado
        /// - response: DTO con GameId, PlayerId y CreatedAt (puede ser null si falló)
        /// </returns>
        Task<(bool success, string message, StartGameResponse response)> StartGameAsync(StartGameRequest request);

        /// <summary>
        /// Procesa un intento de adivinanza del número secreto.
        /// </summary>
        /// <param name="request">DTO con GameId y número intentado</param>
        /// <returns>
        /// Tupla con:
        /// - success: true si el intento se procesó correctamente
        /// - message: Mensaje descriptivo (puede ser mensaje de error si success=false)
        /// - response: DTO con el resultado del intento (puede ser null si falló)
        /// </returns>
        Task<(bool success, string message, GuessNumberResponse response)> GuessNumberAsync(GuessNumberRequest request);
    }
}