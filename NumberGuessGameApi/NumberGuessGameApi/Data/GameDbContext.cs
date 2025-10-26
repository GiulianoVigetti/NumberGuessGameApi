using Microsoft.EntityFrameworkCore;
using NumberGuessGameApi.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace NumberGuessGameApi.Data
{
    /// <summary>
    /// Contexto de base de datos para la aplicación del juego Picas y Famas.
    /// </summary>
    /// <remarks>
    /// Esta clase hereda de DbContext, que es la clase base de Entity Framework Core
    /// para trabajar con bases de datos. El DbContext:
    /// 
    /// 1. Representa una sesión con la base de datos
    /// 2. Permite consultar y guardar instancias de las entidades
    /// 3. Configura el modelo de datos y las relaciones
    /// 4. Gestiona el tracking de cambios en las entidades
    /// 5. Maneja las transacciones de base de datos
    /// 
    /// GameDbContext es la puerta de entrada para todas las operaciones
    /// de persistencia de datos en la aplicación.
    /// </remarks>
    public class GameDbContext : DbContext
    {
        /// <summary>
        /// Constructor que recibe las opciones de configuración del contexto.
        /// </summary>
        /// <param name="options">
        /// Opciones de configuración que incluyen:
        /// - La cadena de conexión a la base de datos
        /// - El proveedor de base de datos (SQL Server, PostgreSQL, SQLite, etc.)
        /// - Configuraciones de comportamiento (logging, lazy loading, etc.)
        /// </param>
        /// <remarks>
        /// El parámetro options se inyecta mediante Dependency Injection.
        /// Este patrón permite:
        /// 1. Configurar el contexto desde Program.cs o Startup.cs
        /// 2. Facilitar el testing con bases de datos en memoria
        /// 3. Cambiar fácilmente entre diferentes proveedores de BD
        /// 
        /// La llamada a base(options) pasa las opciones a la clase DbContext padre,
        /// permitiendo que Entity Framework inicialice correctamente el contexto.
        /// </remarks>
        public GameDbContext(DbContextOptions<GameDbContext> options) : base(options)
        {
            // El constructor simplemente pasa las opciones al constructor base (DbContext).
            // No necesita lógica adicional porque toda la configuración se hace
            // en Program.cs y en el método OnModelCreating.
        }

        /// <summary>
        /// DbSet que representa la tabla de Jugadores en la base de datos.
        /// </summary>
        /// <remarks>
        /// DbSet<Player> es una colección que permite:
        /// 1. Consultar jugadores: var players = context.Players.Where(p => p.Age > 18).ToList();
        /// 2. Agregar jugadores: context.Players.Add(newPlayer);
        /// 3. Actualizar jugadores: context.Players.Update(existingPlayer);
        /// 4. Eliminar jugadores: context.Players.Remove(playerToDelete);
        /// 
        /// Entity Framework mapea este DbSet a una tabla llamada "Players" en la base de datos.
        /// Cada operación en este DbSet se traduce a una consulta SQL correspondiente.
        /// </remarks>
        public DbSet<Player> Players { get; set; }

        /// <summary>
        /// DbSet que representa la tabla de Juegos en la base de datos.
        /// </summary>
        /// <remarks>
        /// Similar a Players, este DbSet permite todas las operaciones CRUD sobre juegos.
        /// 
        /// Operaciones típicas:
        /// - Obtener juegos activos: context.Games.Where(g => !g.IsFinished).ToList();
        /// - Obtener juegos de un jugador: context.Games.Where(g => g.PlayerId == id).ToList();
        /// - Crear nuevo juego: context.Games.Add(newGame);
        /// - Finalizar juego: game.IsFinished = true; context.SaveChanges();
        /// 
        /// La tabla en la base de datos se llamará "Games".
        /// </remarks>
        public DbSet<Game> Games { get; set; }

        /// <summary>
        /// DbSet que representa la tabla de Intentos en la base de datos.
        /// </summary>
        /// <remarks>
        /// Este DbSet gestiona todos los intentos de adivinanza realizados en todos los juegos.
        /// 
        /// Consultas comunes:
        /// - Intentos de un juego: context.Attempts.Where(a => a.GameId == gameId).ToList();
        /// - Contar intentos: context.Attempts.Count(a => a.GameId == gameId);
        /// - Último intento: context.Attempts.Where(a => a.GameId == gameId)
        ///                                    .OrderByDescending(a => a.AttemptedAt)
        ///                                    .FirstOrDefault();
        /// 
        /// La tabla en la base de datos se llamará "Attempts".
        /// </remarks>
        public DbSet<Attempt> Attempts { get; set; }

        /// <summary>
        /// Configura el modelo de datos cuando se crea la base de datos.
        /// </summary>
        /// <param name="modelBuilder">
        /// Constructor de modelos que permite configurar:
        /// - Relaciones entre entidades
        /// - Restricciones de unicidad
        /// - Índices para optimización
        /// - Nombres de tablas personalizados
        /// - Configuraciones de columnas
        /// </param>
        /// <remarks>
        /// Este método se llama automáticamente por Entity Framework cuando se inicializa
        /// el contexto por primera vez. Permite configuración fluida del modelo mediante
        /// el patrón Fluent API.
        /// 
        /// Se usa en lugar de atributos cuando:
        /// 1. La configuración es compleja
        /// 2. Se necesita configurar relaciones específicas
        /// 3. Se quiere separar la configuración del modelo de las clases de entidad
        /// </remarks>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Llama al método base para asegurar que se ejecute cualquier configuración
            // predeterminada de Entity Framework.
            base.OnModelCreating(modelBuilder);

            // Configuración de la entidad Player
            // modelBuilder.Entity<Player>() - Obtiene el constructor para configurar la entidad Player
            modelBuilder.Entity<Player>()
                // .HasIndex(p => new { p.FirstName, p.LastName }) - Crea un índice compuesto
                // en las columnas FirstName y LastName. Un índice:
                // 1. Acelera las búsquedas por estos campos
                // 2. Permite búsquedas eficientes de jugadores existentes
                .HasIndex(p => new { p.FirstName, p.LastName })
                // .IsUnique() - Hace que el índice sea único, garantizando que no puedan
                // existir dos jugadores con el mismo nombre y apellido.
                // Esto previene duplicados en el registro.
                .IsUnique();

            // Configuración de la relación Player -> Games (uno a muchos)
            // Un jugador puede tener múltiples juegos, pero cada juego pertenece a un solo jugador.
            modelBuilder.Entity<Player>()
                // .HasMany(p => p.Games) - Especifica que un Player tiene muchos Games
                // (navegación desde la colección Games en la entidad Player)
                .HasMany(p => p.Games)
                // .WithOne(g => g.Player) - Especifica que cada Game tiene un solo Player
                // (navegación desde la propiedad Player en la entidad Game)
                .WithOne(g => g.Player)
                // .HasForeignKey(g => g.PlayerId) - Define explícitamente que PlayerId
                // en la tabla Games es la foreign key que apunta a PlayerId en Players
                .HasForeignKey(g => g.PlayerId)
                // .OnDelete(DeleteBehavior.Cascade) - Configura el comportamiento al eliminar:
                // Cuando se elimina un Player, automáticamente se eliminan todos sus Games.
                // Esto mantiene la integridad referencial de la base de datos.
                .OnDelete(DeleteBehavior.Cascade);

            // Configuración de la relación Game -> Attempts (uno a muchos)
            // Un juego puede tener múltiples intentos, pero cada intento pertenece a un solo juego.
            modelBuilder.Entity<Game>()
                // .HasMany(g => g.Attempts) - Un Game tiene muchos Attempts
                .HasMany(g => g.Attempts)
                // .WithOne(a => a.Game) - Cada Attempt pertenece a un solo Game
                .WithOne(a => a.Game)
                // .HasForeignKey(a => a.GameId) - GameId en Attempts es la foreign key
                .HasForeignKey(a => a.GameId)
                // .OnDelete(DeleteBehavior.Cascade) - Al eliminar un Game,
                // se eliminan automáticamente todos sus Attempts.
                // Esto asegura que no queden intentos huérfanos en la base de datos.
                .OnDelete(DeleteBehavior.Cascade);

            // Nota: Entity Framework puede inferir muchas de estas relaciones automáticamente
            // por las convenciones de nombres, pero las especificamos explícitamente para:
            // 1. Mayor claridad y documentación del modelo
            // 2. Control explícito sobre el comportamiento de eliminación
            // 3. Evitar ambigüedades en relaciones complejas
        }
    }
}