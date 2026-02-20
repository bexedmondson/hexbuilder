using System.Collections.Generic;
using Godot;

public partial class TileSelectorList : ScrollContainer
{
    [Export]
    private Control tileOptionParent;
    
    [Export]
    private Godot.Collections.Array<AbstractTileFilterToggleButton> filterToggles = new();

    public override void _Ready()
    {
        base._Ready();

        foreach (var filterToggle in filterToggles)
        {
            filterToggle.Toggled += OnFilterToggled;
        }
    }

    public List<TileOptionUI> GetTileOptionUIs()
    {
        List<TileOptionUI> tileOptionUIs = new();
        foreach (var child in tileOptionParent.GetChildren())
        {
            if (child is TileOptionUI tileOptionUI)
                tileOptionUIs.Add(tileOptionUI);
        }
        return tileOptionUIs;
    }

    public void AddTileOptionUI(TileOptionUI tileOptionUI)
    {
        tileOptionParent.AddChild(tileOptionUI);
        OnFilterToggled(true);
    }

    public void FocusSelected()
    {
        foreach (var child in tileOptionParent.GetChildren())
        {
            if (child is not TileOptionUI tileOptionUI || !tileOptionUI.Visible || !tileOptionUI.IsPressed())
                continue;
            
            bool wasFollowFocus = this.FollowFocus;
            this.FollowFocus = true;
            tileOptionUI.GrabFocus();
            this.FollowFocus = wasFollowFocus;
            break;
        }
    }

    private void OnFilterToggled(bool _)
    {
        foreach (var child in tileOptionParent.GetChildren())
        {
            if (child is not TileOptionUI tileOptionUI)
                continue;

            bool isFilteredOut = false;
            foreach (var filterToggle in filterToggles)
            {
                isFilteredOut = filterToggle.DoesFilterTileData(tileOptionUI.tileInfo.tileData);
                if (isFilteredOut)
                    break;
            }

            tileOptionUI.Visible = !isFilteredOut;
        }
        
        FocusSelected();
    }

    public int GetVisibleTileOptionCount()
    {
        int count = 0;
        foreach (var child in tileOptionParent.GetChildren())
        {
            if (child is TileOptionUI tileOptionUI && tileOptionUI.tileInfo.tileData.IsUnlocked())
                count++;
        }
        return count;
    }

    public void Cleanup()
    {
        for (int i = tileOptionParent.GetChildren().Count - 1; i >= 0; i--)
        {
            var child = tileOptionParent.GetChild(i);
            if (child is TileOptionUI)
                child.QueueFree();
        }
    }
}
