// ⭐ IMPORTAR LA LIBRERÍA ESCMB.GameCore
//using ESCMB.GameCore;
//using ESCMB;
using GameCore;
//using ESCMB.GameCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NumberGuessGameApi.Data;
using NumberGuessGameApi.DataTransferObjects;
using NumberGuessGameApi.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NumberGuessGameApi.Services
{
    /// <summary>
    /// Implementación del servicio de lógica de negocio para el juego Picas y Famas.
    /// Utiliza la librería ESCMB.GameCore para el cálculo de Picas y Famas.
    /// </summary>
    public class GameService : IGameService
    {
        private readonly GameDbContext _context;
        private readonly ILogger<GameService> _logger;

        /// <summary>
        /// Constructor con inyección de dependencias.
        /// </summary>
        /// <param name="context">Contexto de base de datos</param>
        /// <param name="logger">Logger para auditoría</param>
        public GameService(GameDbContext context, ILogger<GameService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Registra un nuevo jugador en el sistema.
        /// </summary>
        public async Task<(bool success, string message, RegisterPlayerResponse response)> RegisterPlayerAsync(RegisterPlayerRequest request)
        {
            _logger.LogInformation(
                "Iniciando registro de jugador: {FirstName} {LastName}, Edad: {Age}",
                request.FirstName,
                request.LastName,
                request.Age
            );

            try
            {
                var existingPlayer = await _context.Players
                    .AnyAsync(p =>
                        p.FirstName == request.FirstName &&
                        p.LastName == request.LastName
                    );

                if (existingPlayer)
                {
                    _logger.LogWarning(
                        "Intento de registro duplicado: {FirstName} {LastName}",
                        request.FirstName,
                        request.LastName
                    );

                    return (false, "El jugador ya se encuentra registrado con ese nombre y apellido", null);
                }

                var newPlayer = new Player
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Age = request.Age,
                    RegistrationDate = DateTime.UtcNow
                };

                _context.Players.Add(newPlayer);
                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Jugador registrado exitosamente: {FirstName} {LastName}, PlayerId: {PlayerId}",
                    newPlayer.FirstName,
                    newPlayer.LastName,
                    newPlayer.PlayerId
                );

                var response = new RegisterPlayerResponse
                {
                    PlayerId = newPlayer.PlayerId
                };

                return (true, "Jugador registrado exitosamente", response);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error al registrar jugador: {FirstName} {LastName}",
                    request.FirstName,
                    request.LastName
                );

                return (false, "Error interno al registrar el jugador", null);
            }
        }

        /// <summary>
        /// Inicia un nuevo juego para un jugador.
        /// </summary>
        public async Task<(bool success, string message, StartGameResponse response)> StartGameAsync(StartGameRequest request)
        {
            _logger.LogInformation(
                "Iniciando nuevo juego para PlayerId: {PlayerId}",
                request.PlayerId
            );

            try
            {
                var player = await _context.Players.FindAsync(request.PlayerId);

                if (player == null)
                {
                    _logger.LogWarning(
                        "Intento de iniciar juego con PlayerId inexistente: {PlayerId}",
                        request.PlayerId
                    );

                    return (false, $"El jugador con ID {request.PlayerId} no existe", null);
                }

                var hasActiveGame = await _context.Games
                    .AnyAsync(g => g.PlayerId == request.PlayerId && !g.IsFinished);

                if (hasActiveGame)
                {
                    _logger.LogWarning(
                        "Intento de iniciar juego teniendo uno activo. PlayerId: {PlayerId}",
                        request.PlayerId
                    );

                    return (false, "Ya tienes un juego activo. Debes finalizarlo antes de iniciar uno nuevo", null);
                }

                string secretNumber = GenerateSecretNumber();

                var newGame = new Game
                {
                    PlayerId = request.PlayerId,
                    SecretNumber = secretNumber,
                    CreatedAt = DateTime.UtcNow,
                    IsFinished = false
                };

                _context.Games.Add(newGame);
                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Juego creado exitosamente. GameId: {GameId}, PlayerId: {PlayerId}",
                    newGame.GameId,
                    newGame.PlayerId
                );

                var response = new StartGameResponse
                {
                    GameId = newGame.GameId,
                    PlayerId = newGame.PlayerId,
                    CreatedAt = newGame.CreatedAt
                };

                return (true, "Juego iniciado exitosamente", response);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error al iniciar juego para PlayerId: {PlayerId}",
                    request.PlayerId
                );

                return (false, "Error interno al iniciar el juego", null);
            }
        }

        /// <summary>
        /// Procesa un intento de adivinanza del número secreto.
        /// ⭐ UTILIZA ESCMB.GameCore PARA CALCULAR PICAS Y FAMAS.
        /// </summary>
        public async Task<(bool success, string message, GuessNumberResponse response)> GuessNumberAsync(GuessNumberRequest request)
        {
            _logger.LogInformation(
                "Procesando intento para GameId: {GameId}, Número: {AttemptedNumber}",
                request.GameId,
                request.AttemptedNumber
            );

            try
            {
                string attemptedNumberStr = request.AttemptedNumber.ToString("D4");

                if (attemptedNumberStr.Length != 4)
                {
                    _logger.LogWarning(
                        "Número con longitud incorrecta: {AttemptedNumber}",
                        request.AttemptedNumber
                    );
                    return (false, "El número debe tener exactamente 4 dígitos", null);
                }

                if (attemptedNumberStr.Distinct().Count() != 4)
                {
                    _logger.LogWarning(
                        "Número con dígitos repetidos: {AttemptedNumber}",
                        attemptedNumberStr
                    );
                    return (false, "El número no puede tener dígitos repetidos", null);
                }

                var game = await _context.Games.FindAsync(request.GameId);

                if (game == null)
                {
                    _logger.LogWarning(
                        "Intento en juego inexistente: GameId {GameId}",
                        request.GameId
                    );
                    return (false, $"El juego con ID {request.GameId} no existe", null);
                }

                if (game.IsFinished)
                {
                    _logger.LogWarning(
                        "Intento en juego finalizado: GameId {GameId}",
                        request.GameId
                    );
                    return (false, $"El juego {request.GameId} ya ha finalizado", null);
                }

                // ⭐⭐⭐ USAR ESCMB.GameCore PARA CALCULAR PICAS Y FAMAS ⭐⭐⭐
                // La librería ESCMB.GameCore proporciona la clase GameLogic
                // que tiene el método EvaluateGuess para comparar números

                //string resultMessage;

                /*try
                {
                    // Crear instancia de GameLogic con el número secreto
                    // GameLogic es la clase principal de ESCMB.GameCore
                    //var gameLogic = new GameLogic(game.SecretNumber);
  
                    var gameLogic = new GameLogic(game.SecretNumber);

                    // EvaluateGuess compara el intento con el secreto
                    // y devuelve un string con el mensaje de resultado
                    //EvaluateGuess
                    resultMessage = gameLogic.EvaluateGuess(attemptedNumberStr);

                    

                    _logger.LogInformation(
                        "Resultado de ESCMB.GameCore para GameId {GameId}: {Message}",
                        game.GameId,
                        resultMessage
                    );
                }*/
                string resultMessage;
                GameCore.EvaluatorResult evaluationResult; // Declaramos una variable para el objeto devuelto

                try
                {
                    // Llamada al método estático correcto que devuelve el objeto EvaluatorResult
                    evaluationResult = Evaluator.Validate(game.SecretNumber, attemptedNumberStr);

                    // Convertimos el resultado del objeto a string, asumiendo que tiene un método o propiedad para el mensaje

                    // ⭐ PROBABLEMENTE EL OBJETO TIENE UNA PROPIEDAD LLAMADA 'Message'
                    resultMessage = evaluationResult.Message;

                    // Si la anterior falla, la librería puede tener un método para formatear el resultado
                    // resultMessage = evaluationResult.FormatResult(); 

                    // O puede ser necesario construir el mensaje a mano (usando Picas y Famas):
                    /*
                    if (evaluationResult.Famas == 4)
                    {
                        resultMessage = "¡Felicidades! Has adivinado el número.";
                    }
                    else
                    {
                        resultMessage = $"Tu número tiene {evaluationResult.Famas} fama y {evaluationResult.Picas} pica";
                    }
                    */

                    _logger.LogInformation(
                        "Resultado de ESCMB.GameCore para GameId {GameId}: {Message}",
                        game.GameId,
                        resultMessage // Usamos el string extraído del objeto
                    );
                }
                catch (Exception coreEx)
                {
                    // Si hay error con GameCore, loguearlo pero no fallar
                    _logger.LogError(
                        coreEx,
                        "Error al usar ESCMB.GameCore. GameId: {GameId}",
                        game.GameId
                    );

                    // Fallback: usar implementación manual si GameCore falla
                    resultMessage = CalculatePicasYFamasFallback(game.SecretNumber, attemptedNumberStr);
                }

                var attempt = new Attempt
                {
                    GameId = request.GameId,
                    AttemptedNumber = attemptedNumberStr,
                    Message = resultMessage,
                    AttemptedAt = DateTime.UtcNow
                };

                _context.Attempts.Add(attempt);

                // Verificar si adivinó (4 famas o mensaje de éxito)
                bool isCorrect = resultMessage.Contains("Felicidades") ||
                                 resultMessage.Contains("adivinado") ||
                                 attemptedNumberStr == game.SecretNumber;

                if (isCorrect)
                {
                    game.IsFinished = true;
                    _context.Games.Update(game);

                    var attemptCount = await _context.Attempts.CountAsync(a => a.GameId == game.GameId);

                    _logger.LogInformation(
                        "Juego completado. GameId: {GameId}, Intentos totales: {AttemptCount}",
                        game.GameId,
                        attemptCount + 1
                    );
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Intento procesado. GameId: {GameId}, Resultado: {Message}",
                    game.GameId,
                    resultMessage
                );

                var response = new GuessNumberResponse
                {
                    GameId = request.GameId,
                    AttemptedNumber = request.AttemptedNumber,
                    Message = resultMessage
                };

                return (true, resultMessage, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error al procesar intento. GameId: {GameId}, Número: {AttemptedNumber}",
                    request.GameId,
                    request.AttemptedNumber
                );

                return (false, "Error interno al procesar el intento", null);
            }
        }

        /// <summary>
        /// Genera un número secreto de 4 dígitos sin repetir.
        /// Utiliza el algoritmo Fisher-Yates shuffle.
        /// </summary>
        private string GenerateSecretNumber()
        {
            var random = new Random();
            var digits = Enumerable.Range(0, 10).ToArray();

            for (int i = digits.Length - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                int temp = digits[i];
                digits[i] = digits[j];
                digits[j] = temp;
            }

            return string.Join("", digits.Take(4).Select(d => d.ToString()));
        }

        /// <summary>
        /// Implementación de respaldo en caso de que ESCMB.GameCore falle.
        /// NO debe usarse normalmente, solo como fallback.
        /// </summary>
        private string CalculatePicasYFamasFallback(string secretNumber, string guessNumber)
        {
            _logger.LogWarning("Usando implementación de respaldo (fallback) para calcular Picas y Famas");

            int famas = 0;
            int picas = 0;

            for (int i = 0; i < 4; i++)
            {
                if (secretNumber[i] == guessNumber[i])
                {
                    famas++;
                }
            }

            for (int i = 0; i < 4; i++)
            {
                if (secretNumber.Contains(guessNumber[i]))
                {
                    if (secretNumber[i] != guessNumber[i])
                    {
                        picas++;
                    }
                }
            }

            if (famas == 4)
            {
                return "¡Felicidades! Has adivinado el número.";
            }

            return $"Tu número tiene {famas} fama y {picas} pica";
        }
    }

    }