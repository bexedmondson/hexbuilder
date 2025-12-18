using System.Collections.Generic;

public class CellBuildStatsTracker : IInjectable
{
    public CellBuildStatsTracker()
    {
        InjectionManager.Register(this);
    }

    ~CellBuildStatsTracker()
    {
        InjectionManager.Deregister(this);
    }

    private Dictionary<CustomTileData, int> tileBuildCounts = new();

    public void OnCellTileBuilt(CustomTileData tileData)
    {
        if (!tileBuildCounts.TryAdd(tileData, 1))
            tileBuildCounts[tileData]++;
    }

    public int GetBuildCount(CustomTileData tileData)
    {
        return tileBuildCounts.GetValueOrDefault(tileData);
    }
}