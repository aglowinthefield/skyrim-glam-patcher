using Mutagen.Bethesda.Plugins;

namespace Boutique.Models;

/// <summary>
/// Contains all NPC data needed for SPID filter matching.
/// This includes keywords, factions, race, gender, level, and other properties
/// that SPID uses to determine if a distribution applies to an NPC.
/// </summary>
public sealed class NpcFilterData
{
  public required FormKey FormKey { get; init; }
  public required string? EditorId { get; init; }
  public required string? Name { get; init; }
  public required ModKey SourceMod { get; init; }

  /// <summary>Keywords from NPC record and race keywords.</summary>
  public required IReadOnlySet<string> Keywords { get; init; }

  /// <summary>Factions the NPC belongs to.</summary>
  public required IReadOnlyList<FactionMembership> Factions { get; init; }

  /// <summary>Race FormKey.</summary>
  public required FormKey? RaceFormKey { get; init; }
  /// <summary>Race EditorID.</summary>
  public required string? RaceEditorId { get; init; }

  /// <summary>Class FormKey.</summary>
  public required FormKey? ClassFormKey { get; init; }
  /// <summary>Class EditorID.</summary>
  public required string? ClassEditorId { get; init; }

  /// <summary>Combat style FormKey.</summary>
  public required FormKey? CombatStyleFormKey { get; init; }
  /// <summary>Combat style EditorID.</summary>
  public required string? CombatStyleEditorId { get; init; }

  /// <summary>Voice type FormKey.</summary>
  public required FormKey? VoiceTypeFormKey { get; init; }
  /// <summary>Voice type EditorID.</summary>
  public required string? VoiceTypeEditorId { get; init; }

  /// <summary>Default outfit FormKey (for outfit filtering).</summary>
  public required FormKey? DefaultOutfitFormKey { get; init; }
  /// <summary>Default outfit EditorID.</summary>
  public required string? DefaultOutfitEditorId { get; init; }

  /// <summary>Whether the NPC is female.</summary>
  public required bool IsFemale { get; init; }
  /// <summary>Whether the NPC is unique.</summary>
  public required bool IsUnique { get; init; }
  /// <summary>Whether the NPC is summonable.</summary>
  public required bool IsSummonable { get; init; }
  /// <summary>Whether the NPC is a child.</summary>
  public required bool IsChild { get; init; }
  /// <summary>Whether the NPC is leveled.</summary>
  public required bool IsLeveled { get; init; }

  /// <summary>NPC level (for level filter matching).</summary>
  public required short Level { get; init; }

  /// <summary>Template FormKey (for template-based filtering).</summary>
  public required FormKey? TemplateFormKey { get; init; }
  /// <summary>Template EditorID.</summary>
  public required string? TemplateEditorId { get; init; }

  /// <summary>Display name (Name if available, otherwise EditorID).</summary>
  public string DisplayName => !string.IsNullOrWhiteSpace(Name) ? Name : EditorId ?? "(No EditorID)";
  public string FormKeyString => FormKey.ToString();
  public string ModDisplayName => SourceMod.FileName;
}

/// <summary>
/// Represents an NPC's membership in a faction with rank info.
/// </summary>
public sealed class FactionMembership
{
  public required FormKey FactionFormKey { get; init; }
  public required string? FactionEditorId { get; init; }
  public required int Rank { get; init; }
}
