using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NumberGuessGameApi.DataTransferObjects;
using NumberGuessGameApi.Services;
using System.Threading.Tasks;

namespace NumberGuessGameApi.Controllers
{
    /// <summary>
    /// Controlador API para el juego Picas y Famas.
    /// </summary>
    /// <remarks>
    /// Este controlador es la CAPA DE PRESENTACIÓN de la aplicación.
    /// 
    /// RESPONSABILIDADES:
    /// 1. Recibir solicitudes HTTP
    /// 2. Validar formato de entrada (model binding y validación)
    /// 3. Llamar al servicio de lógica de negocio
    /// 4. Convertir resultados a respuestas HTTP apropiadas
    /// 5. NO contiene lógica de negocio (eso es responsabilidad del Service)
    /// 
    /// ATRIBUTOS DE LA CLASE:
    /// 
    /// [ApiController] - Marca esta clase como un controlador de API REST.
    /// Habilita comportamientos automáticos:
    /// - Validación automática del ModelState
    /// - Binding automático de parámetros desde [FromBody], [FromQuery], etc.
    /// - Respuestas automáticas 400 BadRequest si la validación falla
    /// - Inferencia automática del origen de parámetros
    /// 
    /// [Route("api/game/v1")] - Define la ruta base para todos los endpoints.
    /// Todos los métodos de este controller tendrán rutas que comiencen con:
    /// https://dominio.com/api/game/v1/...
    /// 
    /// El "v1" es una buena práctica de versionado de API:
    /// - Permite crear v2, v3 en el futuro sin romper clientes existentes
    /// - Facilita deprecación gradual de versiones antiguas
    /// - Mantiene compatibilidad hacia atrás
    /// </remarks>
    [ApiController]
    [Route("api/game/v1")]
    public class GameController : ControllerBase
    {
        // Campo privado para el servicio de lógica de negocio
        // readonly: Solo se asigna en el constructor
        private readonly IGameService _gameService;

        // Campo privado para el logger
        private readonly ILogger<GameController> _logger;

        /// <summary>
        /// Constructor con inyección de dependencias.
        /// </summary>
        /// <param name="gameService">Servicio de lógica de negocio</param>
        /// <param name="logger">Logger para el controlador</param>
        /// <remarks>
        /// ASP.NET Core automáticamente inyecta estas dependencias:
        /// - Busca en el contenedor de DI una implementación registrada de IGameService
        /// - Encuentra GameService (registrado en Program.cs)
        /// - Crea una instancia y la pasa al constructor
        /// - Hace lo mismo con ILogger
        /// 
        /// Esto permite:
        /// - Testear el controller con mocks
        /// - Cambiar la implementación sin modificar el controller
        /// - Gestión automática del ciclo de vida de las dependencias
        /// </remarks>
        public GameController(IGameService gameService, ILogger<GameController> logger)
        {
            _gameService = gameService;
            _logger = logger;
        }

