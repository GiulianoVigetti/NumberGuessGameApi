using Microsoft.EntityFrameworkCore;
using NumberGuessGameApi.Data;
using NumberGuessGameApi.Services;

namespace NumberGuessGameApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // =================================================================
            // CONFIGURACIÓN DE LOGGING MEJORADO PARA AUDITORÍA
            // =================================================================

            // Limpiar providers predeterminados para tener control total
            builder.Logging.ClearProviders();

            // ⭐ LOGGING A CONSOLA (se ve en Visual Studio)
            // Incluye timestamps, niveles y contexto completo
            builder.Logging.AddConsole();

            // ⭐ LOGGING A DEBUG (ventana de depuración)
            builder.Logging.AddDebug();

            // ⭐ LOGGING A EVENT LOG DE WINDOWS (opcional, para auditoría avanzada)
            // Descomentar si quieres logs en el visor de eventos de Windows:
            // builder.Logging.AddEventLog();

            // Configurar niveles de logging detallados
            builder.Logging.SetMinimumLevel(LogLevel.Information);

            // Filtros específicos para reducir ruido pero mantener lo importante
            builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Information);
            builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Infrastructure", LogLevel.Warning);
            builder.Logging.AddFilter("Microsoft.AspNetCore.Hosting", LogLevel.Information);
            builder.Logging.AddFilter("Microsoft.AspNetCore.Routing", LogLevel.Warning);

            // ⭐ OPCIONAL: Agregar Serilog para logs en archivos
            // Requiere: Install-Package Serilog.AspNetCore
            // Requiere: Install-Package Serilog.Sinks.File
            /*
            builder.Host.UseSerilog((context, configuration) =>
            {
                configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .Enrich.FromLogContext()
                    .WriteTo.Console()
                    .WriteTo.File(
                        "Logs/api-log-.txt",
                        rollingInterval: RollingInterval.Day,
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}"
                    );
            });
            */

            // =================================================================
            // CONFIGURACIÓN DE SERVICIOS
            // =================================================================

            builder.Services.AddControllers();

            // ⭐⭐⭐ CONFIGURACIÓN DE SQLITE EN LUGAR DE SQL SERVER ⭐⭐⭐
            builder.Services.AddDbContext<GameDbContext>(options =>
            {
                // UseSqlite en lugar de UseSqlServer
                // SQLite es más simple: no necesita servidor, solo un archivo
                options.UseSqlite(
                    builder.Configuration.GetConnectionString("DefaultConnection")
                );

                // ⭐ HABILITAR LOGGING DETALLADO DE ENTITY FRAMEWORK
                // Esto registra todas las queries SQL generadas (importante para auditoría)
                if (builder.Environment.IsDevelopment())
                {
                    // EnableSensitiveDataLogging: Incluye valores de parámetros en logs
                    // SOLO para desarrollo (no en producción por seguridad)
                    options.EnableSensitiveDataLogging();

                    // EnableDetailedErrors: Errores más descriptivos
                    options.EnableDetailedErrors();
                }

                // LogTo para logging personalizado de SQL queries
                options.LogTo(
                    message => System.Diagnostics.Debug.WriteLine(message),
                    new[] { DbLoggerCategory.Database.Command.Name },
                    LogLevel.Information
                );
            });

            builder.Services.AddScoped<IGameService, GameService>();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                // ⭐ MEJORAR DOCUMENTACIÓN DE SWAGGER
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Number Guess Game API - Picas y Famas",
                    Version = "v1",
                    Description = "API RESTful para jugar Picas y Famas con logging completo y métricas para Power BI",
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact
                    {
                        Name = "Proyecto Programación Lógica",
                        Email = "contacto@ejemplo.com"
                    }
                });
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            var app = builder.Build();

            // =================================================================
            // LOGGING DE INICIO DE APLICACIÓN
            // =================================================================
            var logger = app.Services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("========================================");
            logger.LogInformation("Iniciando NumberGuessGameApi");
            logger.LogInformation("Entorno: {Environment}", app.Environment.EnvironmentName);
            logger.LogInformation("Base de datos: SQLite");
            logger.LogInformation("========================================");

            // =================================================================
            // CONFIGURACIÓN DEL PIPELINE
            // =================================================================

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Number Guess Game API v1");
                    options.RoutePrefix = "swagger";
                });

                logger.LogInformation("Swagger habilitado en: /swagger");
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowAll");
            app.UseAuthorization();
            app.MapControllers();

            // =================================================================
            // MIGRACIÓN AUTOMÁTICA DE BASE DE DATOS CON LOGGING
            // =================================================================
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    logger.LogInformation("Verificando estado de la base de datos...");

                    var context = services.GetRequiredService<GameDbContext>();

                    // Verificar si hay migraciones pendientes
                    var pendingMigrations = context.Database.GetPendingMigrations().ToList();

                    if (pendingMigrations.Any())
                    {
                        logger.LogInformation(
                            "Aplicando {Count} migraciones pendientes: {Migrations}",
                            pendingMigrations.Count,
                            string.Join(", ", pendingMigrations)
                        );
                    }

                    // Aplicar migraciones
                    context.Database.Migrate();

                    // Verificar conexión
                    var canConnect = context.Database.CanConnect();

                    if (canConnect)
                    {
                        logger.LogInformation("✓ Base de datos SQLite conectada exitosamente");
                        logger.LogInformation("  Ubicación: {Path}",
                            Path.Combine(Directory.GetCurrentDirectory(), "NumberGuessGame.db"));

                        // Estadísticas iniciales
                        var playerCount = context.Players.Count();
                        var gameCount = context.Games.Count();
                        var attemptCount = context.Attempts.Count();

                        logger.LogInformation("  Jugadores registrados: {PlayerCount}", playerCount);
                        logger.LogInformation("  Juegos creados: {GameCount}", gameCount);
                        logger.LogInformation("  Intentos realizados: {AttemptCount}", attemptCount);
                    }
                    else
                    {
                        logger.LogError("✗ No se pudo conectar a la base de datos");
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "✗ Error al configurar la base de datos");
                    throw;
                }
            }

            logger.LogInformation("========================================");
            logger.LogInformation("Aplicación iniciada correctamente");
            logger.LogInformation("Listening on: https://localhost:XXXX");
            logger.LogInformation("========================================");

            // =================================================================
            // INICIAR LA APLICACIÓN
            // =================================================================
            app.Run();
        }
    }
}

