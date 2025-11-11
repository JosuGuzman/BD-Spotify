<h1 align="center"> E.T. Nº12 D.E. 1º "Libertador Gral. José de San Martín" </h1>
<p align="center">
  <img src="https://et12.edu.ar/imgs/et12.gif">
</p>

## Computación : 2024

**Asignatura**: Base de Datos

**Nombre TP**: BD Spotify

**Apellido y Nombre Alumno**: Miguel Verdugues, Josu Duran, Rene Terraza

**Curso**: 5 ° 7

# BD Spotify

El proyecto BD Spotify es una base de datos diseñada para almacenar y gestionar información relacionada con la música y los usuarios de la plataforma Spotify. Esta base de datos tiene como objetivo proporcionar una estructura organizada para el almacenamiento de datos de canciones, álbumes, artistas, usuarios y sus preferencias musicales. El proyecto busca implementar una solución escalable y eficiente para el manejo de grandes cantidades de datos, permitiendo a los usuarios interactuar con la base de datos de manera sencilla y segura.

<h3 align="center"> Diagrama de Clases </h3>

````
classDiagram
    direction TB

    subgraph I. Usuarios y Suscripción
        class Artista {
            +INT UNSIGNED idArtista
            +VARCHAR(35) NombreArtistico
            +VARCHAR(45) Nombre
            +VARCHAR(45) Apellido
        }
        class Nacionalidad {
            +INT UNSIGNED idNacionalidad
            +VARCHAR(45) Pais
        }
        class Usuario {
            +INT UNSIGNED idUsuario
            +VARCHAR(45) NombreUsuario
            +VARCHAR(45) Email
            +VARCHAR(64) Contrasenia
            +INT UNSIGNED idNacionalidad
        }
        class TipoSuscripcion {
            +INT UNSIGNED idTipoSuscripcion
            +TINYINT UNSIGNED Duracion
            +TINYINT UNSIGNED Costo
            +VARCHAR(45) Tipo
        }
        class Suscripcion {
            +INT UNSIGNED idSuscripcion
            +INT UNSIGNED idUsuario
            +INT UNSIGNED idTipoSuscripcion
            +DATE FechaInicio
        }

        Usuario --> Nacionalidad : tiene
        Suscripcion --> Usuario : registra
        Suscripcion --> TipoSuscripcion : es de tipo
    end

    subgraph II. Contenido Musical
        class Genero {
            +TINYINT UNSIGNED idGenero
            +VARCHAR(45) Genero
            +TEXT Descripcion
        }
        class Album {
            +INT UNSIGNED idAlbum
            +VARCHAR(45) Titulo
            +DATE fechaLanzamiento
            +INT UNSIGNED idArtista
            +VARCHAR(255) Portada
        }
        class Cancion {
            +INT UNSIGNED idCancion
            +VARCHAR(45) Titulo
            +TIME Duracion
            +INT UNSIGNED idAlbum
            +INT UNSIGNED idArtista
            +TINYINT UNSIGNED idGenero
            +VARCHAR(255) ArchivoMP3
        }

        Album --> Artista : pertenece a
        Cancion --> Album : pertenece a
        Cancion --> Artista : interpretada por
        Cancion --> Genero : tiene
    end

    subgraph III. Playlists y Log
        class Playlist {
            +INT UNSIGNED idPlaylist
            +VARCHAR(20) Nombre
            +INT UNSIGNED idUsuario
        }
        class Cancion_Playlist {
            +INT UNSIGNED idCancion
            +INT UNSIGNED idPlaylist
        }
        class HistorialReproduccion {
            +INT UNSIGNED idHistorial
            +INT UNSIGNED idUsuario
            +INT UNSIGNED idCancion
            +DATETIME FechaReproduccion
        }

        Playlist --> Usuario : creada por
        Cancion_Playlist -- Cancion
        Cancion_Playlist -- Playlist
        HistorialReproduccion --> Usuario : registra
        HistorialReproduccion --> Cancion : reproduce
    end
````

## Comenzando 🚀

Clonar el repositorio github, desde Github Desktop o ejecutar en la terminal o CMD:

