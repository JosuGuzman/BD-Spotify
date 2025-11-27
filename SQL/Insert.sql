USE 5to_Spotify;

-- =============================================
-- INSERCIÓN DE DATOS BÁSICOS DEL SISTEMA
-- =============================================

-- Insertar Roles del Sistema
INSERT INTO Rol (NombreRol, Descripcion) VALUES 
('Administrador', 'Acceso completo al sistema de gestión'),
('Usuario', 'Usuario estándar con permisos básicos'),
('Artista', 'Usuario que puede publicar contenido musical');

-- Insertar Nacionalidades
CALL AltaNacionalidad('Argentina', @idArgentina);
CALL AltaNacionalidad('Brasil', @idBrasil);
CALL AltaNacionalidad('México', @idMexico);
CALL AltaNacionalidad('España', @idEspana);
CALL AltaNacionalidad('Estados Unidos', @idEEUU);
CALL AltaNacionalidad('Colombia', @idColombia);

-- Insertar Géneros Musicales
CALL AltaGenero('Pop', 'Música popular comercial', @idPop);
CALL AltaGenero('Rock', 'Género musical tradicional con instrumentos eléctricos', @idRock);
CALL AltaGenero('Hip Hop', 'Género originado en comunidades urbanas', @idHipHop);
CALL AltaGenero('Electrónica', 'Música creada con instrumentos electrónicos', @idElectronica);
CALL AltaGenero('Reggaetón', 'Género originario de Puerto Rico', @idReggaeton);
CALL AltaGenero('Jazz', 'Género con improvisación y ritmos complejos', @idJazz);

-- =============================================
-- INSERCIÓN DE ARTISTAS
-- =============================================

CALL AltaArtista('Bad Bunny', 'Benito', 'Martínez', @idBadBunny);
CALL AltaArtista('Taylor Swift', 'Taylor', 'Swift', @idTaylorSwift);
CALL AltaArtista('The Weeknd', 'Abel', 'Tesfaye', @idTheWeeknd);
CALL AltaArtista('Dua Lipa', 'Dua', 'Lipa', @idDuaLipa);
CALL AltaArtista('Ed Sheeran', 'Edward', 'Sheeran', @idEdSheeran);
CALL AltaArtista('Billie Eilish', 'Billie', 'Eilish', @idBillieEilish);

-- =============================================
-- INSERCIÓN DE ÁLBUMES
-- =============================================

CALL AltaAlbum(@idAlbumUnVerano, 'Un Verano Sin Ti', @idBadBunny, 'un_verano_sin_ti.jpg');
CALL AltaAlbum(@idAlbumMidnights, 'Midnights', @idTaylorSwift, 'midnights.jpg');
CALL AltaAlbum(@idAlbumAfterHours, 'After Hours', @idTheWeeknd, 'after_hours.jpg');
CALL AltaAlbum(@idAlbumFutureNostalgia, 'Future Nostalgia', @idDuaLipa, 'future_nostalgia.jpg');
CALL AltaAlbum(@idAlbumDivide, '÷ (Divide)', @idEdSheeran, 'divide.jpg');
CALL AltaAlbum(@idAlbumHappierThanEver, 'Happier Than Ever', @idBillieEilish, 'happier_than_ever.jpg');

-- =============================================
-- INSERCIÓN DE USUARIOS
-- =============================================

-- Usuario Administrador
CALL AltaUsuario('admin', 'admin@spotify.com', 'Admin123!', @idArgentina, @idAdmin);
UPDATE Usuario SET idRol = 1 WHERE idUsuario = @idAdmin;

-- Usuarios Regulares
CALL AltaUsuario('maria_garcia', 'maria@email.com', 'Maria123!', @idArgentina, @idMaria);
CALL AltaUsuario('carlos_lopez', 'carlos@email.com', 'Carlos123!', @idMexico, @idCarlos);
CALL AltaUsuario('ana_silva', 'ana@email.com', 'Ana123!', @idBrasil, @idAna);
CALL AltaUsuario('david_lee', 'david@email.com', 'David123!', @idEEUU, @idDavid);

-- =============================================
-- INSERCIÓN DE CANCIONES
-- =============================================

-- Canciones de Bad Bunny
CALL AltaCancion(@idCancion1, 'Tití Me Preguntó', '00:04:03', @idAlbumUnVerano, @idBadBunny, @idReggaeton, 'titi_me_pregunto.mp3');
CALL AltaCancion(@idCancion2, 'Me Porto Bonito', '00:02:58', @idAlbumUnVerano, @idBadBunny, @idReggaeton, 'me_porto_bonito.mp3');

