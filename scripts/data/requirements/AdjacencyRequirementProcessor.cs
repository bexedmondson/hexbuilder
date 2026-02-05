using Godot;

public class AdjacencyRequirementProcessor(AdjacencyRequirement dataRequirement) : CellInfoRequirementProcessor<AdjacencyRequirement>(dataRequirement)
{
    public override bool IsSatisfied(Vector2I cell)
    {
        var mapController = InjectionManager.Get<MapController>();
        var neighbours = mapController.BaseMapLayer.GetSurroundingCells(cell);

        foreach (var neighbour in neighbours)
        {
            if (mapController.BaseMapLayer.GetCellCustomData(neighbour) == dataRequirement.neighbourTile)
                return true;
        }
        return false;
    }
}