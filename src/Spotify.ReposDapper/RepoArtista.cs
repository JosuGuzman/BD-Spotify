namespace Spotify.ReposDapper;

public class RepoArtista : RepoGenerico, IRepoArtista
{
    public RepoArtista(IDbConnection conexion) 
        : base(conexion) { }

    public uint Alta(Artista artista)
    {
        var parametros = new DynamicParameters();
        parametros.Add("@unidArtista", direction: ParameterDirection.Output);
        parametros.Add("@unNombreArtistico", artista.NombreArtistico);
        parametros.Add("@unNombre", artista.Nombre);
        parametros.Add("@unApellido", artista.Apellido);

        _conexion.Execute("altaArtista", parametros, commandType: CommandType.StoredProcedure);

        artista.idArtista = parametros.Get<uint>("@unidArtista");

        return artista.idArtista;
    }

    public void Eliminar(uint idArtista)
    {
        string eliminarAlbum = @"DELETE FROM Album WHERE idArtista = @idArtista";
        _conexion.Execute(eliminarAlbum, new { idArtista });
        
        string eliminarArtista = @"DELETE FROM Artista WHERE idArtista = @idArtista";
        _conexion.Execute(eliminarArtista, new { idArtista });
    }

    public IList<Artista> Obtener() => EjecutarSPConReturnDeTipoLista<Artista>("ObtenerArtistas").ToList();

    public Artista? DetalleDe(uint idArtista)
    {
        string consultarArtista = @"SELECT * FROM Artista WHERE idArtista = @idArtista";

        var artista = _conexion.QuerySingleOrDefault<Artista>(consultarArtista, new {idArtista});

        return artista;
    }

    public async Task<Artista> AltaAsync(Artista artista)
    {
        var parametros = new DynamicParameters();
        parametros.Add("@unidArtista", direction: ParameterDirection.Output);
        parametros.Add("@unNombreArtistico", artista.NombreArtistico);
        parametros.Add("@unNombre", artista.Nombre);
        parametros.Add("@unApellido", artista.Apellido);

        await _conexion.ExecuteAsync("altaArtista", parametros, commandType: CommandType.StoredProcedure);

        artista.idArtista = parametros.Get<uint>("@unidArtista");

        return artista;
    }

    public async Task<IEnumerable<Artista>> ObtenerAsync()
    {
        return await _conexion.QueryAsync<Artista>("ObtenerArtistas", commandType: CommandType.StoredProcedure);
    }

    public async Task<Artista?> DetalleDeAsync(uint idArtista)
    {
        string consultarArtista = @"SELECT * FROM Artista WHERE idArtista = @idArtista";
        return await _conexion.QuerySingleOrDefaultAsync<Artista>(consultarArtista, new { idArtista });
    }

    public async Task EliminarAsync(uint idArtista)
    {
        string eliminarAlbum = @"DELETE FROM Album WHERE idArtista = @idArtista";
        await _conexion.ExecuteAsync(eliminarAlbum, new { idArtista });

        string eliminarArtista = @"DELETE FROM Artista WHERE idArtista = @idArtista";
        await _conexion.ExecuteAsync(eliminarArtista, new { idArtista });
    }
}