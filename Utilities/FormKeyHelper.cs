using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;

namespace Boutique.Utilities;

/// <summary>
/// Helper methods for parsing and formatting FormKeys, ModKeys, and related identifiers.
/// </summary>
public static class FormKeyHelper
{
    /// <summary>
    /// Formats a FormKey as "ModKey|FormID" for SkyPatcher format.
    /// </summary>
    public static string Format(FormKey formKey)
    {
        return $"{formKey.ModKey.FileName}|{formKey.ID:X8}";
    }

    /// <summary>
    /// Tries to create a FormKey from a string like "ModKey|FormID" or "FormID~ModKey".
    /// </summary>
    public static bool TryParse(string text, out FormKey formKey)
    {
        formKey = FormKey.Null;
        if (string.IsNullOrWhiteSpace(text))
            return false;

        var trimmed = text.Trim();
        string modPart;
        string formIdPart;

        if (trimmed.Contains('|'))
        {
            var parts = trimmed.Split('|', 2);
            modPart = parts[0].Trim();
            formIdPart = parts[1].Trim();
        }
        else if (trimmed.Contains('~'))
        {
            var parts = trimmed.Split('~', 2);
            formIdPart = parts[0].Trim();
            modPart = parts[1].Trim();
        }
        else
        {
            return false;
        }

        if (!TryParseModKey(modPart, out var modKey))
            return false;

        if (!TryParseFormId(formIdPart, out var id))
            return false;

        formKey = new FormKey(modKey, id);
        return true;
    }

    /// <summary>
    /// Tries to parse a ModKey from a string like "Skyrim.esm".
    /// </summary>
    public static bool TryParseModKey(string input, out ModKey modKey)
    {
        try
        {
            modKey = ModKey.FromNameAndExtension(input);
            return true;
        }
        catch
        {
            modKey = ModKey.Null;
            return false;
        }
    }

    /// <summary>
    /// Tries to parse a FormID from a string, handling "0x" prefix.
    /// </summary>
    public static bool TryParseFormId(string text, out uint id)
    {
        id = 0;
        if (string.IsNullOrWhiteSpace(text))
            return false;

        var trimmed = text.Trim();
        if (trimmed.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            trimmed = trimmed[2..];

        return uint.TryParse(trimmed, System.Globalization.NumberStyles.HexNumber, null, out id);
    }

    /// <summary>
    /// Parses an EditorID reference that may include a mod specifier.
    /// Supports formats like "EditorID", "EditorID|ModKey", "ModKey|EditorID", "EditorID~ModKey".
    /// </summary>
    public static bool TryParseEditorIdReference(string identifier, out ModKey? modKey, out string editorId)
    {
        modKey = null;
        editorId = string.Empty;

        if (string.IsNullOrWhiteSpace(identifier))
            return false;

        var trimmed = identifier.Trim();
        string? modCandidate = null;
        string? editorCandidate = null;

        var pipeIndex = trimmed.IndexOf('|');
        var tildeIndex = trimmed.IndexOf('~');

        if (pipeIndex >= 0)
        {
            var firstPart = trimmed[..pipeIndex].Trim();
            var secondPart = trimmed[(pipeIndex + 1)..].Trim();
            if (!string.IsNullOrWhiteSpace(secondPart) && TryParseModKey(secondPart, out var modFromSecond))
            {
                modKey = modFromSecond;
                editorCandidate = firstPart;
            }
            else if (!string.IsNullOrWhiteSpace(firstPart) && TryParseModKey(firstPart, out var modFromFirst))
            {
                modKey = modFromFirst;
                editorCandidate = secondPart;
            }
            else
            {
                editorCandidate = firstPart;
                modCandidate = secondPart;
            }
        }
        else if (tildeIndex >= 0)
        {
            var firstPart = trimmed[..tildeIndex].Trim();
            var secondPart = trimmed[(tildeIndex + 1)..].Trim();
            if (!string.IsNullOrWhiteSpace(secondPart) && TryParseModKey(secondPart, out var modFromSecond))
            {
                modKey = modFromSecond;
                editorCandidate = firstPart;
            }
            else if (!string.IsNullOrWhiteSpace(firstPart) && TryParseModKey(firstPart, out var modFromFirst))
            {
                modKey = modFromFirst;
                editorCandidate = secondPart;
            }
            else
            {
                editorCandidate = firstPart;
                modCandidate = secondPart;
            }
        }
        else
        {
            editorCandidate = trimmed;
        }

        if (!modKey.HasValue && !string.IsNullOrWhiteSpace(modCandidate) && TryParseModKey(modCandidate, out var parsedMod))
            modKey = parsedMod;

        if (string.IsNullOrWhiteSpace(editorCandidate))
            return false;

        editorId = editorCandidate;
        return true;
    }

    /// <summary>
    /// Formats a FormKey as "0x{FormID}~{ModKey}" for SPID format.
    /// </summary>
    public static string FormatForSpid(FormKey formKey) => $"0x{formKey.ID:X}~{formKey.ModKey.FileName}";

    /// <summary>
    /// Resolves an outfit identifier to a FormKey.
    /// Supports: tilde format (0x800~Plugin.esp), pipe format (Plugin.esp|0x800), or EditorID lookup.
    /// </summary>
    public static FormKey? ResolveOutfit(string identifier, ILinkCache<ISkyrimMod, ISkyrimModGetter> linkCache)
    {
        if (string.IsNullOrWhiteSpace(identifier))
            return null;

        // Try FormKey formats first
        if (TryParse(identifier, out var formKey))
            return formKey;

        // Try to resolve by EditorID
        var outfit = linkCache.WinningOverrides<IOutfitGetter>()
            .FirstOrDefault(o => !string.IsNullOrWhiteSpace(o.EditorID) &&
                                 o.EditorID.Equals(identifier, StringComparison.OrdinalIgnoreCase));

        return outfit?.FormKey;
    }
}
