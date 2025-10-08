namespace Spotify.ReposDapper;
public class RepoCancion : RepoGenerico, IRepoCancion
{
    public RepoCancion(IDbConnection conexion)
        : base(conexion) {}

    public uint Alta(Cancion cancion)
    {
        var parametros = new DynamicParameters();
        parametros.Add("@unidCancion", direction: ParameterDirection.Output);
        parametros.Add("@unTitulo", cancion.Titulo);
        parametros.Add("@unDuration", cancion.Duracion);
        parametros.Add("@unidAlbum", cancion.album.idAlbum);
        parametros.Add("@unidArtista", cancion.artista.idArtista);
        parametros.Add("@unidGenero", cancion.genero.idGenero);

        _conexion.Execute("altaCancion", parametros, commandType: CommandType.StoredProcedure);

        cancion.idCancion = parametros.Get<uint>("@unidCancion");

        return cancion.idCancion;
    }

    public Cancion? DetalleDe(uint idCancion)
    {
        var BuscarCancionPorId = @"SELECT * FROM Cancion WHERE idCancion = @idCancion";

        var Buscar = _conexion.QueryFirstOrDefault<Cancion>(BuscarCancionPorId, new {idCancion});

        return Buscar;
    }

    public void Eliminar(uint idCancion)
    {
        try
        {
            // Primero eliminar registros relacionados en HistorialReproduccion
            string eliminarHistorial = @"DELETE FROM HistorialReproduccion WHERE idCancion = @idCancion";
            _conexion.Execute(eliminarHistorial, new { idCancion });

            // Luego eliminar la canción
            var parametros = new DynamicParameters();
            parametros.Add("@unidCancion", idCancion);
            
            EjecutarSPSinReturn("eliminarCancion", parametros);
            
            // Alternativa si no existe el SP:
            // string eliminarCancion = @"DELETE FROM Cancion WHERE idCancion = @idCancion";
            // _conexion.Execute(eliminarCancion, new { idCancion });
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al eliminar la canción: {ex.Message}", ex);
        }
    }

    public List<string>? Matcheo(string Cadena)
    {
        var parametro = new DynamicParameters();
        parametro.Add("@InputCancion", Cadena);

        var Lista = _conexion.Query<string>("MatcheoCancion", parametro, commandType: CommandType.StoredProcedure);

        return Lista.ToList();
    }

    public IList<Cancion> Obtener() => EjecutarSPConReturnDeTipoLista<Cancion>("ObtenerCanciones").ToList();
}