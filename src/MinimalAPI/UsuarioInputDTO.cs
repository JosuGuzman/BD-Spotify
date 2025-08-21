namespace MinimalAPI;

public class UsuarioInputDTO
{
    public uint idUsuario { get; set; }
    public string NombreUsuario { get; set; } = "";
    public string Gmail { get; set; } = "";
    public string Contrasenia { get; set; } = "";
    public string Nacionalidad { get; set; } = "";
}