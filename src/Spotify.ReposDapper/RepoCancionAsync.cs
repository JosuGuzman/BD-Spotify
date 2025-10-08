namespace Spotify.ReposDapper;

public class RepoCancionAsync : RepoGenerico, IRepoCancionAsync
{
    public RepoCancionAsync(IDbConnection conexion) : base(conexion) { }

    public async Task<Cancion> AltaAsync(Cancion cancion)
    {
        var parametros = new DynamicParameters();
        parametros.Add("@unidCancion", direction: ParameterDirection.Output);
        parametros.Add("@unTitulo", cancion.Titulo);
        parametros.Add("@unDuration", cancion.Duracion);
        parametros.Add("@unidAlbum", cancion.album.idAlbum);
        parametros.Add("@unidArtista", cancion.artista.idArtista);
        parametros.Add("@unidGenero", cancion.genero.idGenero);

        await _conexion.ExecuteAsync("altaCancion", parametros, commandType: CommandType.StoredProcedure);
        cancion.idCancion = parametros.Get<uint>("@unidCancion");
        return cancion;
    }

    public async Task<Cancion?> DetalleDeAsync(uint idCancion)
    {
        string sql = @"
            SELECT c.*, 
                    a.idAlbum, a.Titulo, a.FechaLanzamiento,
                    ar.idArtista, ar.NombreArtistico, ar.Nombre, ar.Apellido,
                    g.idGenero, g.genero
            FROM Cancion c
            INNER JOIN Album a ON c.idAlbum = a.idAlbum
            INNER JOIN Artista ar ON c.idArtista = ar.idArtista
            INNER JOIN Genero g ON c.idGenero = g.idGenero
            WHERE c.idCancion = @idCancion";

        var cancion = (await _conexion.QueryAsync<Cancion, Album, Artista, Genero, Cancion>(sql,
            (cancion, album, artista, genero) => {
                cancion.album = album;
                cancion.artista = artista;
                cancion.genero = genero;
                return cancion;
            },
            new { idCancion },
            splitOn: "idAlbum,idArtista,idGenero"
        )).FirstOrDefault();

        return cancion;
    }

    public async Task<List<Cancion>> Obtener()
    {
        string sql = @"
            SELECT c.*, 
                    a.idAlbum, a.Titulo, a.FechaLanzamiento,
                    ar.idArtista, ar.NombreArtistico, ar.Nombre, ar.Apellido,
                    g.idGenero, g.genero
            FROM Cancion c
            INNER JOIN Album a ON c.idAlbum = a.idAlbum
            INNER JOIN Artista ar ON c.idArtista = ar.idArtista
            INNER JOIN Genero g ON c.idGenero = g.idGenero";

        var canciones = (await _conexion.QueryAsync<Cancion, Album, Artista, Genero, Cancion>(sql,
            (cancion, album, artista, genero) => {
                cancion.album = album;
                cancion.artista = artista;
                cancion.genero = genero;
                return cancion;
            },
            splitOn: "idAlbum,idArtista,idGenero"
        )).ToList();

        return canciones;
    }

    public async Task<List<string>?> Matcheo(string Cadena)
    {
        var parametro = new DynamicParameters();
        parametro.Add("@InputCancion", Cadena);
        var lista = await _conexion.QueryAsync<string>("MatcheoCancion", parametro, commandType: CommandType.StoredProcedure);
        return lista.ToList();
    }

    public async Task EliminarAsync(uint idCancion)
        {
            try
            {
                // Primero eliminar registros relacionados en HistorialReproduccion
                string eliminarHistorial = @"DELETE FROM HistorialReproduccion WHERE idCancion = @idCancion";
                await _conexion.ExecuteAsync(eliminarHistorial, new { idCancion });

                // Luego eliminar la canción usando stored procedure
                var parametros = new DynamicParameters();
                parametros.Add("@unidCancion", idCancion);

                await _conexion.ExecuteAsync("eliminarCancion", parametros, commandType: CommandType.StoredProcedure);

                // Alternativa si no existe el SP:
                // string eliminarCancion = @"DELETE FROM Cancion WHERE idCancion = @idCancion";
                // await _conexion.ExecuteAsync(eliminarCancion, new { idCancion });
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al eliminar la canción: {ex.Message}", ex);
            }
        }
}