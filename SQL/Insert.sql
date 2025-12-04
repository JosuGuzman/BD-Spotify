USE 5to_Spotify;

-- =============================================
-- INSERCIÓN DE DATOS CON PROCEDIMIENTOS ALMACENADOS
-- =============================================

-- Mensaje inicial
SELECT '🚀 Insertando datos con procedimientos almacenados...' AS Mensaje;

-- Limpiar datos existentes (opcional, solo para desarrollo)
SET FOREIGN_KEY_CHECKS = 0;
TRUNCATE TABLE MeGusta;
TRUNCATE TABLE Cancion_Playlist;
TRUNCATE TABLE HistorialReproduccion;
TRUNCATE TABLE Suscripcion;
TRUNCATE TABLE Playlist;
TRUNCATE TABLE Cancion;
TRUNCATE TABLE Album;
TRUNCATE TABLE Artista;
TRUNCATE TABLE Genero;
TRUNCATE TABLE Usuario;
TRUNCATE TABLE Nacionalidad;
TRUNCATE TABLE TipoSuscripcion;
SET FOREIGN_KEY_CHECKS = 1;

-- Declarar variables para IDs
SET @idArgentina = 0;
SET @idEEUU = 0;
SET @idUK = 0;
SET @idCanada = 0;
SET @idMexico = 0;
SET @idPuertoRico = 0;

-- Insertar Nacionalidades 
CALL AltaNacionalidad('Argentina', @idArgentina);
CALL AltaNacionalidad('Estados Unidos', @idEEUU);
CALL AltaNacionalidad('Reino Unido', @idUK);
CALL AltaNacionalidad('Canadá', @idCanada);
CALL AltaNacionalidad('México', @idMexico);
CALL AltaNacionalidad('Puerto Rico', @idPuertoRico);
CALL AltaNacionalidad('España', @idEspana);
CALL AltaNacionalidad('Brasil', @idBrasil);
CALL AltaNacionalidad('Chile', @idChile);
CALL AltaNacionalidad('Colombia', @idColombia);

-- Declarar variables para géneros
SET @idRock = 0;
SET @idPop = 0;
SET @idMetal = 0;
SET @idAlternativo = 0;
SET @idLatino = 0;

-- Insertar Géneros Musicales 
CALL AltaGenero('Rock', 'Música rock con guitarras eléctricas', @idRock);
CALL AltaGenero('Pop', 'Música popular comercial', @idPop);
CALL AltaGenero('Metal', 'Género pesado derivado del rock', @idMetal);
CALL AltaGenero('Alternativo', 'Música alternativa e independiente', @idAlternativo);
CALL AltaGenero('Latino', 'Música latinoamericana', @idLatino);
CALL AltaGenero('Hip Hop', 'Género originado en comunidades urbanas', @idHipHop);
CALL AltaGenero('Electrónica', 'Música creada con instrumentos electrónicos', @idElectronica);
CALL AltaGenero('R&B', 'Rhythm and blues', @idRnB);
CALL AltaGenero('Indie', 'Música independiente', @idIndie);
CALL AltaGenero('Jazz', 'Género con improvisación y ritmos complejos', @idJazz);

-- Declarar variables para artistas
SET @idThreeDaysGrace = 0;
SET @idSkillet = 0;
SET @idFallingInReverse = 0;
SET @idTheOffspring = 0;
SET @idBreakingBenjamin = 0;
SET @idDisturbed = 0;
SET @idOneDirection = 0;
SET @idNickelback = 0;
SET @idThirtySecondsToMars = 0;
SET @idThreeDaysGrace2 = 0;
SET @idSaintAsonia = 0;
SET @idThePrettyReckless = 0;
SET @idSantaflow = 0;

-- Insertar Artistas 
CALL AltaArtista('Three Days Grace', 'Adam', 'Gontier', @idCanada, @idThreeDaysGrace);
CALL AltaArtista('Skillet', 'John', 'Cooper', @idEEUU, @idSkillet);
CALL AltaArtista('Falling in Reverse', 'Ronnie', 'Radke', @idEEUU, @idFallingInReverse);
CALL AltaArtista('The Offspring', 'Bryan', 'Holland', @idEEUU, @idTheOffspring);
CALL AltaArtista('Breaking Benjamin', 'Benjamin', 'Burnley', @idEEUU, @idBreakingBenjamin);
CALL AltaArtista('Disturbed', 'David', 'Draiman', @idEEUU, @idDisturbed);
CALL AltaArtista('One Direction', 'Harry', 'Styles', @idUK, @idOneDirection);
CALL AltaArtista('Nickelback', 'Chad', 'Kroeger', @idCanada, @idNickelback);
CALL AltaArtista('Thirty Seconds to Mars', 'Jared', 'Leto', @idEEUU, @idThirtySecondsToMars);
CALL AltaArtista('Saint Asonia', 'Adam', 'Gontier', @idCanada, @idSaintAsonia);
CALL AltaArtista('The Pretty Reckless', 'Taylor', 'Momsen', @idEEUU, @idThePrettyReckless);
CALL AltaArtista('Santaflow', 'Santiago', 'Benítez', @idArgentina, @idSantaflow);
CALL AltaArtista('Bad Bunny', 'Benito', 'Martínez', @idPuertoRico, @idBadBunny);
CALL AltaArtista('Taylor Swift', 'Taylor', 'Swift', @idEEUU, @idTaylorSwift);
CALL AltaArtista('The Weeknd', 'Abel', 'Tesfaye', @idCanada, @idTheWeeknd);