```
https://github.com/JosuGuzman/BD-Spotify
```

## Pre-requisitos 📋

- .NET 8 (SDK .NET 8.0.105) - [Descargar](https://dotnet.microsoft.com/es-es/download/dotnet/8.0)
- Visual Studio Code - [Descargar](https://code.visualstudio.com/#alt-downloads)
- Git - [Descargar](https://git-scm.com/downloads)
- MySQL - [Descargar](https://dev.mysql.com/downloads/mysql/)
- Dapper - Micro ORM para .NET
- Entity Framework Core - Para la gestión de datos en .NET

## Despliegue 📦

_Para iniciar el proyecto primero debe desplegar la base de datos y para eso tiene que hacer segundo click en la carpeta scripts sql_
_y presionar en terminal integrado, le aparecera una terminal donde tiene que poner lo siguiente:_

```
mysql -u tuUsuario -p 
:tuContraseña
```

_Luego dirigirse a la carpeta src y dentro de la carpeta Spotify.ReposDapper.Test_

1. Crear `appSettings.json`: nombre del archivo json que tiene que estar en la misma carpeta.
El contenido del archivo tiene que ser:  
  ```json
  {
  "ConnectionStrings": {
    "MySQL": "server=localhost;database=tuBD;user=tuUsuarioBD;password=tuPass"
  }
  }
  ```

Para desplegar el proyecto, sigue los siguientes pasos:


1. **Abrir el proyecto**:
   - Navega al directorio del proyecto clonado:
     ``` bash
     cd BD-Spotify
     ```
   - Abre el proyecto en Visual Studio Code ejecutando:
     ``` bash
     code .
     ```

2. **Instalar dependencias**:
   - Asegúrate de tener instalado el SDK de .NET 8.0.105. Si no lo tienes, descárgalo desde [aquí](https://dotnet.microsoft.com/es-es/download/dotnet/8.0).
   - En la terminal de Visual Studio Code, ejecuta el siguiente comando para restaurar las dependencias del proyecto:
     ``` bash
     dotnet restore
     ```

3. **Configurar la base de datos**:
   - Asegúrate de tener MySQL instalado y en funcionamiento.
   - Crea una base de datos llamada `5to_Spotify` en tu servidor MySQL.
   - Navega a la carpeta `scripts sql` dentro del proyecto:
     ``` bash
     cd scripts sql
     ```
   - Ejecuta el siguiente comando para importar los scripts SQL necesarios:
     ``` bash
     mysql -u UsuarioMySQL -p
     ```
   - Luego, ingresa la contraseña de tu usuario y ejecuta:
     ``` shell
     mysql >> SOURCE install.sql
     ```

4. **Ejecutar el proyecto**:
   - Regresa al directorio raíz del proyecto:
     ``` bash
     cd ..
     ```
   - Ejecuta el proyecto utilizando el siguiente comando:
     ```
     dotnet run
     ```

5. **Probar el proyecto**:
   - Para ejecutar las pruebas unitarias, utiliza el siguiente comando:
     ``` bash
     dotnet test
     ```

6. **Acceder a la aplicación**:
   - Una vez que el proyecto esté en ejecución, podrás acceder a la aplicación a través de tu navegador en la dirección que se indique en la terminal (generalmente `http://localhost:5000` o similar).

En la carpeta `src` encontrarás el código fuente del proyecto. En la carpeta `scripts sql` encontrarás los scripts SQL utilizados para la creación y manipulación de la base de datos.

## Construido con 🛠️

El proyecto fue construido utilizando las siguientes herramientas y versiones:

* .NET 8 (SDK .NET 8.0.105)
* Visual Studio Code
* SQL Server
* Dapper (versión 2.1.35)
* Entity Framework Core
* MySQL (versión 8.0 o superior)
* XUnit (para pruebas unitarias)


## Autores ✒️

* Josu Duran - Creador de la base de datos y parte de las clases de C#
* Rene Terraza - Encargado de la documentacion y parte de los tests
* Miguel Verdugues - Creador de la Logica del Proyecto y parte de las clases de C#