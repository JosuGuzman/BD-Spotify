namespace Spotify.ReposDapper;

public class RepoArtista : RepoGenerico, IRepoArtista
{
    public RepoArtista(IDbConnection conexion) 
        : base(conexion) { }

    public void Alta(Artista artista)
    {
        var parametros = new DynamicParameters();
        parametros.Add("@unidArtista", direction: ParameterDirection.Output);
        parametros.Add("@unNobreArtistico", artista.NombreArtistico);
        parametros.Add("@unNombre", artista.Nombre);
        parametros.Add("@unApellido", artista.Apellido);

        _conexion.Execute("altaArtista", parametros, commandType: CommandType.StoredProcedure);

        artista.IdArtista = parametros.Get<uint>("@unidArtista");
    }

    public IList<Artista> Obtener()
    {
        string consultarArtistas = @"SELECT * from Artista ORDER BY NOmbreArtistico ASC";
        var Artistas = _conexion.Query<Artista>(consultarArtistas);
        return Artistas.ToList();
    }
}
