USE Spotify;

-- Tabla Artista
CALL altaArtista ();
CALL altaArtista ();
CALL altaArtista ();
CALL altaArtista ();

-- Tabla Albun
 CALL altaAlbun (@idAlbum1, 'Luz y Sombra', @idArtista1);
 CALL altaAlbun (@idAlbum2, 'Caminos Cruzados', @idArtista2);
 CALL altaAlbun (@idAlbum3, 'Sueños de Verano', @idArtista3);
 CALL altaAlbun (@idAlbum4, 'Ecos del Pasado', @idArtista4);

-- Tabla Nacionalidad
CALL altaNacionalidad ('Argentina', @idNacionalidad1);
CALL altaNacionalidad ('Brasil', @idNacionalidad2 );
CALL altaNacionalidad ('U.R.S.S.', @idNacionalidad3);
CALL altaNacionalidad ('Bolivia', @idNacionalidad4);

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
INSERT INTO Playlist_Canciones(idPlaylist,idCancion,FechaIngresion)
 VALUES
        (1, 1, '2024-07-01 10:00:00'),
        (1, 2, '2024-07-01 10:10:00'),
        (2, 3, '2024-07-01 11:00:00'),
        (2, 4, '2024-07-01 11:10:00');


-- Tabla Suscripcion
CALL altaSuscripcion ();
CALL altaSuscripcion ();
CALL altaSuscripcion ();
CALL altaSuscripcion ();

-- Tabla Suscripcion_Usuario
INSERT INTO Usuario_Suscripcion (idUsuario,idSuscripcion,tipoSuscripcion,FechaInicio)
 VALUES
        (1,1,’Mensual’,’2024-5-3’),
        (2,2,’Bimestral,’2024-7-23’),
        (3,3,’Trimestral’,NOW());