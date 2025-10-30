USE 5to_Spotify;

-- Tabla Artista
CALL altaArtista('TripleT', 'Miguel', 'Verduguez', @idArtistaTripleT);
CALL altaArtista('El Chapo', 'Josu', 'Duran', @idArtistaElChapo);
CALL altaArtista('La Maquinaria', 'Rene', 'Terrazas', @idArtistalaMaquinaria);
CALL altaArtista('El Renacido', 'Leonardo', 'Cheng', @idArtistaElRenacido);

-- Tabla Album (CON PORTADAS)
CALL altaAlbum (@idAlbumLuz, 'Luz y Sombra', @idArtistaTripleT, 'luz_sombra.jpg');
CALL altaAlbum (@idAlbumCaminos, 'Caminos Cruzados', @idArtistaElChapo, 'caminos_cruzados.jpg');
CALL altaAlbum (@idAlbumSuenios, 'Sueños de Verano', @idArtistalaMaquinaria, 'suenios_verano.jpg');
CALL altaAlbum (@idAlbumEcos, 'Ecos del Pasado', @idArtistaElRenacido, 'default_album.png');

-- Tabla Nacionalidad
CALL altaNacionalidad ('Argentina', @idNacionalidadArgentina);
CALL altaNacionalidad ('Brasil', @idNacionalidadBrasil);
CALL altaNacionalidad ('U.R.S.S.', @idNacionalidadURSS);
CALL altaNacionalidad ('Bolivia', @idNacionalidadBolivia);

-- Tabla Usuario
CALL altaUsuario ("Miguel", "miguelito@gmail.com", "Deadpool3saliomal", 1, @idUsuarioMiguel);
CALL altaUsuario ("Josu","josu@gmail.com","OMORIoyasumi",2,@idUsuarioJosu);
CALL altaUsuario ("Rene","rene@gmail.com","Totoro",3,@idUsuarioRene);
CALL altaUsuario ("Cheng","chengleonardo@gmail.com","capitalismonofunciona",4,@idUsuarioCheng);

-- Tabla Genero (CON DESCRIPCIONES)
CALL altaGenero('Hip-hop/Rap', 'Género musical que incorpora rima, habla rítmica y lenguaje callejero', @idGeneroHipHop);
CALL altaGenero('Jazz', 'Género musical que se originó en comunidades afroamericanas con raíces en el blues y ragtime', @idGeneroJazz);
CALL altaGenero('Reggae', 'Género musical originario de Jamaica con ritmos característicos y mensajes sociales', @idGeneroReggae);
CALL altaGenero('Ranchera', 'Género de la música popular mexicana con letras que hablan del amor y la patria', @idGeneroRanchera);

-- Tabla Cancion (CON ARCHIVOS MP3)
CALL altaCancion(@idCancionOver, 'Its Over, Isnt It', '00:02:17', @idAlbumSuenios, @idArtistaElRenacido, @idGeneroHipHop, 'over_isnt_it.mp3');
CALL altaCancion(@idCancionRene, 'René', '00:07:41', @idAlbumCaminos, @idArtistaElChapo, @idGeneroJazz, 'rene_cancion.mp3');
CALL altaCancion(@idCancionEstrella, 'Como Estrella', '00:03:40', @idAlbumLuz, @idArtistalaMaquinaria, @idGeneroReggae, 'como_estrella.mp3');
CALL altaCancion(@idCancionCelos, 'Estos Celos', '00:03:10', @idAlbumEcos, @idArtistaTripleT, @idGeneroRanchera, 'estos_celos.mp3');

-- Tabla Historial Reproduccion
CALL altaHistorial_reproduccion(@idHistorialMiguel, @idUsuarioMiguel, @idCancionOver, '2024-07-01 10:00:00');
CALL altaHistorial_reproduccion(@idHistorialJosu, @idUsuarioJosu, @idCancionRene, '2024-07-01 11:00:00');
CALL altaHistorial_reproduccion(@idHistorialRene, @idUsuarioRene, @idCancionEstrella, '2024-07-01 12:00:00');
CALL altaHistorial_reproduccion(@idHistorialCheng, @idUsuarioCheng, @idCancionCelos, '2024-07-01 13:00:00');

-- Tabla Playlist
CALL altaPlaylist('Éxitos de Rock', @idUsuarioMiguel, @idPlaylistRock);
CALL altaPlaylist('Clásicos del Pop', @idUsuarioJosu, @idPlaylistPop);
CALL altaPlaylist('Vibras de Jazz', @idUsuarioRene, @idPlaylistJazz);
CALL altaPlaylist('Ritmos Chill', @idUsuarioCheng, @idPlaylistChill);

-- Tabla TipoSuscripcion
CALL altaTipoSuscripcion(@idSuscripcionMensual,1,8,"Mensual");
CALL altaTipoSuscripcion(@idSuscripcionBimestral,2,12,"Bimestral");
CALL altaTipoSuscripcion(@idSuscripcionTrimestral,3,15,"Trimestral");
CALL altaTipoSuscripcion(@idSuscripcionCuatrimestral,4,20,"Cuatrimestral");

-- Tabla Cancion_Playlist
CALL altaPlaylistCancion(@idCancionOver, @idPlaylistRock);
CALL altaPlaylistCancion(@idCancionRene, @idPlaylistPop);
CALL altaPlaylistCancion(@idCancionEstrella, @idPlaylistJazz);
CALL altaPlaylistCancion(@idCancionCelos, @idPlaylistChill);

-- Tabla Suscripcion_Usuario
INSERT INTO Suscripcion (idUsuario, idSuscripcion, idTipoSuscripcion, FechaInicio)
VALUES
       (1,1,@idSuscripcionMensual,"2024-5-3"),
       (2,2,@idSuscripcionBimestral,"2024-7-23"),
       (3,3,@idSuscripcionTrimestral,"2024-7-23");