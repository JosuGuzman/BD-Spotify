USE 5to_Spotify;

-- Eliminar Artista (y sus álbumes/canciones relacionadas)
DELIMITER $$
DROP PROCEDURE IF EXISTS eliminarArtista $$
CREATE PROCEDURE eliminarArtista(IN unidArtista INT UNSIGNED)
BEGIN
    -- Eliminar canciones del artista
    DELETE FROM Cancion WHERE idArtista = unidArtista;
    
    -- Eliminar álbumes del artista
    DELETE FROM Album WHERE idArtista = unidArtista;
    
    -- Finalmente eliminar el artista
    DELETE FROM Artista WHERE idArtista = unidArtista;
END $$

-- Eliminar Álbum (y sus canciones relacionadas)
DELIMITER $$
DROP PROCEDURE IF EXISTS eliminarAlbum $$
CREATE PROCEDURE eliminarAlbum(IN unidAlbum INT UNSIGNED)
BEGIN
    -- Eliminar canciones del álbum
    DELETE FROM Cancion WHERE idAlbum = unidAlbum;
    
    -- Eliminar el álbum
    DELETE FROM Album WHERE idAlbum = unidAlbum;
END $$

-- Eliminar Canción (y sus relaciones)
DELIMITER $$
DROP PROCEDURE IF EXISTS eliminarCancion $$
CREATE PROCEDURE eliminarCancion(IN unidCancion INT UNSIGNED)
BEGIN
    -- Eliminar de historial de reproducción
    DELETE FROM HistorialReproduccion WHERE idCancion = unidCancion;
    
    -- Eliminar de playlists
    DELETE FROM Cancion_Playlist WHERE idCancion = unidCancion;
    
    -- Finalmente eliminar la canción
    DELETE FROM Cancion WHERE idCancion = unidCancion;
END $$

-- Eliminar Género (y sus canciones relacionadas)
DELIMITER $$
DROP PROCEDURE IF EXISTS eliminarGenero $$
CREATE PROCEDURE eliminarGenero(IN unidGenero TINYINT UNSIGNED)
BEGIN
    -- Eliminar canciones del género
    DELETE FROM Cancion WHERE idGenero = unidGenero;
    
    -- Eliminar el género
    DELETE FROM Genero WHERE idGenero = unidGenero;
END $$