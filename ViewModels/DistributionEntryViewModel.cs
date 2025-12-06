using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using Boutique.Models;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Boutique.ViewModels;

public class DistributionEntryViewModel : ReactiveObject
{
    private ObservableCollection<NpcRecordViewModel> _selectedNpcs = new();

    public DistributionEntryViewModel(
        DistributionEntry entry,
        System.Action<DistributionEntryViewModel>? removeAction = null)
    {
        Entry = entry;
        SelectedOutfit = entry.Outfit;
        
        // Initialize selected NPCs from entry
        if (entry.NpcFormKeys.Count > 0)
        {
            var npcVms = entry.NpcFormKeys
                .Select(fk => new NpcRecordViewModel(new NpcRecord(fk, null, null, fk.ModKey)))
                .ToList();
            
            foreach (var npcVm in npcVms)
            {
                // Don't set IsSelected - that's only for temporary picker selection state
                // The NPC is tracked by being in the SelectedNpcs collection
                _selectedNpcs.Add(npcVm);
            }
        }

        // Sync SelectedOutfit changes back to Entry
        this.WhenAnyValue(x => x.SelectedOutfit)
            .Subscribe(outfit => Entry.Outfit = outfit);

        RemoveCommand = ReactiveCommand.Create(() => removeAction?.Invoke(this));
    }

    public DistributionEntry Entry { get; }

    [Reactive] public IOutfitGetter? SelectedOutfit { get; set; }

    public ObservableCollection<NpcRecordViewModel> SelectedNpcs
    {
        get => _selectedNpcs;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedNpcs, value);
            UpdateEntryNpcs();
        }
    }

    public ReactiveCommand<Unit, Unit> RemoveCommand { get; }

    [Reactive] public bool IsSelected { get; set; }

    public void UpdateEntryNpcs()
    {
        if (Entry != null)
        {
            Entry.NpcFormKeys.Clear();
            Entry.NpcFormKeys.AddRange(SelectedNpcs.Select(npc => npc.FormKey));
        }
    }

    public void AddNpc(NpcRecordViewModel npc)
    {
        // Check if NPC is already in the list by FormKey
        if (!_selectedNpcs.Any(existing => existing.FormKey == npc.FormKey))
        {
            // Don't set IsSelected - that's only for temporary picker selection state
            // The NPC is tracked by being in the SelectedNpcs collection
            _selectedNpcs.Add(npc);
            UpdateEntryNpcs();
        }
    }

    public void RemoveNpc(NpcRecordViewModel npc)
    {
        if (_selectedNpcs.Remove(npc))
        {
            // Don't modify IsSelected - that's only for temporary picker selection state
            UpdateEntryNpcs();
        }
    }
}
