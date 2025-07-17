
namespace Spotify.ReposDapper;

public class RepoArtistaAsync : RepoGenerico, IRepoArtistaAsync
{
    public RepoArtistaAsync(IDbConnection conexion)
        : base(conexion) { }

    public async Task<Artista> AltaAsync(Artista artista)
    {
        var parametros = new DynamicParameters();
        parametros.Add("@unidArtista", direction: ParameterDirection.Output);
        parametros.Add("@unNombreArtistico", artista.NombreArtistico);
        parametros.Add("@unNombre", artista.Nombre);
        parametros.Add("@unApellido", artista.Apellido);

        await _conexion.ExecuteAsync("altaArtista", parametros, commandType: CommandType.StoredProcedure);

        artista.idArtista = parametros.Get<uint>("@unidArtista");

        return artista;
    }

    public async Task<Artista?> DetalleDeAsync(uint idArtista)
    {
        string consultarArtista = @"SELECT * FROM Artista WHERE idArtista = @idArtista";

        var artista = await _conexion.QuerySingleOrDefaultAsync<Artista>(consultarArtista, new {idArtista});

        return artista;
    }

    public async Task EliminarAsync(uint idArtista)
    {
        string eliminarAlbum = @"DELETE FROM Album WHERE idArtista = @idArtista";
        await _conexion.ExecuteAsync(eliminarAlbum, new { idArtista });
        
        string eliminarArtista = @"DELETE FROM Artista WHERE idArtista = @idArtista";
        await _conexion.ExecuteAsync(eliminarArtista, new { idArtista });
    }

        public async Task<List<Artista>> Obtener()
    { 
        var task = await EjecutarSPConReturnDeTipoListaAsync<Artista>("ObtenerArtista");
        return task.ToList();
    }
}