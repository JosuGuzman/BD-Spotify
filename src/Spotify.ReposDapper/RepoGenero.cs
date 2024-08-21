using System.Data;
using Dapper;
using Spotify.Core;
using Spotify.Core.Persistencia;

namespace Spotify.ReposDapper;

public class RepoGenero : RepoGenerico, IRepoGenero
{
    public RepoGenero(IDbConnection conexion)
        : base(conexion) { }

    public void Alta(Genero genero)
    {
        var parametros = new DynamicParameters();
        parametros.Add("@unGenero", genero.Generos);
        parametros.Add("@unidGenero", direction: ParameterDirection.Output);

        _conexion.Execute("altaGenero", parametros, commandType: CommandType.StoredProcedure);

        genero.IdGenero = parametros.Get<byte>("@unidGenero");
    } 

    public IList<Genero> Obtener()
    {
        string consultarGeneros = @"SELECT * from Genero ORDER BY Genero ASC";
        var generos = _conexion.Query<Genero>(consultarGeneros);
        return generos.ToList();
    }
}