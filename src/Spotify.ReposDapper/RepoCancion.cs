namespace Spotify.ReposDapper;
public class RepoCancion : RepoGenerico, IRepoCancion
{
    public RepoCancion(IDbConnection conexion)
        : base(conexion) { }

    public uint Alta(Cancion cancion)
    {
        var parametros = new DynamicParameters();
        parametros.Add("@unidCancion", direction: ParameterDirection.Output);
        parametros.Add("@unTitulo", cancion.Titulo);
        parametros.Add("@unDuration", cancion.Duracion);
        parametros.Add("@unidAlbum", cancion.album.idAlbum);
        parametros.Add("@unidArtista", cancion.artista.idArtista);
        parametros.Add("@unidGenero", cancion.genero.idGenero);

        _conexion.Execute("altaCancion", parametros, commandType: CommandType.StoredProcedure);

        cancion.idCancion = parametros.Get<uint>("@unidCancion");

        return cancion.idCancion;
    }

    public Cancion? DetalleDe(uint idCancion)
    {
        var BuscarCancionPorId = @"SELECT * FROM Cancion WHERE idCancion = @idCancion";

        var Buscar = _conexion.QueryFirstOrDefault<Cancion>(BuscarCancionPorId, new {idCancion});

        return Buscar;
    }

    public IList<Cancion> Obtener()
    {
        var Canciones =_conexion.Query<Cancion>("ObtenerCanciones", commandType: CommandType.StoredProcedure);
        return Canciones.ToList();
    }

}