-- Declarar variables para álbumes
SET @idAlbumOneX = 0;
SET @idAlbumThisIsWar = 0;
SET @idAlbumThreeDaysGrace = 0;
SET @idAlbumAwake = 0;
SET @idAlbumRiseAndFall = 0;
SET @idAlbumPhobia = 0;
SET @idAlbumTenThousandFists = 0;
SET @idAlbumMadeInTheAM = 0;
SET @idAlbumAllTheRightReasons = 0;
SET @idAlbumComatose = 0;
SET @idAlbumAtrapalosATodos = 0;

-- Insertar Álbumes 
CALL AltaAlbum('One-X', @idThreeDaysGrace, 'onex.jpg', @idAlbumOneX);
CALL AltaAlbum('This Is War', @idThirtySecondsToMars, 'thisiswar.jpg', @idAlbumThisIsWar);
CALL AltaAlbum('Three Days Grace', @idThreeDaysGrace, 'threedaysgrace.jpg', @idAlbumThreeDaysGrace);
CALL AltaAlbum('Awake', @idSkillet, 'awake.jpg', @idAlbumAwake);
CALL AltaAlbum('Rise and Fall, Rage and Grace', @idTheOffspring, 'riseandfall.jpg', @idAlbumRiseAndFall);
CALL AltaAlbum('Phobia', @idBreakingBenjamin, 'phobia.jpg', @idAlbumPhobia);
CALL AltaAlbum('Ten Thousand Fists', @idDisturbed, 'tenthousandfists.jpg', @idAlbumTenThousandFists);
CALL AltaAlbum('Made in the A.M.', @idOneDirection, 'madeintheam.jpg', @idAlbumMadeInTheAM);
CALL AltaAlbum('All the Right Reasons', @idNickelback, 'alltherightreasons.jpg', @idAlbumAllTheRightReasons);
CALL AltaAlbum('Comatose', @idSkillet, 'comatose.jpg', @idAlbumComatose);
CALL AltaAlbum('Atrapalos a Todos', @idSantaflow, 'atrapalos.jpg', @idAlbumAtrapalosATodos);
CALL AltaAlbum('Un Verano Sin Ti', @idBadBunny, 'unverano.jpg', @idAlbumUnVerano);
CALL AltaAlbum('Midnights', @idTaylorSwift, 'midnights.jpg', @idAlbumMidnights);
CALL AltaAlbum('After Hours', @idTheWeeknd, 'afterhours.jpg', @idAlbumAfterHours);

-- Declarar variables para canciones
SET @idCancion1 = 0; SET @idCancion2 = 0; SET @idCancion3 = 0;
SET @idCancion4 = 0; SET @idCancion5 = 0; SET @idCancion6 = 0;
SET @idCancion7 = 0; SET @idCancion8 = 0; SET @idCancion9 = 0;
SET @idCancion10 = 0; SET @idCancion11 = 0; SET @idCancion12 = 0;
SET @idCancion13 = 0; SET @idCancion14 = 0; SET @idCancion15 = 0;
SET @idCancion16 = 0; SET @idCancion17 = 0; SET @idCancion18 = 0;
SET @idCancion19 = 0; SET @idCancion20 = 0; SET @idCancion21 = 0;
SET @idCancion22 = 0; SET @idCancion23 = 0; SET @idCancion24 = 0;
SET @idCancion25 = 0; SET @idCancion26 = 0; SET @idCancion27 = 0;

