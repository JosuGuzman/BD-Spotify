USE Spotify;

-- Tabla Artista
CALL altaArtista ();
CALL altaArtista ();
CALL altaArtista ();
CALL altaArtista ();

-- Tabla Albun
 CALL altaAlbun ();
 CALL altaAlbun ();
 CALL altaAlbun ();
 CALL altaAlbun ();

-- Tabla Nacionalidad
CALL altaNacionalidad ();
CALL altaNacionalidad ();
CALL altaNacionalidad ();
CALL altaNacionalidad ();

-- Tabla Usuario
CALL altaUsuario ("Miguel", "miguelito@gmail.com", "Deadpool3saliomal", 1, @idUsuarioMiguel);
CALL altaUsuario ("Josu","josu@gmail.com","OMORIoyasumi",2,@idUsuarioJosu);
CALL altaUsuario ("Rene","rene@gmail.com","Totoro",3,@idUsuarioRene);
CALL altaUsuario ("Cheng","chengleonardo@gmail.com","capitalismonofunciona",4,@idUsuarioCheng);

-- Tabla Genero
CALL altaGenero ();
CALL altaGenero ();
CALL altaGenero ();
CALL altaGenero ();

-- Tabla Cancion
CALL altaCancion ();
CALL altaCancion ();
CALL altaCancion ();
CALL altaCancion ();

-- Tabla Historial Reproduccion
CALL altaHistorial_reproduccion ();
CALL altaHistorial_reproduccion ();
CALL altaHistorial_reproduccion ();
CALL altaHistorial_reproduccion ();

-- Tabla Playlist
CALL altaPlaylist ();
CALL altaPlaylist ();
CALL altaPlaylist ();
CALL altaPlaylist ();

-- Tabla Playlist_Cancion

-- Tabla Suscripcion
CALL altaSuscripcion ();
CALL altaSuscripcion ();
CALL altaSuscripcion ();
CALL altaSuscripcion ();

-- Tabla Suscripcion_Usuario