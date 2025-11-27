-- MySQL Workbench Forward Engineering

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';

-- -----------------------------------------------------
-- Schema 5to_Spotify
-- -----------------------------------------------------
DROP SCHEMA IF EXISTS `5to_Spotify` ;

-- -----------------------------------------------------
-- Schema 5to_Spotify
-- -----------------------------------------------------
CREATE SCHEMA IF NOT EXISTS `5to_Spotify` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci ;
USE `5to_Spotify` ;

-- -------------------------------------
-- Tabla Rol
-- -------------------------------------
CREATE TABLE IF NOT EXISTS `5to_Spotify`.`Rol` (
  `idRol` TINYINT UNSIGNED NOT NULL AUTO_INCREMENT,
  `NombreRol` VARCHAR(20) NOT NULL,
  `Descripcion` TEXT NULL,
  PRIMARY KEY (`idRol`),
  UNIQUE INDEX `NombreRol_UNICO` (`NombreRol` ASC) VISIBLE
)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;

-- -----------------------------------------------------
-- Table `5to_Spotify`.`Nacionalidad`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `5to_Spotify`.`Nacionalidad` (
  `idNacionalidad` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `Pais` VARCHAR(45) NOT NULL,
  PRIMARY KEY (`idNacionalidad`),
  UNIQUE INDEX `idNacionalidad_UNIQUE` (`idNacionalidad` ASC) VISIBLE)
ENGINE = InnoDB;

-- -----------------------------------------------------
-- Table `5to_Spotify`.`Artista`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `5to_Spotify`.`Artista` (
  `idArtista` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `NombreArtistico` VARCHAR(35) NOT NULL,
  `NombreReal` VARCHAR(45) NULL,
  `ApellidoReal` VARCHAR(45) NULL,
  `Biografia` TEXT NULL,
  `FotoArtista` VARCHAR(255) NULL DEFAULT 'default_artist.png',
  `EstaActivo` BOOLEAN NOT NULL DEFAULT TRUE,
  PRIMARY KEY (`idArtista`),
  UNIQUE INDEX `NombreArtistico_UNICO` (`NombreArtistico` ASC) VISIBLE
)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;

-- -----------------------------------------------------
-- Table `5to_Spotify`.`Genero`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `5to_Spotify`.`Genero` (
  `idGenero` TINYINT UNSIGNED NOT NULL AUTO_INCREMENT,
  `Genero` VARCHAR(45) NOT NULL,
  `Descripcion` TEXT NULL,
  PRIMARY KEY (`idGenero`))
ENGINE = InnoDB;

-- -----------------------------------------------------
-- Table `5to_Spotify`.`TipoSuscripcion`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `5to_Spotify`.`TipoSuscripcion` (
  `idTipoSuscripcion` INT UNSIGNED AUTO_INCREMENT NOT NULL,
  `Duracion` TINYINT UNSIGNED NOT NULL,
  `Costo` TINYINT UNSIGNED NOT NULL,
  `Tipo` VARCHAR(45) NOT NULL,
  PRIMARY KEY (`idTipoSuscripcion`))
ENGINE = InnoDB;

