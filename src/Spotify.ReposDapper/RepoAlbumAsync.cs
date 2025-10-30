namespace Spotify.ReposDapper;

public class RepoAlbumAsync : RepoGenerico, IRepoAlbumAsync
{
    public RepoAlbumAsync(IDbConnection conexion) : base(conexion) { }

    public async Task<Album> AltaAsync(Album album)
    {
        var parametros = new DynamicParameters();
        parametros.Add("@unidAlbum", direction: ParameterDirection.Output);
        parametros.Add("@unTitulo", album.Titulo);
        parametros.Add("@unidArtista", album.artista.idArtista);
        parametros.Add("@unPortada", album.Portada);

        await _conexion.ExecuteAsync("altaAlbum", parametros, commandType: CommandType.StoredProcedure);
        album.idAlbum = parametros.Get<uint>("@unidAlbum");
        return album;
    }

    public async Task<Album?> DetalleDeAsync(uint idAlbum)
    {
        string sql = @"
            SELECT a.*, ar.idArtista, ar.NombreArtistico, ar.Nombre, ar.Apellido
            FROM Album a
            INNER JOIN Artista ar ON a.idArtista = ar.idArtista
            WHERE a.idAlbum = @idAlbum";

        var album = (await _conexion.QueryAsync<Album, Artista, Album>(sql,
            (album, artista) => {
                album.artista = artista;
                return album;
            },
            new { idAlbum },
            splitOn: "idArtista"
        )).FirstOrDefault();

        return album;
    }

    public async Task EliminarAsync(uint idAlbum)
    {
        var parametros = new DynamicParameters();
        parametros.Add("@unidAlbum", idAlbum);
        await _conexion.ExecuteAsync("eliminarAlbum", parametros, commandType: CommandType.StoredProcedure);
    }

    public async Task<List<Album>> Obtener()
    {
        string sql = @"
            SELECT a.*, ar.idArtista, ar.NombreArtistico, ar.Nombre, ar.Apellido
            FROM Album a
            INNER JOIN Artista ar ON a.idArtista = ar.idArtista";

        var albums = (await _conexion.QueryAsync<Album, Artista, Album>(sql,
            (album, artista) => {
                album.artista = artista;
                return album;
            },
            splitOn: "idArtista"
        )).ToList();

        return albums;
    }
}