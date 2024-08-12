USE Spotify;

-- Tabla Artista
 CALL altaArtista('TripleT', 'Miguel', 'Verduguez', @idArtistaTripleT);
 CALL altaArtista('El Chapo', 'Josu', 'Duran', @idArtistaElChapo);
 CALL altaArtista('La Maquinaria', 'Rene', 'Terrazas', @idArtistalaMaquinaria);
 CALL altaArtista('El Renacido', 'Leonardo', 'Cheng', @idArtistaElRenacido);
-- Tabla Albun
 CALL altaAlbun (@idAlbum1, 'Luz y Sombra', @idArtista1);
 CALL altaAlbun (@idAlbum2, 'Caminos Cruzados', @idArtista2);
 CALL altaAlbun (@idAlbum3, 'Sueños de Verano', @idArtista3);
 CALL altaAlbun (@idAlbum4, 'Ecos del Pasado', @idArtista4);

-- Tabla Nacionalidad
CALL altaNacionalidad ('Argentina', @idNacionalidadArgentina);
CALL altaNacionalidad ('Brasil', @idNacionalidadBrasil );
CALL altaNacionalidad ('U.R.S.S.', @idNacionalidadURSS);
CALL altaNacionalidad ('Bolivia', @idNacionalidadBolivia);

-- Tabla Usuario
CALL altaUsuario ("Miguel", "miguelito@gmail.com", "Deadpool3saliomal", 1, @idUsuarioMiguel);
CALL altaUsuario ("Josu","josu@gmail.com","OMORIoyasumi",2,@idUsuarioJosu);
CALL altaUsuario ("Rene","rene@gmail.com","Totoro",3,@idUsuarioRene);
CALL altaUsuario ("Cheng","chengleonardo@gmail.com","capitalismonofunciona",4,@idUsuarioCheng);

-- Tabla Genero
CALL altaGenero('Hip-hop/Rap', @idGeneroHipHop);
CALL altaGenero('Jazz', @idGeneroJazz);
CALL altaGenero('Reggae', @idGeneroReggae);
CALL altaGenero('Ranchera', @idGeneroRanchera);
-- Tabla Cancion
CALL altaCancion(@idCancion1, 'It\'s Over, Isn\'t It', '00:02:17', @idAlbum3, @idArtista1, @idGenero1);
CALL altaCancion(@idCancion2, 'René', '00:07:41', @idAlbum2, @idArtista2, @idGenero2);
CALL altaCancion(@idCancion3, 'Como Estrella', '00:03:40', @idAlbum1, @idArtista3, @idGenero3);
CALL altaCancion(@idCancion4, 'Estos Celos', '00:03:10', @idAlbum4, @idArtista4, @idGenero4);


-- Tabla Historial Reproduccion
CALL altaHistorial_reproduccion(@idHistorial1, @idUsuario1, @idCancion1, '2024-07-01 10:00:00');
CALL altaHistorial_reproduccion(@idHistorial2, @idUsuario2, @idCancion2, '2024-07-01 11:00:00');
CALL altaHistorial_reproduccion(@idHistorial3, @idUsuario3, @idCancion3, '2024-07-01 12:00:00');
CALL altaHistorial_reproduccion(@idHistorial4, @idUsuario4, @idCancion4, '2024-07-01 13:00:00');

-- Tabla Playlist
CALL altaPlaylist('Éxitos de Rock', @idUsuario1, @idPlaylist1);
CALL altaPlaylist('Clásicos del Pop', @idUsuario2, @idPlaylist2);
CALL altaPlaylist('Vibras de Jazz', @idUsuario3, @idPlaylist3);
CALL altaPlaylist('Ritmos Chill', @idUsuario4, @idPlaylist4);


-- Tabla Playlist_Cancion
INSERT INTO Playlist_Canciones(idPlaylist,idCancion,FechaIngresion)
 VALUES
        (1, 1, '2024-07-01 10:00:00'),
        (1, 2, '2024-07-01 10:10:00'),
        (2, 3, '2024-07-01 11:00:00'),
        (2, 4, '2024-07-01 11:10:00');


-- Tabla Suscripcion
CALL altaSuscripcion (@idSuscripcionMensual,1,8,’Mensual’);
CALL altaSuscripcion (@idSuscripcionTrimestral,3,15,’Trimestral’);
CALL altaSuscripcion (@idSuscripcionCuatrimestrall,4,20,’Cuatrimestral’);
CALL altaSuscripcion (@idSuscripcionBimestral,2,12,’Bimestral);



-- Tabla Suscripcion_Usuario
INSERT INTO Usuario_Suscripcion (idUsuario,idSuscripcion,tipoSuscripcion,FechaInicio)
 VALUES
        (1,1,’Mensual’,’2024-5-3’),
        (2,2,’Bimestral,’2024-7-23’),
        (3,3,’Trimestral’,NOW());