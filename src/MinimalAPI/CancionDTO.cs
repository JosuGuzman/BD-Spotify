namespace MinimalAPI;

public class CancionOutputDTO
{
    public uint idCancion { get; set; }
    public string Titulo { get; set; } = "";
    public TimeSpan Duracion { get; set; }
    public string Artista { get; set; } = "";
}