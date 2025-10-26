using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NumberGuessGameApi.Models
{
    /// <summary>
    /// Representa una partida individual del juego Picas y Famas.
    /// </summary>
    /// <remarks>
    /// Cada juego está asociado a un único jugador y puede tener múltiples intentos.
    /// El juego mantiene el estado de si está activo o finalizado, y almacena
    /// el número secreto que el jugador debe adivinar.
    /// </remarks>
    public class Game
    {
        /// <summary>
        /// Identificador único del juego.
        /// </summary>
        /// <remarks>
        /// [Key] - Define esta propiedad como clave primaria.
        /// [DatabaseGenerated(DatabaseGeneratedOption.Identity)] - El valor se genera
        /// automáticamente de forma incremental por la base de datos al insertar un nuevo registro.
        /// </remarks>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GameId { get; set; }

        /// <summary>
        /// Identificador del jugador que está jugando esta partida.
        /// </summary>
        /// <remarks>
        /// [Required] - Campo obligatorio que no puede ser nulo.
        /// Este campo actúa como foreign key hacia la tabla Player.
        /// Entity Framework reconoce automáticamente la convención de nombres
        /// (PlayerId) y crea la relación con la tabla Player.
        /// </remarks>
        [Required]
        public int PlayerId { get; set; }

        /// <summary>
        /// Número secreto de 4 dígitos que el jugador debe adivinar.
        /// </summary>
        /// <remarks>
        /// [Required] - Campo obligatorio.
        /// [StringLength(4)] - Limita la longitud exacta a 4 caracteres.
        /// 
        /// Se almacena como string en lugar de int por dos razones:
        /// 1. Facilita la validación de que tenga exactamente 4 dígitos
        /// 2. Preserva números que comienzan con 0 (ejemplo: "0123")
        /// 3. Simplifica la comparación dígito por dígito para calcular Picas y Famas
        /// </remarks>
        [Required]
        [StringLength(4)]
        public string SecretNumber { get; set; }

        /// <summary>
        /// Fecha y hora en que se creó el juego.
        /// </summary>
        /// <remarks>
        /// Almacena el momento exacto en que se inició la partida.
        /// Útil para:
        /// - Auditoría del sistema
        /// - Cálculo de duración de partidas
        /// - Ordenamiento cronológico de juegos
        /// </remarks>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Indica si el juego ha finalizado.
        /// </summary>
        /// <remarks>
        /// false - El juego está activo y el jugador puede seguir intentando
        /// true - El juego terminó (el jugador adivinó el número)
        /// 
        /// Este flag se usa para:
        /// 1. Prevenir que un jugador haga intentos en un juego ya terminado
        /// 2. Evitar que se cree un nuevo juego si hay uno activo
        /// 3. Filtrar juegos para estadísticas (solo juegos finalizados)
        /// </remarks>
        [Required]
        public bool IsFinished { get; set; }

        /// <summary>
        /// Referencia al jugador asociado a este juego.
        /// </summary>
        /// <remarks>
        /// [ForeignKey("PlayerId")] - Especifica explícitamente que PlayerId es la
        /// foreign key que relaciona esta entidad con Player.
        /// 
        /// virtual - Habilita "lazy loading", permitiendo que Entity Framework
        /// cargue automáticamente los datos del Player cuando se accede a esta propiedad.
        /// 
        /// Esta es una propiedad de navegación que permite:
        /// - Acceder directamente al objeto Player desde un Game
        /// - Realizar consultas LINQ que incluyan datos del jugador
        /// Ejemplo: var playerName = game.Player.FirstName;
        /// </remarks>
        [ForeignKey("PlayerId")]
        public virtual Player Player { get; set; }

        /// <summary>
        /// Colección de todos los intentos realizados en este juego.
        /// </summary>
        /// <remarks>
        /// Propiedad de navegación que establece la relación uno-a-muchos entre Game y Attempt.
        /// 
        /// virtual - Permite lazy loading de los intentos.
        /// ICollection<Attempt> - Interfaz que permite agregar, eliminar y consultar intentos.
        /// = new List<Attempt>() - Inicialización para evitar NullReferenceException.
        /// 
        /// Se utiliza para:
        /// 1. Almacenar el historial completo de intentos del jugador
        /// 2. Calcular métricas (número de intentos hasta adivinar)
        /// 3. Mostrar el progreso del juego
        /// 4. Generar estadísticas para Power BI
        /// </remarks>
        public virtual ICollection<Attempt> Attempts { get; set; } = new List<Attempt>();
    }
}