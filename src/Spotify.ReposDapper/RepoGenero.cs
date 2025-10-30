namespace Spotify.ReposDapper;

public class RepoGenero : RepoGenerico, IRepoGenero
{
    public RepoGenero(IDbConnection conexion)
        : base(conexion) {}

    public byte Alta(Genero genero)
    {
        var parametros = new DynamicParameters();
        parametros.Add("@unGenero", genero.genero);
        parametros.Add("@unDescripcion", genero.Descripcion);
        parametros.Add("@unidGenero", direction: ParameterDirection.Output);

        _conexion.Execute("altaGenero", parametros, commandType: CommandType.StoredProcedure);
        genero.idGenero = parametros.Get<byte>("@unidGenero");
        return genero.idGenero;
    }

    public Genero? DetalleDe(byte idGenero)
    {
        var BuscarGeneroPorId = @"SELECT * FROM Genero WHERE idGenero = @idGenero";
        var Buscar = _conexion.QueryFirstOrDefault<Genero>(BuscarGeneroPorId, new { idGenero });
        return Buscar;
    }

    public void Eliminar(byte idGenero)
    {
        var parametros = new DynamicParameters();
        parametros.Add("@unidGenero", idGenero);
        EjecutarSPSinReturn("eliminarGenero", parametros);
    }

    public IList<Genero> Obtener() => EjecutarSPConReturnDeTipoLista<Genero>("ObtenerGeneros").ToList();
}