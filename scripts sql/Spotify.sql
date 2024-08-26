-- MySQL Workbench Forward Engineering

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';

-- -----------------------------------------------------
-- Schema Spotify
-- -----------------------------------------------------
DROP SCHEMA IF EXISTS `Spotify` ;

-- -----------------------------------------------------
-- Schema Spotify
-- -----------------------------------------------------
CREATE SCHEMA IF NOT EXISTS `Spotify` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci ;
USE `Spotify` ;

-- -----------------------------------------------------
-- Table `Spotify`.`Artista`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `Spotify`.`Artista` (
  `idArtista` INT NOT NULL AUTO_INCREMENT,
  `NombreArtistico` VARCHAR(35) NULL,
  `Nombre` VARCHAR(45) NOT NULL,
  `Apellido` VARCHAR(45) NOT NULL,
  PRIMARY KEY (`idArtista`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `Spotify`.`Album`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `Spotify`.`Album` (
  `idAlbun` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `Titulo` VARCHAR(45) NOT NULL,
  `fechaLanzamiento` DATE NOT NULL,
  `idArtista` INT UNSIGNED NOT NULL,
  PRIMARY KEY (`idAlbun`),
  INDEX `artist_id` (`idArtista` ASC) VISIBLE,
  CONSTRAINT `Albums_ibfk_1`
    FOREIGN KEY (`idArtista`)
    REFERENCES `Spotify`.`Artista` (`idArtista`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `Spotify`.`Nacionalidad`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `Spotify`.`Nacionalidad` (
  `idNacionalidad` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `Pais` VARCHAR(45) NOT NULL,
  PRIMARY KEY (`idNacionalidad`),
  UNIQUE INDEX `idNacionalidad_UNIQUE` (`idNacionalidad` ASC) VISIBLE)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `Spotify`.`Usuario`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `Spotify`.`Usuario` (
  `idUsuario` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `NombreUsuario` VARCHAR(45) NOT NULL,
  `Email` VARCHAR(45) NOT NULL,
  `Contraseña` VARCHAR(64) NOT NULL,
  `idNacionalidad` INT UNSIGNED NOT NULL,
  PRIMARY KEY (`idUsuario`),
  UNIQUE INDEX `email` (`Email` ASC) VISIBLE,
  INDEX `fk_Usuario_Nacionalidad1_idx` (`idNacionalidad` ASC) VISIBLE,
  CONSTRAINT `fk_Usuario_Nacionalidad1`
    FOREIGN KEY (`idNacionalidad`)
    REFERENCES `Spotify`.`Nacionalidad` (`idNacionalidad`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `Spotify`.`Genero`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `Spotify`.`Genero` (
  `idGenero` TINYINT UNSIGNED NOT NULL AUTO_INCREMENT,
  `Genero` VARCHAR(45) NOT NULL,
  PRIMARY KEY (`idGenero`))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `Spotify`.`Cancion`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `Spotify`.`Cancion` (
  `idCancion` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `Titulo` VARCHAR(45) NOT NULL,
  `duration` TIME NOT NULL,
  `idAlbun` INT UNSIGNED NOT NULL,
  `idArtista` INT UNSIGNED NOT NULL,
  `idGenero` TINYINT UNSIGNED NOT NULL,
  PRIMARY KEY (`idCancion`),
  INDEX `album_id` (`idAlbun` ASC) VISIBLE,
  INDEX `artist_id` (`idArtista` ASC) VISIBLE,
  INDEX `fk_Canciones_Genero1_idx` (`idGenero` ASC) VISIBLE,
  CONSTRAINT `Tracks_ibfk_1`
    FOREIGN KEY (`idAlbun`)
    REFERENCES `Spotify`.`Album` (`idAlbun`),
  CONSTRAINT `Tracks_ibfk_2`
    FOREIGN KEY (`idArtista`)
    REFERENCES `Spotify`.`Artista` (`idArtista`),
  CONSTRAINT `fk_Canciones_Genero1`
    FOREIGN KEY (`idGenero`)
    REFERENCES `Spotify`.`Genero` (`idGenero`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `Spotify`.`HistorialReproducción`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `Spotify`.`HistorialReproducción` (
  `idHistorial` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `idUsuario` INT UNSIGNED NOT NULL,
  `idCancion` INT UNSIGNED NOT NULL,
  `FechaReproduccion` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`idHistorial`, `idUsuario`, `idCancion`),
  INDEX `user_id` (`idUsuario` ASC) VISIBLE,
  INDEX `track_id` (`idCancion` ASC) VISIBLE,
  CONSTRAINT `Listening_History_ibfk_1`
    FOREIGN KEY (`idUsuario`)
    REFERENCES `Spotify`.`Usuario` (`idUsuario`),
  CONSTRAINT `Listening_History_ibfk_2`
    FOREIGN KEY (`idCancion`)
    REFERENCES `Spotify`.`Cancion` (`idCancion`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `Spotify`.`Playlist`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `Spotify`.`Playlist` (
  `idPlaylist` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `Nombre` VARCHAR(20) NOT NULL,
  `idUsuario` INT UNSIGNED NOT NULL,
  PRIMARY KEY (`idPlaylist`),
  INDEX `user_id` (`idUsuario` ASC) VISIBLE,
  CONSTRAINT `Playlists_ibfk_1`
    FOREIGN KEY (`idUsuario`)
    REFERENCES `Spotify`.`Usuario` (`idUsuario`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `Spotify`.`TipoSuscripcion`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `Spotify`.`TipoSuscripcion` (
  `idTipoSuscripcion` INT UNSIGNED NOT NULL,
  `Duracion` TINYINT UNSIGNED NOT NULL,
  `Costo` TINYINT UNSIGNED NOT NULL,
  `Tipo` VARCHAR(45) NOT NULL,
  PRIMARY KEY (`idTipoSuscripcion`))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `Spotify`.`Suscripcion`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `Spotify`.`Suscripcion` (
  `idUsuario` INT UNSIGNED NOT NULL,
  `idTipoSuscripcion` INT UNSIGNED NOT NULL,
  `tipoTipoSuscripcion` VARCHAR(45) NOT NULL,
  `FechaInicio` DATE NULL,
  PRIMARY KEY (`idUsuario`, `idTipoSuscripcion`),
  INDEX `fk_Suscripcion_TipoSuscripcion1_idx` (`idTipoSuscripcion` ASC) VISIBLE,
  CONSTRAINT `fk_Suscripcion_Usuario1`
    FOREIGN KEY (`idUsuario`)
    REFERENCES `Spotify`.`Usuario` (`idUsuario`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_Suscripcion_TipoSuscripcion1`
    FOREIGN KEY (`idTipoSuscripcion`)
    REFERENCES `Spotify`.`TipoSuscripcion` (`idTipoSuscripcion`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;