using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NumberGuessGameApi.Models
{
    /// <summary>
    /// Representa un jugador en el sistema.
    /// Esta entidad almacena la información básica del usuario que se registra para jugar.
    /// </summary>
    /// <remarks>
    /// Un jugador puede tener múltiples juegos asociados a través de la relación uno-a-muchos.
    /// La unicidad se garantiza mediante la combinación de FirstName y LastName.
    /// </remarks>
    public class Player
    {
        /// <summary>
        /// Identificador único del jugador en la base de datos.
        /// </summary>
        /// <remarks>
        /// [Key] - Indica que esta propiedad es la clave primaria de la tabla.
        /// [DatabaseGenerated(DatabaseGeneratedOption.Identity)] - Especifica que el valor
        /// será generado automáticamente por la base de datos en forma incremental.
        /// </remarks>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PlayerId { get; set; }

        /// <summary>
        /// Nombre del jugador.
        /// </summary>
        /// <remarks>
        /// [Required] - Indica que este campo es obligatorio y no puede ser nulo.
        /// [StringLength(100)] - Limita la longitud máxima del nombre a 100 caracteres
        /// para optimizar el almacenamiento en la base de datos.
        /// </remarks>
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        /// <summary>
        /// Apellido del jugador.
        /// </summary>
        /// <remarks>
        /// Similar a FirstName, es un campo requerido con longitud máxima de 100 caracteres.
        /// Junto con FirstName, se utiliza para identificar usuarios únicos.
        /// </remarks>
        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        /// <summary>
        /// Edad del jugador.
        /// </summary>
        /// <remarks>
        /// [Required] - Campo obligatorio.
        /// Se almacena como entero para facilitar validaciones y consultas por rango de edad.
        /// </remarks>
        [Required]
        public int Age { get; set; }

        /// <summary>
        /// Fecha y hora en que el jugador se registró en el sistema.
        /// </summary>
        /// <remarks>
        /// Se utiliza DateTime para almacenar tanto la fecha como la hora exacta del registro.
        /// Este campo es útil para auditoría y para generar métricas de registro por día.
        /// </remarks>
        public DateTime RegistrationDate { get; set; }

        /// <summary>
        /// Colección de juegos asociados a este jugador.
        /// </summary>
        /// <remarks>
        /// Esta es una propiedad de navegación que establece la relación uno-a-muchos
        /// entre Player y Game. Entity Framework utiliza esta propiedad para:
        /// 1. Crear la foreign key en la tabla Game
        /// 2. Permitir la navegación desde un Player a sus Games
        /// 3. Facilitar consultas LINQ que incluyan juegos relacionados
        /// 
        /// Al inicializarla como new List<Game>(), evitamos excepciones de referencia nula
        /// cuando intentamos acceder a Games antes de que se cargue desde la base de datos.
        /// </remarks>
        public virtual ICollection<Game> Games { get; set; } = new List<Game>();
    }
}