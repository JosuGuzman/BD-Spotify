namespace Spotify.ReposDapper;

public class RepoCancion : RepoGenerico, IRepoCancion
{
    public RepoCancion(IDbConnection conexion)
        : base(conexion) {}

    public uint Alta(Cancion cancion)
    {
        var parametros = new DynamicParameters();
        parametros.Add("@unidCancion", direction: ParameterDirection.Output);
        parametros.Add("@unTitulo", cancion.Titulo);
        parametros.Add("@unDuration", cancion.Duracion);
        parametros.Add("@unidAlbum", cancion.album.idAlbum);
        parametros.Add("@unidArtista", cancion.artista.idArtista);
        parametros.Add("@unidGenero", cancion.genero.idGenero);
        parametros.Add("@unArchivoMP3", cancion.ArchivoMP3);

        _conexion.Execute("altaCancion", parametros, commandType: CommandType.StoredProcedure);
        cancion.idCancion = parametros.Get<uint>("@unidCancion");
        return cancion.idCancion;
    }

    public Cancion? DetalleDe(uint idCancion)
    {
        string sql = @"
            SELECT c.*, 
                    a.idAlbum, a.Titulo, a.FechaLanzamiento, a.Portada,
                    ar.idArtista, ar.NombreArtistico, ar.Nombre, ar.Apellido,
                    g.idGenero, g.genero, g.Descripcion
            FROM Cancion c
            INNER JOIN Album a ON c.idAlbum = a.idAlbum
            INNER JOIN Artista ar ON c.idArtista = ar.idArtista
            INNER JOIN Genero g ON c.idGenero = g.idGenero
            WHERE c.idCancion = @idCancion";

        var cancion = _conexion.Query<Cancion, Album, Artista, Genero, Cancion>(sql,
            (cancion, album, artista, genero) => {
                cancion.album = album;
                cancion.artista = artista;
                cancion.genero = genero;
                return cancion;
            },
            new { idCancion },
            splitOn: "idAlbum,idArtista,idGenero"
        ).FirstOrDefault();

        return cancion;
    }

    public void Eliminar(uint idCancion)
    {
        var parametros = new DynamicParameters();
        parametros.Add("@unidCancion", idCancion);
        EjecutarSPSinReturn("eliminarCancion", parametros);
    }

    public List<string>? Matcheo(string Cadena)
    {
        var parametro = new DynamicParameters();
        parametro.Add("@InputCancion", Cadena);
        var Lista = _conexion.Query<string>("MatcheoCancion", parametro, commandType: CommandType.StoredProcedure);
        return Lista.ToList();
    }

    public IList<Cancion> Obtener()
    {
        string sql = @"
            SELECT c.*, 
                    a.idAlbum, a.Titulo, a.FechaLanzamiento, a.Portada,
                    ar.idArtista, ar.NombreArtistico, ar.Nombre, ar.Apellido,
                    g.idGenero, g.genero, g.Descripcion
            FROM Cancion c
            INNER JOIN Album a ON c.idAlbum = a.idAlbum
            INNER JOIN Artista ar ON c.idArtista = ar.idArtista
            INNER JOIN Genero g ON c.idGenero = g.idGenero";

        var canciones = _conexion.Query<Cancion, Album, Artista, Genero, Cancion>(sql,
            (cancion, album, artista, genero) => {
                cancion.album = album;
                cancion.artista = artista;
                cancion.genero = genero;
                return cancion;
            },
            splitOn: "idAlbum,idArtista,idGenero"
        ).ToList();

        return canciones;
    }
}