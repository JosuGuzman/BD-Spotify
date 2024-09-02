using System.Data;
using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace Spotify.ReposDapper.Test;

public class TestBase
{
    static readonly string _nombreConexion = "MySQL";
    protected readonly IDbConnection  Conexion;
    public TestBase()
    {
        IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile("appSettings.json", optional: true, reloadOnChange: true)
            .Build();
        string cadena = config.GetConnectionString(_nombreConexion)!;
        
        Conexion = new MySqlConnection(cadena);
    }
}