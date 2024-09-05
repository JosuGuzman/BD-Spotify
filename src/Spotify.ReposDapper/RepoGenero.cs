namespace Spotify.ReposDapper;
public class RepoGenero : RepoGenerico, IRepoGenero
{
    public RepoGenero(IDbConnection conexion)
        : base(conexion) { }

    public uint Alta(Genero genero)
    {
        var parametros = new DynamicParameters();
        parametros.Add("@unGenero", genero.genero);
        parametros.Add("@unidGenero", direction: ParameterDirection.Output);

        _conexion.Execute("altaGenero", parametros, commandType: CommandType.StoredProcedure);

        genero.IdGenero = parametros.Get<uint>("@unidGenero");
        return genero.IdGenero;
    } 
 
    public IList<Genero> Obtener()
    { 
        string consultarGeneros = @"SELECT * from Genero ORDER BY Genero ASC";
        var generos = _conexion.Query<Genero>(consultarGeneros);
        return generos.ToList();
    }
}