        /// <summary>
        /// Endpoint para registrar un nuevo jugador.
        /// </summary>
        /// <param name="request">Datos del jugador a registrar</param>
        /// <returns>ActionResult con el PlayerId generado o mensaje de error</returns>
        /// <remarks>
        /// RUTA: POST /api/game/v1/register
        /// 
        /// [HttpPost("register")] - Define que este método responde a:
        /// - Método HTTP: POST
        /// - Ruta completa: /api/game/v1/register
        ///   (combina [Route] de la clase con "register")
        /// 
        /// [ProducesResponseType] - Documenta los posibles códigos de respuesta.
        /// Swagger/OpenAPI usa esto para generar documentación.
        /// StatusCodes.Status200OK = 200
        /// StatusCodes.Status400BadRequest = 400
        /// StatusCodes.Status500InternalServerError = 500
        /// 
        /// REQUEST BODY ESPERADO (JSON):
        /// {
        ///   "firstname": "Juan",
        ///   "lastname": "Pérez",
        ///   "age": 25
        /// }
        /// 
        /// RESPONSE EXITOSO (200):
        /// {
        ///   "playerid": 1
        /// }
        /// 
        /// RESPONSE ERROR (400):
        /// {
        ///   "message": "El jugador ya se encuentra registrado"
        /// }
        /// </remarks>
        [HttpPost("register")]
        [ProducesResponseType(typeof(RegisterPlayerResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> RegisterPlayer([FromBody] RegisterPlayerRequest request)
        {
            // [FromBody] - Indica que el parámetro viene del cuerpo de la solicitud HTTP.
            // ASP.NET Core automáticamente:
            // 1. Deserializa el JSON a un objeto RegisterPlayerRequest
            // 2. Valida las Data Annotations ([Required], [Range], etc.)
            // 3. Si la validación falla, devuelve 400 BadRequest automáticamente

            // LOGGING: Registrar solicitud recibida
            _logger.LogInformation(
                "Solicitud de registro recibida para: {FirstName} {LastName}",
                request.FirstName,
                request.LastName
            );

            // LLAMAR AL SERVICIO
            // await: Espera asíncronamente el resultado
            // La tupla se descompone en tres variables
            var (success, message, response) = await _gameService.RegisterPlayerAsync(request);

            // DECIDIR QUÉ RESPUESTA HTTP DEVOLVER

            if (success)
            {
                // ÉXITO: Devolver 200 OK con el response
                // Ok(): Método helper que crea un ObjectResult con status 200
                // Serializa automáticamente el objeto a JSON
                return Ok(response);
            }
            else
            {
                // ERROR: Determinar si es error del cliente (400) o del servidor (500)

                // Si el mensaje indica que el jugador ya existe → Error del cliente
                if (message.Contains("ya se encuentra registrado"))
                {
                    // BadRequest(): Crea una respuesta 400 Bad Request
                    // new { message }: Objeto anónimo que se serializa a JSON
                    // Resultado: { "message": "El jugador ya se encuentra registrado..." }
                    return BadRequest(new { message });
                }

                // Si es otro tipo de error → Error interno del servidor
                // StatusCode(): Permite especificar cualquier código de estado
                // 500 Internal Server Error indica un problema en el servidor
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message });
            }
        }

        /// <summary>
        /// Endpoint para iniciar un nuevo juego.
        /// </summary>
        /// <param name="request">PlayerId del jugador que inicia el juego</param>
        /// <returns>ActionResult con los datos del juego creado o mensaje de error</returns>
        /// <remarks>
        /// RUTA: POST /api/game/v1/start
        /// 
        /// REQUEST BODY (JSON):
        /// {
        ///   "playerid": 1
        /// }
        /// 
        /// RESPONSE EXITOSO (200):
        /// {
        ///   "gameid": 34,
        ///   "playerid": 1,
        ///   "createat": "2025-10-08T19:01:23"
        /// }
        /// 
        /// RESPONSE ERROR (404):
        /// {
        ///   "message": "El jugador con ID 1 no existe"
        /// }
        /// 
        /// RESPONSE ERROR (400):
        /// {
        ///   "message": "Ya tienes un juego activo..."
        /// }
        /// </remarks>
        [HttpPost("start")]
        [ProducesResponseType(typeof(StartGameResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> StartGame([FromBody] StartGameRequest request)
        {
            _logger.LogInformation(
                "Solicitud de inicio de juego para PlayerId: {PlayerId}",
                request.PlayerId
            );

            // Llamar al servicio
            var (success, message, response) = await _gameService.StartGameAsync(request);

            if (success)
            {
                // Éxito: Devolver 200 OK
                return Ok(response);
            }
            else
            {
                // Error: Determinar el código de estado apropiado

                // Si el jugador no existe → 404 Not Found
                if (message.Contains("no existe"))
                {
                    // NotFound(): Crea una respuesta 404
                    return NotFound(new { message });
                }

                // Si ya tiene un juego activo → 400 Bad Request (error de lógica)
                if (message.Contains("juego activo"))
                {
                    return BadRequest(new { message });
                }

                // Otros errores → 500 Internal Server Error
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message });
            }
        }

        /// <summary>
        /// Endpoint para realizar un intento de adivinanza.
        /// </summary>
        /// <param name="request">GameId y número intentado</param>
        /// <returns>ActionResult con el resultado del intento</returns>
        /// <remarks>
        /// RUTA: POST /api/game/v1/guess
        /// 
        /// REQUEST BODY (JSON):
        /// {
        ///   "gameid": 1,
        ///   "attemptedNumber": 1234
        /// }
        /// 
        /// RESPONSE EXITOSO (200):
        /// {
        ///   "gameid": 1,
        ///   "attemptedNumber": 1234,
        ///   "message": "Tu número tiene 2 fama y 1 pica"
        /// }
        /// 
        /// RESPONSE ERROR (404):
        /// {
        ///   "message": "El juego con ID 1 no existe"
        /// }
        /// 
        /// RESPONSE ERROR (400):
        /// {
        ///   "message": "El juego 1 ya ha finalizado"
        /// }
        /// </remarks>
        [HttpPost("guess")]
        [ProducesResponseType(typeof(GuessNumberResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GuessNumber([FromBody] GuessNumberRequest request)
        {
            _logger.LogInformation(
                "Intento de adivinanza para GameId: {GameId}",
                request.GameId
            );

            // Llamar al servicio
            var (success, message, response) = await _gameService.GuessNumberAsync(request);

            if (success)
            {
                // Éxito: Devolver 200 OK con el resultado
                return Ok(response);
            }
            else
            {
                // Error: Determinar el código apropiado

                // Si el juego no existe → 404 Not Found
                if (message.Contains("no existe"))
                {
                    return NotFound(new { message });
                }

                // Si el juego ya finalizó o hay error de validación → 400 Bad Request
                if (message.Contains("finalizado") ||
                    message.Contains("dígitos") ||
                    message.Contains("repetidos"))
                {
                    return BadRequest(new { message });
                }

                // Otros errores → 500 Internal Server Error
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message });
            }
        }
    }
}