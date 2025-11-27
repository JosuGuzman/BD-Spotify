-- Script de instalación completo y optimizado
SOURCE 5to_Spotify.sql
SOURCE SP.sql
SOURCE SF.sql
SOURCE Triggers.sql
SOURCE View.sql
SOURCE Insert.sql
SOURCE DCL.sql

-- Verificación final
SELECT 'Base de datos Spotify instalada exitosamente!' AS Mensaje;
CALL ObtenerEstadisticasSistema();