/*
 * =================================================================
 * CAMBIOS REALIZADOS PARA SQLITE
 * =================================================================
 * 
 * 1. ✅ UseSqlite() en lugar de UseSqlServer()
 * 2. ✅ No requiere puerto ni credenciales
 * 3. ✅ Base de datos en archivo: NumberGuessGame.db
 * 4. ✅ Se crea automáticamente si no existe
 * 5. ✅ Portable: puedes copiar el archivo .db
 * 
 * =================================================================
 * MEJORAS DE LOGGING PARA AUDITORÍA
 * =================================================================
 * 
 * 1. ✅ Logging detallado de Entity Framework (queries SQL)
 * 2. ✅ Logging de inicio de aplicación con estadísticas
 * 3. ✅ Logging de migraciones aplicadas
 * 4. ✅ Logging de conexión a base de datos
 * 5. ✅ EnableSensitiveDataLogging para debugging
 * 6. ✅ Filtros configurados para información relevante
 * 7. ✅ Preparado para Serilog (logs en archivos)
 * 
 * =================================================================
 * LOGS QUE VERÁS AL INICIAR
 * =================================================================
 * 
 * ========================================
 * Iniciando NumberGuessGameApi
 * Entorno: Development
 * Base de datos: SQLite
 * ========================================
 * Verificando estado de la base de datos...
 * ✓ Base de datos SQLite conectada exitosamente
 *   Ubicación: C:\...\NumberGuessGame.db
 *   Jugadores registrados: 0
 *   Juegos creados: 0
 *   Intentos realizados: 0
 * ========================================
 * Aplicación iniciada correctamente
 * ========================================
 * 
 * =================================================================
 * DIFERENCIAS SQLITE VS SQL SERVER
 * =================================================================
 * 
 * SQLITE:
 * - Archivo único: NumberGuessGame.db
 * - No necesita servidor
 * - Más simple de configurar
 * - Perfecto para desarrollo/demos
 * - Soporta hasta ~140TB de datos
 * - Transacciones ACID completas
 * 
 * SQL SERVER:
 * - Requiere servidor instalado
 * - Mejor para producción enterprise
 * - Más opciones de escalabilidad
 * - Requiere más configuración
 */