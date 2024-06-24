namespace Senium.Core.Settings;

public class EmailSettings
{
    public string Name { get; set; } = String.Empty;
    public string User  { get; set; } = String.Empty;
    public string Password { get; set; } = String.Empty;
    public string Server { get; set; } = String.Empty;
    public int Port { get; set; }
}