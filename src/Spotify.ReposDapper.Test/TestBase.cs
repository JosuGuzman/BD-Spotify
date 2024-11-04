using System.Data;
using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace Spotify.ReposDapper.Test;

public class TestBase
{
    static readonly string _nombreConexion = "MySQL";
    protected readonly IDbConnection  Conexion;
    public TestBase() : this (_nombreConexion) { }
    public TestBase(string nombreConexion)
    {
        IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile("appSettings.json", optional: true, reloadOnChange: true)
            .Build();
        string cadena = config.GetConnectionString(nombreConexion)!;
        
        Conexion = new MySqlConnection(cadena);
    }
}