-- Insertar Canciones solicitadas 
CALL AltaCancion('Animal I Have Become', 183, @idAlbumOneX, @idThreeDaysGrace, @idRock, 'animal_i_have_become.mp3', @idCancion1);
CALL AltaCancion('This is War', 300, @idAlbumThisIsWar, @idThirtySecondsToMars, @idRock, 'this_is_war.mp3', @idCancion2);
CALL AltaCancion('Just Like You', 216, @idAlbumThreeDaysGrace, @idThreeDaysGrace, @idRock, 'just_like_you.mp3', @idCancion3);
CALL AltaCancion('Legion of Monsters', 240, @idAlbumAwake, @idSkillet, @idRock, 'legion_of_monsters.mp3', @idCancion4);
CALL AltaCancion('You''re Gonna Go Far, Kid', 180, @idAlbumRiseAndFall, @idTheOffspring, @idRock, 'youre_gonna_go_far_kid.mp3', @idCancion5);
CALL AltaCancion('Breathe Into Me', 225, @idAlbumPhobia, @idBreakingBenjamin, @idRock, 'breathe_into_me.mp3', @idCancion6);
CALL AltaCancion('Monster', 180, @idAlbumAwake, @idSkillet, @idRock, 'monster.mp3', @idCancion7);
CALL AltaCancion('Drag Me Down', 190, @idAlbumMadeInTheAM, @idOneDirection, @idPop, 'drag_me_down.mp3', @idCancion8);
CALL AltaCancion('Savin'' Me', 220, @idAlbumAllTheRightReasons, @idNickelback, @idRock, 'savin_me.mp3', @idCancion9);
CALL AltaCancion('Beggin''', 210, @idAlbumAllTheRightReasons, @idNickelback, @idRock, 'beggin.mp3', @idCancion10);
CALL AltaCancion('Crawl', 245, @idAlbumPhobia, @idBreakingBenjamin, @idRock, 'crawl.mp3', @idCancion11);
CALL AltaCancion('Falling Inside The Black', 230, @idAlbumAwake, @idSkillet, @idRock, 'falling_inside_the_black.mp3', @idCancion12);
CALL AltaCancion('The Resistance', 195, @idAlbumThisIsWar, @idThirtySecondsToMars, @idRock, 'the_resistance.mp3', @idCancion13);
CALL AltaCancion('Hero', 210, @idAlbumAwake, @idSkillet, @idRock, 'hero.mp3', @idCancion14);
CALL AltaCancion('Its Mine', 200, @idAlbumAwake, @idSkillet, @idRock, 'its_mine.mp3', @idCancion15);
CALL AltaCancion('Thnks fr th Mmrs', 205, @idAlbumAllTheRightReasons, @idNickelback, @idRock, 'thnks_fr_th_mmrs.mp3', @idCancion16);
CALL AltaCancion('Will Not Bow', 215, @idAlbumAwake, @idSkillet, @idRock, 'will_not_bow.mp3', @idCancion17);
CALL AltaCancion('Comatose', 220, @idAlbumComatose, @idSkillet, @idRock, 'comatose.mp3', @idCancion18);
CALL AltaCancion('Let You Down', 195, @idAlbumAwake, @idSkillet, @idRock, 'let_you_down.mp3', @idCancion19);
CALL AltaCancion('The Phoenix', 240, @idAlbumThisIsWar, @idThirtySecondsToMars, @idRock, 'the_phoenix.mp3', @idCancion20);
CALL AltaCancion('Hero Of Our Time', 230, @idAlbumAwake, @idSkillet, @idRock, 'hero_of_our_time.mp3', @idCancion21);
CALL AltaCancion('One For the Money', 185, @idAlbumAwake, @idSkillet, @idRock, 'one_for_the_money.mp3', @idCancion22);
CALL AltaCancion('Fight (Like It''s Your Last)', 210, @idAlbumAwake, @idSkillet, @idRock, 'fight_like_its_your_last.mp3', @idCancion23);
CALL AltaCancion('Angeles Fuimos', 245, @idAlbumAtrapalosATodos, @idSantaflow, @idLatino, 'angeles_fuimos.mp3', @idCancion24);
CALL AltaCancion('Atrapalos a Todos', 260, @idAlbumAtrapalosATodos, @idSantaflow, @idLatino, 'atrapalos_a_todos.mp3', @idCancion25);

-- Canciones adicionales populares
CALL AltaCancion('Moscow Mule', 245, @idAlbumUnVerano, @idBadBunny, @idLatino, 'moscow_mule.mp3', @idCancion26);
CALL AltaCancion('Anti-Hero', 200, @idAlbumMidnights, @idTaylorSwift, @idPop, 'anti_hero.mp3', @idCancion27);

-- Declarar variables para tipos de suscripción
SET @idGratis = 0;
SET @idPremium = 0;

