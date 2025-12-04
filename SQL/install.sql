-- =====================================================
-- SCRIPT DE INSTALACIÓN COMPLETO EN ESPAÑOL
-- =====================================================

SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;

-- Mensaje inicial
SELECT '🚀 Iniciando instalación de Spotify Database...' AS Mensaje;

-- 1. Crear esquema y tablas
SOURCE 5to_Spotify.sql;
SELECT '✅ Esquema de base de datos creado' AS Mensaje;

-- 2. Procedimientos Almacenados
SOURCE SP_espanol.sql;
SELECT '✅ Procedimientos almacenados creados' AS Mensaje;

-- 3. Insertar datos usando SPs
SOURCE Insert_espanol.sql;
SELECT '✅ Datos insertados correctamente' AS Mensaje;

-- Verificación final
SELECT '🎉 Instalación completada exitosamente!' AS Mensaje;

-- Mostrar resumen
SELECT 
    '📊 RESUMEN DE INSTALACIÓN' AS Titulo,
    (SELECT COUNT(*) FROM information_schema.TABLES WHERE TABLE_SCHEMA = 'Spotify') AS 'Tablas Creadas',
    (SELECT COUNT(*) FROM information_schema.ROUTINES WHERE ROUTINE_SCHEMA = 'Spotify' AND ROUTINE_TYPE = 'PROCEDURE') AS 'Procedimientos',
    (SELECT COUNT(*) FROM Usuario) AS 'Usuarios',
    (SELECT COUNT(*) FROM Artista) AS 'Artistas',
    (SELECT COUNT(*) FROM Cancion) AS 'Canciones',
    (SELECT COUNT(*) FROM Album) AS 'Álbumes';

-- Mostrar estadísticas
CALL ObtenerEstadisticasSistema();

-- Mostrar algunas canciones
SELECT '🎵 Muestra de canciones insertadas:' AS Titulo;
SELECT 
    c.IdCancion AS 'ID',
    c.Titulo AS 'Canción',
    a.NombreArtistico AS 'Artista',
    g.Nombre AS 'Género',
    SEC_TO_TIME(c.DuracionSegundos) AS 'Duración'
FROM Cancion c
JOIN Artista a ON c.IdArtista = a.IdArtista
JOIN Genero g ON c.IdGenero = g.IdGenero
ORDER BY c.ContadorReproducciones DESC
LIMIT 10;

-- Restaurar configuración
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;

SELECT '✅ Base de datos Spotify en español lista para usar!' AS Estado_Final;