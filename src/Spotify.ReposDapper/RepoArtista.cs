using System.Net.Http.Headers;

namespace Spotify.ReposDapper;

public class RepoArtista : RepoGenerico, IRepoArtista
{
    public RepoArtista(IDbConnection conexion) 
        : base(conexion) { }

    public uint Alta(Artista artista)
    {
        var parametros = new DynamicParameters();
        parametros.Add("@unidArtista", direction: ParameterDirection.Output);
        parametros.Add("@unNobreArtistico", artista.NombreArtistico);
        parametros.Add("@unNombre", artista.Nombre);
        parametros.Add("@unApellido", artista.Apellido);

        _conexion.Execute("altaArtista", parametros, commandType: CommandType.StoredProcedure);

        artista.idArtista = parametros.Get<uint>("@unidArtista");

        return artista.idArtista;
    }

    public void Eliminar(uint elemento)
    {
        throw new NotImplementedException();
    }

    public IList<Artista> Obtener()
    {
        string consultarArtistas = @"SELECT * from Artista ORDER BY NombreArtistico ASC";
        var Artistas = _conexion.Query<Artista>(consultarArtistas);
        return Artistas.ToList();
    }
}
