using System;
using System.Text.Json;

namespace Spotify.Core.Entidades;

public class RegistroAuditoria
{
    public long IdAuditoria { get; set; }
    public string Tabla { get; set; }
    public string Operacion { get; set; }
    public string? IdRegistro { get; set; }
    public string? UsuarioEjecutor { get; set; }
    public DateTime Fecha { get; set; }
    public JsonDocument? Detalle { get; set; }
}