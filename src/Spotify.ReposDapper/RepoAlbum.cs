
namespace Spotify.ReposDapper;

public class RepoAlbum : RepoGenerico, IRepoAlbum
{
    public RepoAlbum(IDbConnection conexion) 
        : base(conexion) { }

    public void Alta(Album album)
    {
        var parametros = new DynamicParameters();
        parametros.Add("@unidAlbum", direction: ParameterDirection.Output);
        parametros.Add("@unTitulo", album.Titulo);
        parametros.Add("@unidArtista", album.Artistas.IdArtista);

        _conexion.Execute("altaalbum", parametros, commandType: CommandType.StoredProcedure);

        album.IdAlbum = parametros.Get<uint>("@unidAlbum");
    }

    public IList<Album> Obtener()
    {
        string consultarAlbumes = @"SELECT * from Cancion ORDER BY Cancion ASC";
        var Albumes = _conexion.Query<Album>(consultarAlbumes);
        return Albumes.ToList();
    }
}
