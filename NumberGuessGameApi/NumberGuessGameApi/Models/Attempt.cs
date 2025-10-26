using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NumberGuessGameApi.Models
{
    /// <summary>
    /// Representa un intento individual de adivinar el número secreto dentro de un juego.
    /// </summary>
    /// <remarks>
    /// Cada intento registra:
    /// - El número que el jugador intentó
    /// - El momento en que se realizó el intento
    /// - La respuesta del sistema (mensaje con Picas y Famas)
    /// 
    /// Esta entidad es crucial para:
    /// 1. Auditoría completa del juego
    /// 2. Análisis de estrategias de los jugadores
    /// 3. Generación de métricas en Power BI (cantidad de intentos por juego)
    /// </remarks>
    public class Attempt
    {
        /// <summary>
        /// Identificador único del intento.
        /// </summary>
        /// <remarks>
        /// [Key] - Marca esta propiedad como la clave primaria de la tabla Attempts.
        /// [DatabaseGenerated(DatabaseGeneratedOption.Identity)] - Indica que la base de datos
        /// generará automáticamente un valor único incremental para cada nuevo intento.
        /// 
        /// Esto garantiza que cada intento tenga un identificador único en toda la tabla,
        /// independientemente del juego al que pertenezca.
        /// </remarks>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AttemptId { get; set; }

        /// <summary>
        /// Identificador del juego al que pertenece este intento.
        /// </summary>
        /// <remarks>
        /// [Required] - Campo obligatorio que no puede ser nulo.
        /// 
        /// Esta es la foreign key que relaciona cada intento con su juego correspondiente.
        /// Permite:
        /// 1. Agrupar todos los intentos de una partida específica
        /// 2. Calcular el número total de intentos por juego
        /// 3. Mantener la integridad referencial (no pueden existir intentos huérfanos)
        /// 
        /// Entity Framework reconoce automáticamente esta propiedad como foreign key
        /// por la convención de nombres (GameId coincide con la clave primaria de Game).
        /// </remarks>
        [Required]
        public int GameId { get; set; }

        /// <summary>
        /// Número de 4 dígitos que el jugador intentó como adivinanza.
        /// </summary>
        /// <remarks>
        /// [Required] - Campo obligatorio.
        /// [StringLength(4)] - Valida que el número tenga exactamente 4 caracteres.
        /// 
        /// Se almacena como string en lugar de entero porque:
        /// 1. Preserva el formato exacto que el usuario envió (incluido el 0 inicial si existe)
        /// 2. Facilita la validación de longitud exacta
        /// 3. Simplifica la comparación dígito por dígito con el número secreto
        /// 
        /// Ejemplo: "0123" se preserva correctamente como string, 
        /// mientras que como int se convertiría a 123.
        /// </remarks>
        [Required]
        [StringLength(4)]
        public string AttemptedNumber { get; set; }

        /// <summary>
        /// Mensaje de respuesta generado por el sistema para este intento.
        /// </summary>
        /// <remarks>
        /// [Required] - Campo obligatorio.
        /// 
        /// Este mensaje es generado por la librería ESCMB.GuessCore y contiene
        /// información sobre Picas y Famas, o el mensaje de éxito si adivinó.
        /// 
        /// Ejemplos de mensajes:
        /// - "Tu número tiene 1 fama y 2 pica"
        /// - "Tu número tiene 3 fama 0 pica"
        /// - "¡Felicidades! Has adivinado el número."
        /// 
        /// Se almacena para:
        /// 1. Mostrar el historial completo de intentos al usuario
        /// 2. Auditoría del sistema
        /// 3. Depuración y análisis de comportamiento
        /// </remarks>
        [Required]
        public string Message { get; set; }

        /// <summary>
        /// Fecha y hora exacta en que se realizó el intento.
        /// </summary>
        /// <remarks>
        /// Registra el timestamp preciso del intento.
        /// 
        /// Se utiliza para:
        /// 1. Ordenar cronológicamente los intentos de un juego
        /// 2. Calcular el tiempo que le tomó al jugador adivinar
        /// 3. Analizar patrones de tiempo entre intentos
        /// 4. Auditoría y trazabilidad del sistema
        /// 
        /// El valor se asigna típicamente con DateTime.Now o DateTime.UtcNow
        /// en el momento de registrar el intento.
        /// </remarks>
        public DateTime AttemptedAt { get; set; }

        /// <summary>
        /// Referencia de navegación al juego al que pertenece este intento.
        /// </summary>
        /// <remarks>
        /// [ForeignKey("GameId")] - Especifica explícitamente que la propiedad GameId
        /// es la foreign key que relaciona esta entidad con Game.
        /// 
        /// virtual - Permite "lazy loading", de manera que Entity Framework puede
        /// cargar automáticamente los datos del Game asociado cuando se accede
        /// a esta propiedad.
        /// 
        /// Esta propiedad de navegación permite:
        /// 1. Acceder directamente al juego desde un intento
        ///    Ejemplo: var secretNumber = attempt.Game.SecretNumber;
        /// 2. Realizar consultas LINQ que incluyan datos del juego
        ///    Ejemplo: var attempts = dbContext.Attempts.Include(a => a.Game).ToList();
        /// 3. Mantener la cohesión del modelo de datos
        /// 
        /// Nota: Si no se carga explícitamente (con Include o Load), 
        /// esta propiedad será null hasta que se acceda por primera vez,
        /// momento en el cual EF la cargará automáticamente si lazy loading está habilitado.
        /// </remarks>
        [ForeignKey("GameId")]
        public virtual Game Game { get; set; }
    }
}