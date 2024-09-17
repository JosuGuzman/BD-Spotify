
namespace Spotify.ReposDapper;

public class RepoNacionalidad : RepoGenerico, IRepoNacionalidad
{
    public RepoNacionalidad(IDbConnection conexion) 
        : base(conexion) { }
    
    public uint Alta (Nacionalidad nacionalidad )
    {
        var parametros = new DynamicParameters();
        parametros.Add("@unidNacionalidad", direction: ParameterDirection.Output);
        parametros.Add("@unPais", nacionalidad.Pais);

        _conexion.Execute("altaNacionalidad", parametros, commandType: CommandType.StoredProcedure);

        nacionalidad.idNacionalidad = parametros.Get<uint>("@unidNacionalidad");

        return nacionalidad.idNacionalidad;
    }

    public void Eliminar(uint idNacionalidad)
    {
        string eliminarUsuario = @"DELETE FROM Usuario WHERE idNacionalidad = @idNacionalidad";
        _conexion.Execute(eliminarUsuario, new {idNacionalidad});

        string eliminarNacionalidad = @"DELETE FROM Nacionalidad WHERE idNacionalidad = @idNacionalidad";
        _conexion.Execute(eliminarNacionalidad, new {idNacionalidad});
    }

    public IList<Nacionalidad> Obtener ()
    {
        string consultarNacionalidades = @"SELECT * from Nacionalidad ORDER BY Pais ASC";
        var Nacionalidades = _conexion.Query<Nacionalidad>(consultarNacionalidades);
        return Nacionalidades.ToList();
    }
}