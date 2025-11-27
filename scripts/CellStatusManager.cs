using System.Collections.Generic;
using System.Linq;
using Godot;

public class CellStatusManager
{
    private Dictionary<Vector2I, bool> visibleCellUnlockStates = new();

    public void OnNewGame()
    {
        visibleCellUnlockStates.Clear();
        visibleCellUnlockStates[Vector2I.Zero] = true;
    }

    public List<Vector2I> OnCellUnlocked(Vector2I cell, Godot.Collections.Array<Vector2I> surroundingCells)
    {
        List<Vector2I> newlyShownCells = new();
        //updating surrounding cells after the above, so that in the above call, relevant tiles can be updated by checking GetCellStatus == Hidden
        //before they're made visible
        visibleCellUnlockStates[cell] = true;
        
        foreach (var surroundingCell in surroundingCells)
        {
            if (visibleCellUnlockStates.ContainsKey(surroundingCell))
                continue;
            
            visibleCellUnlockStates[surroundingCell] = false;
            newlyShownCells.Add(cell);
        }

        return newlyShownCells;
    }
    
    public CellStatus GetCellStatus(Vector2I cell)
    {
        if (visibleCellUnlockStates.ContainsKey(cell))
        {
            return visibleCellUnlockStates[cell] ? CellStatus.Unlocked : CellStatus.Locked;
        }

        return CellStatus.Hidden;
    }
    
    public Vector2I[] GetVisibleCells()
    {
        return visibleCellUnlockStates.Keys.ToArray();
    }
}
