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

    public Genero? DetalleDe(byte id)
    {
        throw new NotImplementedException();
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


    public IList<Genero> Obtener()
    { 
        string consultarGeneros = @"SELECT * from Genero ORDER BY Genero ASC";
        var generos = _conexion.Query<Genero>(consultarGeneros);
        return generos.ToList();
    }
}