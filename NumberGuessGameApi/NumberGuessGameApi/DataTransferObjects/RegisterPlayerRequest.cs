using System.ComponentModel.DataAnnotations;

namespace NumberGuessGameApi.DataTransferObjects
{
    /// <summary>
    /// DTO (Data Transfer Object) para la solicitud de registro de un nuevo jugador.
    /// </summary>
    /// <remarks>
    /// Este DTO define el contrato de datos que el cliente debe enviar para registrarse.
    /// 
    /// ¿Por qué usar DTOs en lugar de las entidades directamente?
    /// 
    /// 1. SEGURIDAD: Evita que el cliente pueda enviar campos que no debería modificar
    ///    (como PlayerId o RegistrationDate).
    /// 
    /// 2. DESACOPLAMIENTO: Separa la estructura de la API de la estructura de la base de datos.
    ///    Si cambias el modelo Player, no afecta necesariamente la API.
    /// 
    /// 3. VALIDACIÓN: Permite validaciones específicas para la entrada del usuario,
    ///    diferentes de las validaciones del modelo de datos.
    /// 
    /// 4. CLARIDAD: Define explícitamente qué datos necesita cada endpoint.
    /// 
    /// 5. DOCUMENTACIÓN: Herramientas como Swagger generan automáticamente
    ///    documentación basada en estos DTOs.
    /// </remarks>
    public class RegisterPlayerRequest
    {
        /// <summary>
        /// Nombre del jugador que se está registrando.
        /// </summary>
        /// <remarks>
        /// [Required(ErrorMessage = "...")] - Data Annotation que valida automáticamente
        /// que este campo no sea nulo o vacío. Si la validación falla:
        /// 1. ASP.NET Core automáticamente rechaza la solicitud
        /// 2. Devuelve un BadRequest (400) con el mensaje de error personalizado
        /// 3. No llega a ejecutarse el código del controller
        /// 
        /// ErrorMessage personaliza el mensaje que se envía al cliente cuando falla.
        /// Esto mejora la experiencia del usuario al darle información clara.
        /// 
        /// Ejemplo de uso en JSON:
        /// { "firstname": "Juan", "lastname": "Pérez", "age": 25 }
        /// </remarks>
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string FirstName { get; set; }

        /// <summary>
        /// Apellido del jugador que se está registrando.
        /// </summary>
        /// <remarks>
        /// [Required] - Validación obligatoria similar a FirstName.
        /// 
        /// [StringLength(100)] - Limita la longitud máxima a 100 caracteres.
        /// Esta validación:
        /// 1. Previene ataques de denegación de servicio con strings muy largos
        /// 2. Asegura consistencia con el modelo de base de datos
        /// 3. Devuelve automáticamente BadRequest si se excede el límite
        /// 
        /// La combinación de Required + StringLength garantiza que el apellido:
        /// - No sea nulo
        /// - No esté vacío
        /// - No exceda 100 caracteres
        /// </remarks>
        [Required(ErrorMessage = "El apellido es requerido")]
        [StringLength(100, ErrorMessage = "El apellido no puede exceder 100 caracteres")]
        public string LastName { get; set; }

        /// <summary>
        /// Edad del jugador.
        /// </summary>
        /// <remarks>
        /// [Required] - Valida que se proporcione un valor para la edad.
        /// 
        /// [Range(1, 120)] - Valida que la edad esté entre 1 y 120 años.
        /// Esta validación de rango:
        /// 1. Previene valores negativos o cero (edades inválidas)
        /// 2. Previene valores irrazonablemente altos (120 es un límite razonable)
        /// 3. Rechaza automáticamente la solicitud si el valor está fuera del rango
        /// 
        /// El ErrorMessage personalizado informa al usuario exactamente qué se espera.
        /// 
        /// ASP.NET Core automáticamente convierte el JSON numérico a int,
        /// y valida el rango antes de que el código del controller se ejecute.
        /// 
        /// Ejemplo válido: "age": 25
        /// Ejemplo inválido: "age": -5 (devuelve BadRequest con el mensaje de error)
        /// </remarks>
        [Required(ErrorMessage = "La edad es requerida")]
        [Range(1, 120, ErrorMessage = "La edad debe estar entre 1 y 120 años")]
        public int Age { get; set; }
    }
}