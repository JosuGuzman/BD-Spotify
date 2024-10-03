namespace Spotify.ReposDapper;
public class RepoGenero : RepoGenerico, IRepoGenero
{
    public RepoGenero(IDbConnection conexion)
        : base(conexion) { }

    public byte Alta(Genero genero)
    {
        var parametros = new DynamicParameters();
        parametros.Add("@unGenero", genero.genero);
        parametros.Add("@unidGenero", direction: ParameterDirection.Output);

        _conexion.Execute("altaGenero", parametros, commandType: CommandType.StoredProcedure);

        genero.idGenero = parametros.Get<byte>("@unidGenero");
        return genero.idGenero;
    }

    public Genero? DetalleDe(byte idGenero)
    {
        var BuscarGeneroPorId = @"SELECT * FROM Genero WHERE idGenero = @idGenero";

        var Buscar = _conexion.QueryFirstOrDefault<Genero>(BuscarGeneroPorId, new {idGenero});

        return Buscar;
    }

    public void Eliminar(uint idGenero)
    {
        string eliminarHistorialReproducciones = @"
            DELETE FROM HistorialReproducción 
            WHERE idCancion IN (SELECT idCancion FROM Cancion WHERE idGenero = @idGenero)";
        _conexion.Execute(eliminarHistorialReproducciones, new { idGenero });

        string eliminarCanciones = @"DELETE FROM Cancion WHERE idGenero = @idGenero";
        _conexion.Execute(eliminarCanciones, new { idGenero });
        
        string eliminarGenero = @"DELETE FROM Genero WHERE idGenero = @idGenero";
        _conexion.Execute(eliminarGenero, new { idGenero });
    }


    public IList<Genero> Obtener() => EjecutarSPConReturnDeTipoLista<Genero>("ObtenerGeneros").ToList();
}