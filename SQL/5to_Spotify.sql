-- =====================================================
-- SCRIPT PARA CREAR LA BASE DE DATOS SPOTIFY
-- =====================================================
SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE;
SET SQL_MODE='STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION';
SET @OLD_TIME_ZONE=@@time_zone;
SET time_zone = '+00:00';

-- -----------------------------------------------------
-- Schema Spotify
-- -----------------------------------------------------
DROP DATABASE IF EXISTS `Sto_potify`;
CREATE DATABASE `5to_Spotify` CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE `5to_Spotify`;

-- =====================================================
-- TABLA: Rol
-- =====================================================
CREATE TABLE IF NOT EXISTS `Rol` (
  `IdRol` TINYINT UNSIGNED NOT NULL AUTO_INCREMENT,
  `Nombre` VARCHAR(30) NOT NULL,
  `Descripcion` VARCHAR(255) DEFAULT NULL,
  PRIMARY KEY (`IdRol`),
  UNIQUE KEY `UQ_Rol_Nombre` (`Nombre`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =====================================================
-- TABLA: Nacionalidad
-- =====================================================
CREATE TABLE IF NOT EXISTS `Nacionalidad` (
  `IdNacionalidad` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `Pais` VARCHAR(100) NOT NULL,
  PRIMARY KEY (`IdNacionalidad`),
  UNIQUE KEY `UQ_Nacionalidad_Pais` (`Pais`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =====================================================
-- TABLA: Artista
-- =====================================================
CREATE TABLE IF NOT EXISTS `Artista` (
  `IdArtista` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `NombreArtistico` VARCHAR(150) NOT NULL,
  `NombreReal` VARCHAR(100) DEFAULT NULL,
  `ApellidoReal` VARCHAR(100) DEFAULT NULL,
  `Biografia` TEXT DEFAULT NULL,
  `FotoArtista` VARCHAR(255) DEFAULT 'artista_default.png',
  `EstaActivo` TINYINT(1) NOT NULL DEFAULT 1,
  `FechaCreacion` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `FechaActualizacion` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `IdNacionalidad` INT UNSIGNED DEFAULT NULL,
  PRIMARY KEY (`IdArtista`),
  UNIQUE KEY `UQ_Artista_NombreArtistico` (`NombreArtistico`),
  INDEX `IX_Artista_Nombre` (`NombreArtistico`(50)),
  CONSTRAINT `FK_Artista_Nacionalidad` FOREIGN KEY (`IdNacionalidad`) REFERENCES `Nacionalidad`(`IdNacionalidad`)
    ON DELETE SET NULL ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =====================================================
-- TABLA: Genero
-- =====================================================
CREATE TABLE IF NOT EXISTS `Genero` (
  `IdGenero` TINYINT UNSIGNED NOT NULL AUTO_INCREMENT,
  `Nombre` VARCHAR(100) NOT NULL,
  `Descripcion` VARCHAR(255) DEFAULT NULL,
  `EstaActivo` TINYINT(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`IdGenero`),
  UNIQUE KEY `UQ_Genero_Nombre` (`Nombre`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =====================================================
-- TABLA: TipoSuscripcion
-- =====================================================
CREATE TABLE IF NOT EXISTS `TipoSuscripcion` (
  `IdTipoSuscripcion` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `Tipo` VARCHAR(100) NOT NULL,
  `DuracionMeses` INT UNSIGNED NOT NULL COMMENT 'Duración en meses',
  `Costo` DECIMAL(10,2) NOT NULL COMMENT 'Costo en la moneda local',
  `EstaActivo` TINYINT(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`IdTipoSuscripcion`),
  UNIQUE KEY `UQ_TipoSuscripcion_Tipo` (`Tipo`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =====================================================
-- TABLA: Usuario
-- =====================================================
CREATE TABLE IF NOT EXISTS `Usuario` (
  `IdUsuario` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `NombreUsuario` VARCHAR(60) NOT NULL,
  `Email` VARCHAR(150) NOT NULL,
  `Contrasenia` VARCHAR(255) NOT NULL COMMENT 'hash bcrypt',
  `IdNacionalidad` INT UNSIGNED NOT NULL,
  `IdRol` TINYINT UNSIGNED NOT NULL DEFAULT 2, -- 2=Usuario, 3=Administrador
  `FechaRegistro` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `FotoPerfil` VARCHAR(255) DEFAULT 'avatar_default.png',
  `EstaActivo` TINYINT(1) NOT NULL DEFAULT 1,
  `UltimoAcceso` DATETIME DEFAULT NULL,
  PRIMARY KEY (`IdUsuario`),
  UNIQUE KEY `UQ_Usuario_Email` (`Email`),
  INDEX `IX_Usuario_IdRol` (`IdRol`),
  INDEX `IX_Usuario_IdNacionalidad` (`IdNacionalidad`),
  INDEX `IX_Usuario_FechaRegistro` (`FechaRegistro`),
  CONSTRAINT `FK_Usuario_Rol` FOREIGN KEY (`IdRol`) REFERENCES `Rol`(`IdRol`)
    ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `FK_Usuario_Nacionalidad` FOREIGN KEY (`IdNacionalidad`) REFERENCES `Nacionalidad`(`IdNacionalidad`)
    ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =====================================================
-- TABLA: Album
-- =====================================================
CREATE TABLE IF NOT EXISTS `Album` (
  `IdAlbum` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `Titulo` VARCHAR(200) NOT NULL,
  `FechaLanzamiento` DATE DEFAULT NULL,
  `IdArtista` INT UNSIGNED NOT NULL,
  `Portada` VARCHAR(255) DEFAULT 'album_default.png',
  `FechaCreacion` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `FechaActualizacion` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `EstaActivo` TINYINT(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`IdAlbum`),
  INDEX `IX_Album_IdArtista` (`IdArtista`),
  INDEX `IX_Album_Titulo` (`Titulo`(100)),
  INDEX `IX_Album_FechaTitulo` (`FechaLanzamiento`, `Titulo`(100)),
  CONSTRAINT `FK_Album_Artista` FOREIGN KEY (`IdArtista`) REFERENCES `Artista`(`IdArtista`)
    ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =====================================================
-- TABLA: Cancion
-- =====================================================
CREATE TABLE IF NOT EXISTS `Cancion` (
  `IdCancion` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `Titulo` VARCHAR(200) NOT NULL,
  `DuracionSegundos` INT UNSIGNED NOT NULL COMMENT 'duración en segundos',
  `IdAlbum` INT UNSIGNED NOT NULL,
  `IdArtista` INT UNSIGNED NOT NULL,
  `IdGenero` TINYINT UNSIGNED NOT NULL,
  `ArchivoMP3` VARCHAR(500) NOT NULL,
  `ContadorReproducciones` BIGINT UNSIGNED NOT NULL DEFAULT 0,
  `EstaActiva` TINYINT(1) NOT NULL DEFAULT 1,
  `FechaCreacion` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `FechaActualizacion` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`IdCancion`),
  INDEX `IX_Cancion_IdAlbum` (`IdAlbum`),
  INDEX `IX_Cancion_IdArtista` (`IdArtista`),
  INDEX `IX_Cancion_IdGenero` (`IdGenero`),
  INDEX `IX_Cancion_Titulo_Contador` (`Titulo`(100), `ContadorReproducciones`),
  INDEX `IX_Cancion_Contador` (`ContadorReproducciones`),
  INDEX `IX_Cancion_ArtistaGenero` (`IdArtista`, `IdGenero`),
  FULLTEXT INDEX `FT_Cancion_Titulo` (`Titulo`),
  CONSTRAINT `FK_Cancion_Album` FOREIGN KEY (`IdAlbum`) REFERENCES `Album`(`IdAlbum`)
    ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `FK_Cancion_Artista` FOREIGN KEY (`IdArtista`) REFERENCES `Artista`(`IdArtista`)
    ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `FK_Cancion_Genero` FOREIGN KEY (`IdGenero`) REFERENCES `Genero`(`IdGenero`)
    ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =====================================================
-- TABLA: Playlist
-- =====================================================
CREATE TABLE IF NOT EXISTS `Playlist` (
  `IdPlaylist` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `IdUsuario` INT UNSIGNED NOT NULL,
  `Nombre` VARCHAR(200) NOT NULL,
  `Descripcion` TEXT DEFAULT NULL,
  `EsPublica` TINYINT(1) NOT NULL DEFAULT 1,
  `EsSistema` TINYINT(1) NOT NULL DEFAULT 0,
  `EstaActiva` TINYINT(1) NOT NULL DEFAULT 1,
  `FechaCreacion` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `FechaActualizacion` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`IdPlaylist`),
  INDEX `IX_Playlist_IdUsuario` (`IdUsuario`),
  INDEX `IX_Playlist_Nombre` (`Nombre`(100)),
  CONSTRAINT `FK_Playlist_Usuario` FOREIGN KEY (`IdUsuario`) REFERENCES `Usuario`(`IdUsuario`)
    ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =====================================================
-- TABLA: Cancion_Playlist (relación muchos-a-muchos)
-- =====================================================
CREATE TABLE IF NOT EXISTS `Cancion_Playlist` (
  `IdCancion` INT UNSIGNED NOT NULL,
  `IdPlaylist` INT UNSIGNED NOT NULL,
  `Orden` INT UNSIGNED NOT NULL DEFAULT 0,
  `FechaAgregado` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`IdCancion`,`IdPlaylist`),
  INDEX `IX_CP_IdCancion` (`IdCancion`),
  INDEX `IX_CP_IdPlaylist` (`IdPlaylist`),
  CONSTRAINT `FK_CP_Cancion` FOREIGN KEY (`IdCancion`) REFERENCES `Cancion`(`IdCancion`)
    ON DELETE CASCADE ON UPDATE NO ACTION,
  CONSTRAINT `FK_CP_Playlist` FOREIGN KEY (`IdPlaylist`) REFERENCES `Playlist`(`IdPlaylist`)
    ON DELETE CASCADE ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =====================================================
-- TABLA: HistorialReproduccion
-- =====================================================
CREATE TABLE IF NOT EXISTS `HistorialReproduccion` (
  `IdHistorial` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  `IdUsuario` INT UNSIGNED NOT NULL,
  `IdCancion` INT UNSIGNED NOT NULL,
  `FechaReproduccion` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `DuracionReproducida` INT UNSIGNED DEFAULT NULL COMMENT 'segundos reproducidos',
  PRIMARY KEY (`IdHistorial`),
  INDEX `IX_HR_IdUsuario` (`IdUsuario`),
  INDEX `IX_HR_IdCancion` (`IdCancion`),
  INDEX `IX_HR_Usuario_Fecha` (`IdUsuario`,`FechaReproduccion`),
  CONSTRAINT `FK_HR_Usuario` FOREIGN KEY (`IdUsuario`) REFERENCES `Usuario`(`IdUsuario`)
    ON DELETE CASCADE ON UPDATE NO ACTION,
  CONSTRAINT `FK_HR_Cancion` FOREIGN KEY (`IdCancion`) REFERENCES `Cancion`(`IdCancion`)
    ON DELETE CASCADE ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =====================================================
-- TABLA: Suscripcion
-- =====================================================
CREATE TABLE IF NOT EXISTS `Suscripcion` (
  `IdSuscripcion` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `IdUsuario` INT UNSIGNED NOT NULL,
  `IdTipoSuscripcion` INT UNSIGNED NOT NULL,
  `FechaInicio` DATE NOT NULL,
  `FechaFin` DATE DEFAULT NULL,
  `Activo` TINYINT(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`IdSuscripcion`),
  INDEX `IX_Suscripcion_IdUsuario` (`IdUsuario`),
  INDEX `IX_Suscripcion_Tipo` (`IdTipoSuscripcion`),
  CONSTRAINT `FK_Suscripcion_Usuario` FOREIGN KEY (`IdUsuario`) REFERENCES `Usuario`(`IdUsuario`)
    ON DELETE CASCADE ON UPDATE NO ACTION,
  CONSTRAINT `FK_Suscripcion_Tipo` FOREIGN KEY (`IdTipoSuscripcion`) REFERENCES `TipoSuscripcion`(`IdTipoSuscripcion`)
    ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =====================================================
-- TABLA: MeGusta (Favoritos)
-- =====================================================
CREATE TABLE IF NOT EXISTS `MeGusta` (
  `IdUsuario` INT UNSIGNED NOT NULL,
  `IdCancion` INT UNSIGNED NOT NULL,
  `FechaMeGusta` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`IdUsuario`,`IdCancion`),
  INDEX `IX_MeGusta_IdCancion` (`IdCancion`),
  INDEX `IX_MeGusta_IdUsuario` (`IdUsuario`),
  CONSTRAINT `FK_MeGusta_Usuario` FOREIGN KEY (`IdUsuario`) REFERENCES `Usuario`(`IdUsuario`)
    ON DELETE CASCADE ON UPDATE NO ACTION,
  CONSTRAINT `FK_MeGusta_Cancion` FOREIGN KEY (`IdCancion`) REFERENCES `Cancion`(`IdCancion`)
    ON DELETE CASCADE ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =====================================================
-- TABLA: RegistroAuditoria
-- =====================================================
CREATE TABLE IF NOT EXISTS `RegistroAuditoria` (
  `IdAuditoria` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  `Tabla` VARCHAR(100) NOT NULL,
  `Operacion` VARCHAR(20) NOT NULL,
  `IdRegistro` VARCHAR(100) DEFAULT NULL,
  `UsuarioEjecutor` VARCHAR(100) DEFAULT NULL,
  `Fecha` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `Detalle` JSON DEFAULT NULL,
  PRIMARY KEY (`IdAuditoria`),
  INDEX `IX_Auditoria_Fecha` (`Fecha`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =====================================================
-- INSERTAR ROLES REQUERIDOS POR PDF
-- =====================================================
INSERT INTO `Rol` (`Nombre`, `Descripcion`) VALUES
('Visitante', 'Usuario sin sesión activa - Solo visualización'),
('Usuario', 'Usuario registrado con permisos básicos'),
('Administrador', 'Control total del sistema');

-- =====================================================
-- FIN: restaurar configuraciones
-- =====================================================
SET time_zone = @OLD_TIME_ZONE;
SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;