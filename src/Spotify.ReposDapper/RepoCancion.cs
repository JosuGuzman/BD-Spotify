namespace Spotify.ReposDapper;
public class RepoCancion : RepoGenerico, IRepoCancion
{
    public RepoCancion(IDbConnection conexion)
        : base(conexion) { }

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

    public async Task<Cancion> AltaAsync(Cancion cancion)
    {
        var parametros = new DynamicParameters();
        parametros.Add("@unidCancion", direction: ParameterDirection.Output);
        parametros.Add("@unTitulo", cancion.Titulo);
        parametros.Add("@unDuration", cancion.Duracion);
        parametros.Add("@unidAlbum", cancion.album.idAlbum);
        parametros.Add("@unidArtista", cancion.artista.idArtista);
        parametros.Add("@unidGenero", cancion.genero.idGenero);

        await _conexion.ExecuteAsync("altaCancion", parametros, commandType: CommandType.StoredProcedure);

        cancion.idCancion = parametros.Get<uint>("@unidCancion");

        return cancion;
    }

    public Cancion? DetalleDe(uint idCancion)
    {
        var BuscarCancionPorId = @"SELECT * FROM Cancion WHERE idCancion = @idCancion";

        var Buscar = _conexion.QueryFirstOrDefault<Cancion>(BuscarCancionPorId, new {idCancion});

        return Buscar;
    }

    public async Task<Cancion?> DetalleDeAsync(uint idCancion)
    {
        var BuscarCancionPorId = @"SELECT * FROM Cancion WHERE idCancion = @idCancion";

        var Buscar = await _conexion.QueryFirstOrDefaultAsync<Cancion>(BuscarCancionPorId, new { idCancion });

        return Buscar;
    }

    public Task EliminarAsync(uint id)
    {
        throw new NotImplementedException();
    }

    public List<string>? Matcheo(string Cadena)
    {
        var parametro = new DynamicParameters();
        parametro.Add("@InputCancion", Cadena);

        var Lista = _conexion.Query<string>("MatcheoCancion", parametro, commandType: CommandType.StoredProcedure);

        return Lista.ToList();
    }


    public IList<Cancion> Obtener() => EjecutarSPConReturnDeTipoLista<Cancion>("ObtenerCanciones").ToList();

    public Task<IEnumerable<Cancion>> ObtenerAsync()
    {
        throw new NotImplementedException();
    }
}