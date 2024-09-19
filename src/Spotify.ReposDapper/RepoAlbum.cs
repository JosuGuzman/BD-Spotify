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

        _conexion.Execute("altaalbum", parametros, commandType: CommandType.StoredProcedure);
        album.idAlbum = parametros.Get<uint>("@unidAlbum");
        return album.idAlbum;
    }

    public void Eliminar(uint idAlbum)
    {
        string eliminarCanciones = @"DELETE FROM Cancion WHERE idAlbum = @idAlbum";
        _conexion.Execute(eliminarCanciones, new {idAlbum});

        string eliminarAlbum = @"DELETE FROM Album WHERE idAlbum = @idAlbum";
        _conexion.Execute(eliminarAlbum, new {idAlbum});
    }

    public IList<Album> Obtener()
    {
        string consultarAlbumes = @"SELECT * from Album ORDER BY Nombre ASC";
        var Albumes = _conexion.Query<Album>(consultarAlbumes);
        return Albumes.ToList();
    }
    
}
