namespace Spotify.ReposDapper;

public class RepoReproduccion : RepoGenerico, IRepoReproduccion
{
    public RepoReproduccion(IDbConnection conexion) 
        : base(conexion) {}

    public uint Alta(Reproduccion reproduccion)
    {
        var parametros = new DynamicParameters();
        parametros.Add("@unidHistorial", direction: ParameterDirection.Output);
        parametros.Add("@unidUsuario", reproduccion.usuario.idUsuario);
        parametros.Add("@unidCancion", reproduccion.cancion.idCancion);
        parametros.Add("@unFechaReproduccion", reproduccion.FechaReproduccion);
        _conexion.Execute("altaHistorial_reproduccion", parametros, commandType: CommandType.StoredProcedure);

        reproduccion.IdHistorial = parametros.Get<uint>("@unidHistorial");

        return reproduccion.IdHistorial;
    }

    public Reproduccion? DetalleDe(uint idHistorial)
    {
        var BuscarReproduccionPorId = @"SELECT * FROM HistorialReproduccion WHERE idHistorial = @idHistorial";

        var Buscar = _conexion.QueryFirstOrDefault<Reproduccion>(BuscarReproduccionPorId, new {idHistorial});
        
        return Buscar;
    }

    public IList<Reproduccion> Obtener() => EjecutarSPConReturnDeTipoLista<Reproduccion>("ObtenerHistorialReproduccion").ToList();
}