-- Insertar Tipos de Suscripción 
CALL AltaTipoSuscripcion(0, 0.00, 'Gratis', @idGratis);
CALL AltaTipoSuscripcion(1, 9.99, 'Premium Individual', @idPremium);
CALL AltaTipoSuscripcion(1, 12.99, 'Premium Duo', @idPremiumDuo);
CALL AltaTipoSuscripcion(1, 15.99, 'Premium Familiar', @idPremiumFamiliar);

-- Declarar variables para usuarios
SET @idAdmin = 0;
SET @idUsuario1 = 0;
SET @idUsuario2 = 0;

-- Insertar Usuarios 
CALL AltaUsuario('admin', 'admin@spotify.com', '$2a$10$N9qo8uLOickgx2ZMRZoMye8G7C5p1F0J7y8YH8qH7t8N4', @idArgentina, 3, @idAdmin);
CALL AltaUsuario('maria_garcia', 'maria@email.com', '$2a$10$N9qo8uLOickgx2ZMRZoMye8G7C5p1F0J7y8YH8qH7t8N4', @idArgentina, 2, @idUsuario1);
CALL AltaUsuario('carlos_lopez', 'carlos@email.com', '$2a$10$N9qo8uLOickgx2ZMRZoMye8G7C5p1F0J7y8YH8qH7t8N4', @idMexico, 2, @idUsuario2);

-- Declarar variables para playlists
SET @idPlaylistRock = 0;
SET @idPlaylistFavoritas = 0;

-- Insertar Playlists
CALL AltaPlaylist('Rock Pesado', @idUsuario1, 'Lo mejor del rock y metal', TRUE, @idPlaylistRock);
CALL AltaPlaylist('Mis Favoritas', @idUsuario1, 'Mis canciones preferidas', FALSE, @idPlaylistFavoritas);

-- Agregar canciones a playlists
CALL AgregarCancionPlaylist(@idCancion1, @idPlaylistRock);  -- Animal I Have Become
CALL AgregarCancionPlaylist(@idCancion2, @idPlaylistRock);  -- This is War
CALL AgregarCancionPlaylist(@idCancion3, @idPlaylistRock);  -- Just Like You
CALL AgregarCancionPlaylist(@idCancion7, @idPlaylistRock);  -- Monster
CALL AgregarCancionPlaylist(@idCancion18, @idPlaylistRock); -- Comatose

CALL AgregarCancionPlaylist(@idCancion1, @idPlaylistFavoritas);
CALL AgregarCancionPlaylist(@idCancion24, @idPlaylistFavoritas);
CALL AgregarCancionPlaylist(@idCancion27, @idPlaylistFavoritas);

-- Declarar variables para suscripciones
SET @idSuscripcion1 = 0;

-- Insertar Suscripciones
CALL AltaSuscripcionUsuario(@idUsuario1, @idPremium, @idSuscripcion1);

-- Insertar Me Gusta
INSERT INTO MeGusta (IdUsuario, IdCancion) VALUES
(@idUsuario1, @idCancion1),  -- Animal I Have Become
(@idUsuario1, @idCancion7),  -- Monster
(@idUsuario1, @idCancion24), -- Angeles Fuimos
(@idUsuario2, @idCancion2),  -- This is War
(@idUsuario2, @idCancion5);  -- You're Gonna Go Far, Kid

-- Insertar Historial de Reproducción usando SP
CALL AltaHistorialReproduccion(@idUsuario1, @idCancion1, 180, @idHistorial1);
CALL AltaHistorialReproduccion(@idUsuario1, @idCancion7, 170, @idHistorial2);
CALL AltaHistorialReproduccion(@idUsuario2, @idCancion2, 290, @idHistorial3);
CALL AltaHistorialReproduccion(@idUsuario2, @idCancion24, 240, @idHistorial4);

-- Verificación final
SELECT '✅ Datos insertados correctamente usando procedimientos almacenados' AS Mensaje;
SELECT 
    (SELECT COUNT(*) FROM Usuario) AS 'Total Usuarios',
    (SELECT COUNT(*) FROM Artista) AS 'Total Artistas',
    (SELECT COUNT(*) FROM Album) AS 'Total Álbumes',
    (SELECT COUNT(*) FROM Cancion) AS 'Total Canciones',
    (SELECT COUNT(*) FROM Playlist) AS 'Total Playlists';

-- Mostrar canciones insertadas
SELECT '🎵 Canciones insertadas:' AS Titulo;
SELECT 
    c.Titulo AS 'Canción',
    a.NombreArtistico AS 'Artista',
    al.Titulo AS 'Álbum',
    g.Nombre AS 'Género'
FROM Cancion c
JOIN Artista a ON c.IdArtista = a.IdArtista
JOIN Album al ON c.IdAlbum = al.IdAlbum
JOIN Genero g ON c.IdGenero = g.IdGenero
ORDER BY c.Titulo;