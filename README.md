Como Usar: 
 Proyecto de Juego desde CERO
 Tabla de Contenidos
 Instalar Programas Necesarios
 Configurar SQLite
 Crear el Proyecto en Visual Studio
 Instalar Librerías
 
 PASO 1: Instalar Programas Necesarios
 1.1. Descargar e Instalar Visual Studio 2022
 Abre tu navegador (Chrome, Edge, Firefox)
 Ve a esta página: https://visualstudio.microsoft.com/es/downloads/
 Descarga la versión gratuita:
 Busca el botón que dice "Community 2022"
 Click en "Descarga gratuita"
 Espera a que se descargue (archivo grande, ~3GB)
 Ejecuta el instalador:
 Doble click en el archivo descargado
 Si Windows pregunta "¿Desea permitir que esta aplicación haga cambios?" → Click en "Sí"
 Selecciona qué instalar:
 Se abrirá el "Instalador de Visual Studio"
 Marca la casilla: "Desarrollo de ASP.NET y web"
 Marca la casilla: "Desarrollo de escritorio de .NET"
 Click en "Instalar" (abajo a la derecha)
 Espera 20-40 minutos (va a descargar e instalar muchas cosas)
 Cuando termine:
 Click en "Iniciar"
 Visual Studio se abrirá
 Si pide cuenta Microsoft, puedes saltarlo (arriba dice "Omitir esto por ahora")
 Abrir la Consola de Paquetes
 En Visual Studio, ve al menú superior
 Click en: Herramientas
 Luego: Administrador de paquetes NuGet
 Luego: Consola del Administrador de paquetes
 Se abrirá una ventana abajo que dice "PM>"
Instalar Paquetes UNO POR UNO

 IMPORTANTE: Copia y pega CADA comando, presiona Enter, y ESPERA a que termine antes 
de poner el siguiente.
 
 powershellInstall-Package Microsoft.EntityFrameworkCore.SqlServer
 Espera... verás texto desfilando... cuando termine dirá "Successfully installed"
 
 powershellInstall-Package Microsoft.EntityFrameworkCore.Design
 Espera...
 
 powershellInstall-Package Microsoft.EntityFrameworkCore.Tools
 Espera...
 powershellInstall-Package ESCMB.GameCore -Version 1.0.0
 
 Espera...
 **Cuando todos terminen**, verás mensajes como:
 Successfully installed 'ESCMB.GameCore 1.0.0'
 Verificar Instalación
 En el Explorador de soluciones
 Busca un nodo que dice "Dependencias"
 Click en la flechita para expandirlo
 Click en "Paquetes"
 Deberías ver:
 Microsoft.EntityFrameworkCore.SqlServer
 Microsoft.EntityFrameworkCore.Design
 Microsoft.EntityFrameworkCore.Tools
 ESCMB.GameCore
