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
        parametros.Add("@unPortada", album.Portada);

        _conexion.Execute("altaAlbum", parametros, commandType: CommandType.StoredProcedure);
        album.idAlbum = parametros.Get<uint>("@unidAlbum");
        return album.idAlbum;
    }

    public Album? DetalleDe(uint idAlbum)
    {
        string sql = @"
            SELECT a.*, ar.idArtista, ar.NombreArtistico, ar.Nombre, ar.Apellido
            FROM Album a
            INNER JOIN Artista ar ON a.idArtista = ar.idArtista
            WHERE a.idAlbum = @idAlbum";

        var album = _conexion.Query<Album, Artista, Album>(sql,
            (album, artista) => {
                album.artista = artista;
                return album;
            },
            new { idAlbum },
            splitOn: "idArtista"
        ).FirstOrDefault();

        return album;
    }

    public void Eliminar(uint idAlbum)
    {
        var parametros = new DynamicParameters();
        parametros.Add("@unidAlbum", idAlbum);
        EjecutarSPSinReturn("eliminarAlbum", parametros);
    }

    public IList<Album> Obtener()
    {
        string sql = @"
            SELECT a.*, ar.idArtista, ar.NombreArtistico, ar.Nombre, ar.Apellido
            FROM Album a
            INNER JOIN Artista ar ON a.idArtista = ar.idArtista";

        var albums = _conexion.Query<Album, Artista, Album>(sql,
            (album, artista) => {
                album.artista = artista;
                return album;
            },
            splitOn: "idArtista"
        ).ToList();

        return albums;
    }
}