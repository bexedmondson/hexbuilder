using System.Collections.Generic;
using System.Linq;
using Godot;

public class CellStatusManager
{
    private Dictionary<Vector2I, CellStatus> visibleCellUnlockStates = new();

    private EventDispatcher eventDispatcher;

    public void OnNewGame()
    {
        visibleCellUnlockStates.Clear();
        visibleCellUnlockStates[Vector2I.Zero] = CellStatus.Unlocked;
    }

    public List<Vector2I> OnCellUnlocked(Vector2I cell, Godot.Collections.Array<Vector2I> surroundingCells)
    {
        List<Vector2I> newlyShownCells = new();
        //updating surrounding cells after the above, so that in the above call, relevant tiles can be updated
        //by checking GetCellStatus == Hidden before they're made visible
        visibleCellUnlockStates[cell] = CellStatus.Unlocked;
        
        foreach (var surroundingCell in surroundingCells)
        {
            if (visibleCellUnlockStates.ContainsKey(surroundingCell))
                continue;
            
            visibleCellUnlockStates[surroundingCell] = CellStatus.Locked;
            newlyShownCells.Add(surroundingCell);
        }

        return newlyShownCells;
    }

    public void OnCellUnlockInitiated(Vector2I cell)
    {
        SetCellBusy(cell);
    }

    public void SetCellBusy(Vector2I cell)
    {
        visibleCellUnlockStates[cell] = CellStatus.Busy;
        
    }
    
    public void SetCellUnlockedFromBusy(Vector2I cell)
    {
        if (visibleCellUnlockStates.TryGetValue(cell, out var status) && status == CellStatus.Busy)
            visibleCellUnlockStates[cell] = CellStatus.Unlocked;
        else
        {
            GD.PushWarning($"CellStatusManager: trying to set {cell} to Unlocked from Busy, but not currently set as Busy!");
        }
    }
    
    public CellStatus GetCellStatus(Vector2I cell)
    {
        return visibleCellUnlockStates.GetValueOrDefault(cell, CellStatus.Hidden);
    }
    
    public Vector2I[] GetVisibleCells()
    {
        return visibleCellUnlockStates.Keys.ToArray();
    }
}