-- Canciones de Taylor Swift
CALL AltaCancion(@idCancion3, 'Anti-Hero', '00:03:20', @idAlbumMidnights, @idTaylorSwift, @idPop, 'anti_hero.mp3');
CALL AltaCancion(@idCancion4, 'Lavender Haze', '00:03:22', @idAlbumMidnights, @idTaylorSwift, @idPop, 'lavender_haze.mp3');

-- Canciones de The Weeknd
CALL AltaCancion(@idCancion5, 'Blinding Lights', '00:03:20', @idAlbumAfterHours, @idTheWeeknd, @idPop, 'blinding_lights.mp3');
CALL AltaCancion(@idCancion6, 'Save Your Tears', '00:03:35', @idAlbumAfterHours, @idTheWeeknd, @idPop, 'save_your_tears.mp3');

-- =============================================
-- INSERCIÓN DE PLAYISTS
-- =============================================

-- Playlists públicas
CALL AltaPlaylist('Éxitos 2024', @idAdmin, @idPlaylistExitos);
UPDATE Playlist SET EsPublica = TRUE WHERE idPlaylist = @idPlaylistExitos;

CALL AltaPlaylist('Rock Clásico', @idMaria, @idPlaylistRock);
UPDATE Playlist SET EsPublica = TRUE WHERE idPlaylist = @idPlaylistRock;

-- Playlists personales
CALL AltaPlaylist('Mis Favoritas', @idMaria, @idPlaylistFavoritasMaria);
CALL AltaPlaylist('Para Estudiar', @idCarlos, @idPlaylistEstudiarCarlos);
CALL AltaPlaylist('Fiesta', @idAna, @idPlaylistFiestaAna);

-- =============================================
-- INSERCIÓN DE SUSCRIPCIONES
-- =============================================

-- Tipos de Suscripción
CALL AltaTipoSuscripcion(@idSuscripcionGratuita, 0, 0, 'Gratuita');
CALL AltaTipoSuscripcion(@idSuscripcionPremium, 1, 8, 'Premium Individual');
CALL AltaTipoSuscripcion(@idSuscripcionDuo, 1, 12, 'Premium Duo');
CALL AltaTipoSuscripcion(@idSuscripcionFamiliar, 1, 15, 'Premium Familiar');

-- Suscripciones de usuarios
CALL AltaRegistroSuscripcion(@idSuscripcion1, @idMaria, @idSuscripcionPremium);
CALL AltaRegistroSuscripcion(@idSuscripcion2, @idCarlos, @idSuscripcionDuo);
CALL AltaRegistroSuscripcion(@idSuscripcion3, @idAna, @idSuscripcionPremium);

-- =============================================
-- RELACIONES ENTRE TABLAS
-- =============================================

-- Agregar canciones a playlists
CALL AltaPlaylistCancion(@idCancion1, @idPlaylistExitos);
CALL AltaPlaylistCancion(@idCancion3, @idPlaylistExitos);
CALL AltaPlaylistCancion(@idCancion5, @idPlaylistExitos);
CALL AltaPlaylistCancion(@idCancion1, @idPlaylistFavoritasMaria);
CALL AltaPlaylistCancion(@idCancion2, @idPlaylistFiestaAna);

-- Historial de reproducción
CALL AltaHistorialReproduccion(@idHistorial1, @idMaria, @idCancion1, NOW() - INTERVAL 1 HOUR);
CALL AltaHistorialReproduccion(@idHistorial2, @idMaria, @idCancion3, NOW() - INTERVAL 30 MINUTE);
CALL AltaHistorialReproduccion(@idHistorial3, @idCarlos, @idCancion5, NOW() - INTERVAL 15 MINUTE);

-- Me Gusta
INSERT INTO MeGusta (idUsuario, idCancion) VALUES
(@idMaria, @idCancion1),
(@idMaria, @idCancion3),
(@idCarlos, @idCancion5),
(@idAna, @idCancion2);

-- =============================================
-- VERIFICACIÓN DE DATOS INSERTADOS
-- =============================================

SELECT '✅ Datos insertados correctamente' AS Estado;
SELECT COUNT(*) AS TotalUsuarios FROM Usuario;
SELECT COUNT(*) AS TotalCanciones FROM Cancion;
SELECT COUNT(*) AS TotalArtistas FROM Artista;