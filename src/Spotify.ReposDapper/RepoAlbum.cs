namespace Spotify.ReposDapper;

public class RepoAlbum : RepoGenerico, IRepoAlbum
{
    public RepoAlbum(IDbConnection conexion)
        : base(conexion) { }

    public uint Alta(Album album)
    {
        var parametros = new DynamicParameters();
        parametros.Add("@unidAlbum", direction: ParameterDirection.Output);
        parametros.Add("@unTitulo", album.Titulo);
        parametros.Add("@unidArtista", album.artista.idArtista);

        _conexion.Execute("altaAlbum", parametros, commandType: CommandType.StoredProcedure);
        album.idAlbum = parametros.Get<uint>("@unidAlbum");
        return album.idAlbum;
    }

    public async Task<Album> AltaAsync(Album album)
    {
        var parametros = new DynamicParameters();
        parametros.Add("@unidAlbum", direction: ParameterDirection.Output);
        parametros.Add("@unTitulo", album.Titulo);
        parametros.Add("@unidArtista", album.artista.idArtista);

        await _conexion.ExecuteAsync("altaAlbum", parametros, commandType: CommandType.StoredProcedure);
        album.idAlbum = parametros.Get<uint>("@unidAlbum");
        return album;
    }



    public Album? DetalleDe(uint idAlbum)
    {
        string consultarAlbum = @"SELECT * FROM Album WHERE idAlbum = @idAlbum";

        var Album = _conexion.QuerySingleOrDefault<Album>(consultarAlbum, new { idAlbum });

        return Album;
    }

    public async Task<Album?> DetalleDeAsync(uint idAlbum)
    {
        string consultarAlbum = @"SELECT * FROM Album WHERE idAlbum = @idAlbum";

        var Album = await _conexion.QuerySingleOrDefaultAsync<Album>(consultarAlbum, new { idAlbum });

        return Album;
    }

    public void Eliminar(uint idAlbum)
    {
        string eliminarCanciones = @"DELETE FROM Cancion WHERE idAlbum = @idAlbum";
        _conexion.Execute(eliminarCanciones, new { idAlbum });

        string eliminarAlbum = @"DELETE FROM Album WHERE idAlbum = @idAlbum";
        _conexion.Execute(eliminarAlbum, new { idAlbum });
    }

    public async Task EliminarAsync(uint idAlbum)
    {
        string eliminarCanciones = @"DELETE FROM Cancion WHERE idAlbum = @idAlbum";
        await _conexion.ExecuteAsync(eliminarCanciones, new { idAlbum });

        string eliminarAlbum = @"DELETE FROM Album WHERE idAlbum = @idAlbum";
        await _conexion.ExecuteAsync(eliminarAlbum, new { idAlbum });
    }

    public IList<Album> Obtener() => EjecutarSPConReturnDeTipoLista<Album>("ObtenerAlbum").ToList();

    public async Task<List<Album>> ObtenerAsync()
    {
        var resultado = await _conexion.QueryAsync<Album>("ObtenerAlbum", commandType: CommandType.StoredProcedure);
        return resultado.ToList();
    }

}
