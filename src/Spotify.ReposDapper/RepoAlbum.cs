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

public Album? DetalleDe(uint idAlbum)
{
    string sql = @"
        SELECT * FROM Album WHERE idAlbum = @idAlbum;
        SELECT * FROM Artista WHERE idArtista = (
            SELECT idArtista FROM Album WHERE idAlbum = @idAlbum
        );
    ";

    using var multi = _conexion.QueryMultiple(sql, new { idAlbum });

    var album = multi.ReadSingleOrDefault<Album>();
    if (album is not null)
    {
        album.artista = multi.ReadSingleOrDefault<Artista>();
    }

    return album;
}

    public void Eliminar(uint idAlbum)
    {
        string eliminarCanciones = @"DELETE FROM Cancion WHERE idAlbum = @idAlbum";
        _conexion.Execute(eliminarCanciones, new { idAlbum });

        string eliminarAlbum = @"DELETE FROM Album WHERE idAlbum = @idAlbum";
        _conexion.Execute(eliminarAlbum, new { idAlbum });
    }

    public IList<Album> Obtener() => EjecutarSPConReturnDeTipoLista<Album>("ObtenerAlbum").ToList();
}