-- -----------------------------------------------------
-- Table `5to_Spotify`.`Usuario`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `5to_Spotify`.`Usuario` (
  `idUsuario` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `NombreUsuario` VARCHAR(45) NOT NULL,
  `Email` VARCHAR(45) NOT NULL,
  `Contrasenia` VARCHAR(64) NOT NULL,
  `idNacionalidad` INT UNSIGNED NOT NULL,
  `idRol` TINYINT UNSIGNED NOT NULL DEFAULT 2,
  `FechaRegistro` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `FotoPerfil` VARCHAR(255) NULL DEFAULT 'default_avatar.png',
  `EstaActivo` BOOLEAN NOT NULL DEFAULT TRUE,
  PRIMARY KEY (`idUsuario`),
  UNIQUE INDEX `email_UNICO` (`Email` ASC) VISIBLE,
  INDEX `fk_Usuario_Nacionalidad_idx` (`idNacionalidad` ASC) VISIBLE,
  INDEX `fk_Usuario_Rol_idx` (`idRol` ASC) VISIBLE,
  CONSTRAINT `fk_Usuario_Nacionalidad`
    FOREIGN KEY (`idNacionalidad`)
    REFERENCES `5to_Spotify`.`Nacionalidad` (`idNacionalidad`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_Usuario_Rol`
    FOREIGN KEY (`idRol`)
    REFERENCES `5to_Spotify`.`Rol` (`idRol`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION
)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;

-- -----------------------------------------------------
-- Table `5to_Spotify`.`Album`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `5to_Spotify`.`Album` (
  `idAlbum` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `Titulo` VARCHAR(45) NOT NULL,
  `fechaLanzamiento` DATE NOT NULL,
  `idArtista` INT UNSIGNED NOT NULL,
  `Portada` VARCHAR(255) NOT NULL DEFAULT 'default_album.png',
  PRIMARY KEY (`idAlbum`),
  INDEX `artist_id` (`idArtista` ASC) VISIBLE,
  CONSTRAINT `Albums_ibfk_1`
    FOREIGN KEY (`idArtista`)
    REFERENCES `5to_Spotify`.`Artista` (`idArtista`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;

-- -----------------------------------------------------
-- Table `5to_Spotify`.`Cancion`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `5to_Spotify`.`Cancion` (
  `idCancion` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `Titulo` VARCHAR(45) NOT NULL,
  `Duracion` TIME NOT NULL,
  `idAlbum` INT UNSIGNED NOT NULL,
  `idArtista` INT UNSIGNED NOT NULL,
  `idGenero` TINYINT UNSIGNED NOT NULL,
  `ArchivoMP3` VARCHAR(255) NOT NULL,
  PRIMARY KEY (`idCancion`),
  INDEX `album_id` (`idAlbum` ASC) VISIBLE,
  INDEX `artist_id` (`idArtista` ASC) VISIBLE,
  INDEX `fk_Canciones_Genero1_idx` (`idGenero` ASC) VISIBLE,
  CONSTRAINT `Tracks_ibfk_1`
    FOREIGN KEY (`idAlbum`)
    REFERENCES `5to_Spotify`.`Album` (`idAlbum`),
  CONSTRAINT `Tracks_ibfk_2`
    FOREIGN KEY (`idArtista`)
    REFERENCES `5to_Spotify`.`Artista` (`idArtista`),
  CONSTRAINT `fk_Canciones_Genero1`
    FOREIGN KEY (`idGenero`)
    REFERENCES `5to_Spotify`.`Genero` (`idGenero`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;

-- -----------------------------------------------------
-- Table `5to_Spotify`.`Playlist`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `5to_Spotify`.`Playlist` (
  `idPlaylist` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `Nombre` VARCHAR(20) NOT NULL,
  `idUsuario` INT UNSIGNED NOT NULL,
  PRIMARY KEY (`idPlaylist`),
  INDEX `user_id` (`idUsuario` ASC) VISIBLE,
  CONSTRAINT `Playlists_ibfk_1`
    FOREIGN KEY (`idUsuario`)
    REFERENCES `5to_Spotify`.`Usuario` (`idUsuario`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;

-- -----------------------------------------------------
-- Table `5to_Spotify`.`Cancion_Playlist`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `5to_Spotify`.`Cancion_Playlist` (
  `idCancion` INT UNSIGNED NOT NULL,
  `idPlaylist` INT UNSIGNED NOT NULL,
  PRIMARY KEY (`idCancion`, `idPlaylist`),
  INDEX `fk_Cancion_Playlist_Cancion1_idx` (`idCancion` ASC) VISIBLE,
  INDEX `fk_Cancion_Playlist_Playlist1_idx` (`idPlaylist` ASC) VISIBLE,
  CONSTRAINT `fk_Cancion_Playlist_Cancion1`
    FOREIGN KEY (`idCancion`)
    REFERENCES `5to_Spotify`.`Cancion` (`idCancion`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_Cancion_Playlist_Playlist1`
    FOREIGN KEY (`idPlaylist`)
    REFERENCES `5to_Spotify`.`Playlist` (`idPlaylist`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;

-- -----------------------------------------------------
-- Table `5to_Spotify`.`HistorialReproducción`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `5to_Spotify`.`HistorialReproduccion` (
  `idHistorial` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `idUsuario` INT UNSIGNED NOT NULL,
  `idCancion` INT UNSIGNED NOT NULL,
  `FechaReproduccion` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`idHistorial`, `idUsuario`, `idCancion`),
  INDEX `user_id` (`idUsuario` ASC) VISIBLE,
  INDEX `track_id` (`idCancion` ASC) VISIBLE,
  CONSTRAINT `Listening_History_ibfk_1`
    FOREIGN KEY (`idUsuario`)
    REFERENCES `5to_Spotify`.`Usuario` (`idUsuario`),
  CONSTRAINT `Listening_History_ibfk_2`
    FOREIGN KEY (`idCancion`)
    REFERENCES `5to_Spotify`.`Cancion` (`idCancion`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;

-- -----------------------------------------------------
-- Table `5to_Spotify`.`Suscripcion`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `5to_Spotify`.`Suscripcion` (
  `idSuscripcion` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `idUsuario` INT UNSIGNED NOT NULL,
  `idTipoSuscripcion` INT UNSIGNED NOT NULL,
  `FechaInicio` DATE NULL,
  PRIMARY KEY (`idSuscripcion` ),
  INDEX `fk_Suscripcion_TipoSuscripcion1_idx` (`idTipoSuscripcion` ASC) VISIBLE,
  CONSTRAINT `fk_Suscripcion_Usuario1`
    FOREIGN KEY (`idUsuario`)
    REFERENCES `5to_Spotify`.`Usuario` (`idUsuario`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_Suscripcion_TipoSuscripcion1`
    FOREIGN KEY (`idTipoSuscripcion`)
    REFERENCES `5to_Spotify`.`TipoSuscripcion` (`idTipoSuscripcion`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;

-- -------------------------------------
-- Tabla MeGusta
-- -------------------------------------
CREATE TABLE IF NOT EXISTS `5to_Spotify`.`MeGusta` (
  `idUsuario` INT UNSIGNED NOT NULL,
  `idCancion` INT UNSIGNED NOT NULL,
  `FechaMeGusta` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`idUsuario`, `idCancion`),
  INDEX `fk_MeGusta_Usuario_idx` (`idUsuario` ASC) VISIBLE,
  INDEX `fk_MeGusta_Cancion_idx` (`idCancion` ASC) VISIBLE,
  CONSTRAINT `fk_MeGusta_Usuario`
    FOREIGN KEY (`idUsuario`)
    REFERENCES `5to_Spotify`.`Usuario` (`idUsuario`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_MeGusta_Cancion`
    FOREIGN KEY (`idCancion`)
    REFERENCES `5to_Spotify`.`Cancion` (`idCancion`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION
)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;

-- --------------------------------------------------------------------
-- Modificar la tabla Cancion para incluir un índice de texto completo
-- --------------------------------------------------------------------
ALTER TABLE `5to_Spotify`.`Cancion`
ADD FULLTEXT INDEX `ft_index_titulo` (`Titulo`);

SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;