<!-- Completa abajo cambiando ET12DE1Computacion a tu user|organización y template a tu repo, te recomiendo usar el Find & Replace de tu editor -->
![main build.NET6](https://github.com/ET12DE1Computacion/simpleTemplateCSharp/workflows/main-build.NET6/badge.svg?branch=main) ![main test.NET6](https://github.com/ET12DE1Computacion/simpleTemplateCSharp/workflows/main-test.NET6/badge.svg?branch=main)
![dev build.NET6](https://github.com/ET12DE1Computacion/simpleTemplateCSharp/workflows/dev-build.NET6/badge.svg?branch=dev) ![dev test.NET6](https://github.com/ET12DE1Computacion/simpleTemplateCSharp/workflows/dev-test.NET6/badge.svg?branch=dev)
[![Abrir en Visual Studio Code](https://img.shields.io/static/v1?logo=visualstudiocode&label=&message=Abrir%20en%20Visual%20Studio%20Code&labelColor=2c2c32&color=007acc&logoColor=007acc)](https://open.vscode.dev/ET12DE1Computacion/simpleTemplateCSharp)
<!-- Borra este comentario y linea después haber cambiado arriba las ocurrencias de tu usuario/repo -->

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

_Acá va un parrafo que describa lo que es el proyecto._

## Comenzando 🚀

Clonar el repositorio github, desde Github Desktop o ejecutar en la terminal o CMD:

```
https://github.com/JosuGuzman/BD-Spotify
```

### Pre-requisitos 📋

- .NET 8 (SDK .NET 8.0.105) - [Descargar](https://dotnet.microsoft.com/es-es/download/dotnet/8.0)

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

## Construido con 🛠️

_Menciona las herramientas y versiones que utilizaste para crear tu proyecto_

* [Visual Studio Code](https://code.visualstudio.com/#alt-downloads) - Editor de código.

## Autores ✒️

_Menciona a todos aquellos que ayudaron a levantar el proyecto desde sus inicios_


