namespace Boutique.Models;

public class PatcherSettings
{
    public string SkyrimDataPath { get; set; } = string.Empty;
    public string OutputPatchPath { get; set; } = string.Empty;
    public string PatchFileName { get; set; } = "BoutiquePatch.esp";
}
