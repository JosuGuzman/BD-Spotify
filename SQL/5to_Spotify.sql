-- =====================================================
-- SQL Script to create the 5to_Spotify database schema
-- =====================================================
SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE;
SET SQL_MODE='STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION';
SET @OLD_TIME_ZONE=@@time_zone;
SET time_zone = '+00:00';

-- -----------------------------------------------------
-- Schema 5to_Spotify
-- -----------------------------------------------------
DROP DATABASE IF EXISTS `5to_Spotify`;
CREATE DATABASE `5to_Spotify` CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE `5to_Spotify`;

-- =====================================================
-- TABLE: Rol
-- =====================================================
CREATE TABLE IF NOT EXISTS `Rol` (
  `IdRol` TINYINT UNSIGNED NOT NULL AUTO_INCREMENT,
  `Nombre` VARCHAR(30) NOT NULL,
  `Descripcion` VARCHAR(255) DEFAULT NULL,
  PRIMARY KEY (`IdRol`),
  UNIQUE KEY `UQ_Rol_Nombre` (`Nombre`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =====================================================
-- TABLE: Nacionalidad
-- =====================================================
CREATE TABLE IF NOT EXISTS `Nacionalidad` (
  `IdNacionalidad` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `Pais` VARCHAR(100) NOT NULL,
  PRIMARY KEY (`IdNacionalidad`),
  UNIQUE KEY `UQ_Nacionalidad_Pais` (`Pais`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =====================================================
-- TABLE: Artista
-- =====================================================
CREATE TABLE IF NOT EXISTS `Artista` (
  `IdArtista` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `NombreArtistico` VARCHAR(150) NOT NULL,
  `NombreReal` VARCHAR(100) DEFAULT NULL,
  `ApellidoReal` VARCHAR(100) DEFAULT NULL,
  `Biografia` TEXT DEFAULT NULL,
  `FotoArtista` VARCHAR(255) DEFAULT 'default_artist.png',
  `EstaActivo` TINYINT(1) NOT NULL DEFAULT 1,
  `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`IdArtista`),
  UNIQUE KEY `UQ_Artista_NombreArtistico` (`NombreArtistico`),
  INDEX `IX_Artista_Nombre` (`NombreArtistico`(50))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =====================================================
-- TABLE: Genero
-- =====================================================
CREATE TABLE IF NOT EXISTS `Genero` (
  `IdGenero` TINYINT UNSIGNED NOT NULL AUTO_INCREMENT,
  `Nombre` VARCHAR(100) NOT NULL,
  `Descripcion` VARCHAR(255) DEFAULT NULL,
  PRIMARY KEY (`IdGenero`),
  UNIQUE KEY `UQ_Genero_Nombre` (`Nombre`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =====================================================
-- TABLE: TipoSuscripcion
-- =====================================================
CREATE TABLE IF NOT EXISTS `TipoSuscripcion` (
  `IdTipoSuscripcion` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `Tipo` VARCHAR(100) NOT NULL,
  `DuracionDias` INT UNSIGNED NOT NULL COMMENT 'Duración en días',
  `CostoCentavos` INT UNSIGNED NOT NULL COMMENT 'Costo en centavos (evita punto flotante)',
  PRIMARY KEY (`IdTipoSuscripcion`),
  UNIQUE KEY `UQ_TipoSuscripcion_Tipo` (`Tipo`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =====================================================
-- TABLE: Usuario
-- =====================================================
CREATE TABLE IF NOT EXISTS `Usuario` (
  `IdUsuario` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `NombreUsuario` VARCHAR(60) NOT NULL,
  `Email` VARCHAR(150) NOT NULL,
  `Contrasenia` VARCHAR(255) NOT NULL COMMENT 'bcrypt hash',
  `IdNacionalidad` INT UNSIGNED NOT NULL,
  `IdRol` TINYINT UNSIGNED NOT NULL DEFAULT 2,
  `FechaRegistro` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `FotoPerfil` VARCHAR(255) DEFAULT 'default_avatar.png',
  `EstaActivo` TINYINT(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`IdUsuario`),
  UNIQUE KEY `UQ_Usuario_Email` (`Email`),
  INDEX `IX_Usuario_IdRol` (`IdRol`),
  INDEX `IX_Usuario_IdNacionalidad` (`IdNacionalidad`),
  CONSTRAINT `FK_Usuario_Rol` FOREIGN KEY (`IdRol`) REFERENCES `Rol`(`IdRol`)
    ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `FK_Usuario_Nacionalidad` FOREIGN KEY (`IdNacionalidad`) REFERENCES `Nacionalidad`(`IdNacionalidad`)
    ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =====================================================
-- TABLE: Album
-- =====================================================
CREATE TABLE IF NOT EXISTS `Album` (
  `IdAlbum` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `Titulo` VARCHAR(200) NOT NULL,
  `FechaLanzamiento` DATE DEFAULT NULL,
  `IdArtista` INT UNSIGNED NOT NULL,
  `Portada` VARCHAR(255) DEFAULT 'default_album.png',
  `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`IdAlbum`),
  INDEX `IX_Album_IdArtista` (`IdArtista`),
  INDEX `IX_Album_Titulo` (`Titulo`(100)),
  CONSTRAINT `FK_Album_Artista` FOREIGN KEY (`IdArtista`) REFERENCES `Artista`(`IdArtista`)
    ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =====================================================
-- TABLE: Cancion
-- Optimización: duración en segundos (INT) para cálculos y ordenamiento rápido
-- =====================================================
CREATE TABLE IF NOT EXISTS `Cancion` (
  `IdCancion` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `Titulo` VARCHAR(200) NOT NULL,
  `DuracionSeg` INT UNSIGNED NOT NULL COMMENT 'duración en segundos',
  `IdAlbum` INT UNSIGNED NOT NULL,
  `IdArtista` INT UNSIGNED NOT NULL,
  `IdGenero` TINYINT UNSIGNED NOT NULL,
  `ArchivoMP3` VARCHAR(255) NOT NULL,
  `PlayCount` BIGINT UNSIGNED NOT NULL DEFAULT 0,
  `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`IdCancion`),
  INDEX `IX_Cancion_IdAlbum` (`IdAlbum`),
  INDEX `IX_Cancion_IdArtista` (`IdArtista`),
  INDEX `IX_Cancion_IdGenero` (`IdGenero`),
  INDEX `IX_Cancion_Titulo_PlayCount` (`Titulo`(100), `PlayCount`),
  CONSTRAINT `FK_Cancion_Album` FOREIGN KEY (`IdAlbum`) REFERENCES `Album`(`IdAlbum`)
    ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `FK_Cancion_Artista` FOREIGN KEY (`IdArtista`) REFERENCES `Artista`(`IdArtista`)
    ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `FK_Cancion_Genero` FOREIGN KEY (`IdGenero`) REFERENCES `Genero`(`IdGenero`)
    ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Fulltext index (titulo) para búsquedas de texto
CREATE FULLTEXT INDEX IF NOT EXISTS `FT_Cancion_Titulo` ON `Cancion`(`Titulo`);

-- =====================================================
-- TABLE: Playlist
-- =====================================================
CREATE TABLE IF NOT EXISTS `Playlist` (
  `IdPlaylist` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `IdUsuario` INT UNSIGNED NOT NULL,
  `Nombre` VARCHAR(200) NOT NULL,
  `Descripcion` TEXT DEFAULT NULL,
  `EsPublica` TINYINT(1) NOT NULL DEFAULT 1,
  `IsSystem` TINYINT(1) NOT NULL DEFAULT 0,
  `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`IdPlaylist`),
  INDEX `IX_Playlist_IdUsuario` (`IdUsuario`),
  INDEX `IX_Playlist_Nombre` (`Nombre`(100)),
  CONSTRAINT `FK_Playlist_Usuario` FOREIGN KEY (`IdUsuario`) REFERENCES `Usuario`(`IdUsuario`)
    ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =====================================================
-- TABLE: Cancion_Playlist (m:n)
-- =====================================================
CREATE TABLE IF NOT EXISTS `Cancion_Playlist` (
  `IdCancion` INT UNSIGNED NOT NULL,
  `IdPlaylist` INT UNSIGNED NOT NULL,
  `Orden` INT UNSIGNED NOT NULL DEFAULT 0,
  `AddedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`IdCancion`,`IdPlaylist`),
  INDEX `IX_CP_IdCancion` (`IdCancion`),
  INDEX `IX_CP_IdPlaylist` (`IdPlaylist`),
  CONSTRAINT `FK_CP_Cancion` FOREIGN KEY (`IdCancion`) REFERENCES `Cancion`(`IdCancion`)
    ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `FK_CP_Playlist` FOREIGN KEY (`IdPlaylist`) REFERENCES `Playlist`(`IdPlaylist`)
    ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =====================================================
-- TABLE: HistorialReproduccion
-- =====================================================
CREATE TABLE IF NOT EXISTS `HistorialReproduccion` (
  `IdHistorial` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `IdUsuario` INT UNSIGNED NOT NULL,
  `IdCancion` INT UNSIGNED NOT NULL,
  `FechaReproduccion` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `DuracionReproducida` INT UNSIGNED DEFAULT NULL COMMENT 'segundos reproducidos',
  PRIMARY KEY (`IdHistorial`),
  INDEX `IX_HR_IdUsuario` (`IdUsuario`),
  INDEX `IX_HR_IdCancion` (`IdCancion`),
  INDEX `IX_HR_Usuario_Fecha` (`IdUsuario`,`FechaReproduccion`),
  CONSTRAINT `FK_HR_Usuario` FOREIGN KEY (`IdUsuario`) REFERENCES `Usuario`(`IdUsuario`)
    ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `FK_HR_Cancion` FOREIGN KEY (`IdCancion`) REFERENCES `Cancion`(`IdCancion`)
    ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =====================================================
-- TABLE: Suscripcion
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
    ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `FK_Suscripcion_Tipo` FOREIGN KEY (`IdTipoSuscripcion`) REFERENCES `TipoSuscripcion`(`IdTipoSuscripcion`)
    ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =====================================================
-- TABLE: MeGusta (Favoritos)
-- =====================================================
CREATE TABLE IF NOT EXISTS `MeGusta` (
  `IdUsuario` INT UNSIGNED NOT NULL,
  `IdCancion` INT UNSIGNED NOT NULL,
  `FechaMeGusta` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`IdUsuario`,`IdCancion`),
  INDEX `IX_MeGusta_IdCancion` (`IdCancion`),
  INDEX `IX_MeGusta_IdUsuario` (`IdUsuario`),
  CONSTRAINT `FK_MeGusta_Usuario` FOREIGN KEY (`IdUsuario`) REFERENCES `Usuario`(`IdUsuario`)
    ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `FK_MeGusta_Cancion` FOREIGN KEY (`IdCancion`) REFERENCES `Cancion`(`IdCancion`)
    ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =====================================================
-- TABLE: AuditLog (opcional, recomendado)
-- Registra acciones críticas — útil para auditoría admin y debugging
-- =====================================================
CREATE TABLE IF NOT EXISTS `AuditLog` (
  `IdAudit` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  `Tabla` VARCHAR(100) NOT NULL,
  `Operacion` VARCHAR(20) NOT NULL,
  `IdRegistro` VARCHAR(100) DEFAULT NULL,
  `UsuarioEjecutor` VARCHAR(100) DEFAULT NULL,
  `Fecha` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `Detalle` JSON DEFAULT NULL,
  PRIMARY KEY (`IdAudit`),
  INDEX `IX_Audit_Fecha` (`Fecha`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =====================================================
-- VIEWS útiles para lecturas rápidas (opcional)
-- =====================================================

CREATE OR REPLACE VIEW `vw_CancionDetalles` AS
SELECT
  c.IdCancion,
  c.Titulo,
  c.DuracionSeg,
  c.PlayCount,
  c.ArchivoMP3,
  al.IdAlbum,
  al.Titulo AS AlbumTitulo,
  ar.IdArtista,
  ar.NombreArtistico,
  g.IdGenero,
  g.Nombre AS Genero
FROM Cancion c
JOIN Album al ON c.IdAlbum = al.IdAlbum
JOIN Artista ar ON c.IdArtista = ar.IdArtista
JOIN Genero g ON c.IdGenero = g.IdGenero;

CREATE OR REPLACE VIEW `vw_PlaylistDetalle` AS
SELECT
  p.IdPlaylist,
  p.Nombre AS PlaylistNombre,
  p.IdUsuario,
  u.NombreUsuario,
  p.IsSystem,
  p.EsPublica
FROM Playlist p
JOIN Usuario u ON p.IdUsuario = u.IdUsuario;

-- =====================================================
-- INDICES ADICIONALES RECOMENDADOS (mejoran consultas comunes)
-- =====================================================
ALTER TABLE `Cancion`
  ADD INDEX `IX_Cancion_PlayCount` (`PlayCount`),
  ADD INDEX `IX_Cancion_ArtistaGenero` (`IdArtista`,`IdGenero`);

ALTER TABLE `Album`
  ADD INDEX `IX_Album_FechaTitulo` (`FechaLanzamiento`,`Titulo`(100));

ALTER TABLE `Usuario`
  ADD INDEX `IX_Usuario_FechaRegistro` (`FechaRegistro`);

-- =====================================================
-- DATA SEED MÍNIMO (roles) - opcional para desarrollo
-- =====================================================
INSERT INTO `Rol` (`Nombre`,`Descripcion`)
  SELECT 'Visitante','Rol base visitante' UNION ALL
  SELECT 'Usuario','Usuario registrado' UNION ALL
  SELECT 'Admin','Administrador del sistema'
WHERE NOT EXISTS (SELECT 1 FROM `Rol` LIMIT 1);

-- =====================================================
-- FIN: restore settings
-- =====================================================
SET time_zone = @OLD_TIME_ZONE;
SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
