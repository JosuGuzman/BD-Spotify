namespace Spotify.ReposDapper;

public class RepoArtista : RepoGenerico, IRepoArtista
{
    public RepoArtista(IDbConnection conexion) 
        : base(conexion) {}

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

    public Artista? DetalleDe(uint idArtista)
    {
        string consultarArtista = @"SELECT * FROM Artista WHERE idArtista = @idArtista";
        var artista = _conexion.QuerySingleOrDefault<Artista>(consultarArtista, new {idArtista});
        return artista;
    }

    public void Eliminar(uint idArtista)
    {
        var parametros = new DynamicParameters();
        parametros.Add("@unidArtista", idArtista);
        EjecutarSPSinReturn("eliminarArtista", parametros);
    }

    public IList<Artista> Obtener() => EjecutarSPConReturnDeTipoLista<Artista>("ObtenerArtistas").ToList();
}